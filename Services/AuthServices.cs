using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLocatorApp.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthServices> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthServices(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthServices> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse> Register(UserRegisterDto request)
        {
            try
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse { Success = false, Message = "User/Business already exists" };
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    ContactNo = request.ContactNo,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RoleId = 1, // Default to normal User role
                    ProfilePic = null // Set to null unless provided
                };

                var passwordHashString = _passwordHasher.HashPassword(user, request.Password);
                user.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new AuthResponse { Success = true, Message = "User registered successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a user with email {Email}", request.Email);
                return new AuthResponse { Success = false, Message = "An error occurred. Please try again later." };
            }
        }

        public async Task<AuthResponse> Login(UserLoginDto request)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email && u.IsActive==true);
                if (user == null)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials" };
                }

                var storedPasswordHashString = Encoding.UTF8.GetString(user.PasswordHash);
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, storedPasswordHashString, request.Password);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials" };
                }
                else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.PasswordHash = Encoding.UTF8.GetBytes(_passwordHasher.HashPassword(user, request.Password));
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                var token = GenerateJwtToken(user);
                return new AuthResponse
                {
                    Id=user.Id,
                    Success = true,
                    Token = token,
                    Role = GetRoleName(user.RoleId),
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user with email {Email}", request.Email);
                return new AuthResponse { Success = false, Message = "An error occurred. Please try again later." };
            }
        }

        public async Task<AuthResponse> GoogleLogin(string tokenId)
        {
            try
            {
                // Validate the Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "681829799975-f4sud3i9ie37vib3h5jbm4sr9tfvitfu.apps.googleusercontent.com" } // Replace with your actual Google client ID
                });

                var email = payload.Email;
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email && u.IsActive == true);

                if (user == null)
                {
                    // If the user does not exist, create a new user
                    user = new User
                    {
                        PasswordHash = [],
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = email,
                        ProfilePic = string.IsNullOrEmpty(payload.Picture) ? null : ConvertImageUrlToByteArray(payload.Picture),
                        ContactNo = "", // You can leave this blank or fetch it from Google if needed
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        RoleId = 1 // Default role, assuming 1 is 'User'
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate the JWT token
                var token = GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    Id = user.Id,
                    Role = GetRoleName(user.RoleId),
                    Message = "Login successful."
                };
            }
          
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Google login.");
                return new AuthResponse { Success = false, Message = "Google login failed" };
            }
        }

        private byte[] ConvertImageUrlToByteArray(string imageUrl)
        {
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    return webClient.DownloadData(imageUrl);
                }
            }
            catch (Exception)
            {
                return null; // Return null if the image can't be fetched
            }
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, GetRoleName(user.RoleId)),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Extended to 1 hour
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponse> BusinessLogin(UserLoginDto request)
        {
            try
            {
                var business = await _context.Businesses.SingleOrDefaultAsync(b => b.Email == request.Email && b.IsActive == true);
                if (business == null || !business.IsActive)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials or inactive account" };
                }

                var storedPasswordHashString = Encoding.UTF8.GetString(business.PasswordHash);
                var verificationResult = _passwordHasher.VerifyHashedPassword(null, storedPasswordHashString, request.Password);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials" };
                }
                else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    business.PasswordHash = Encoding.UTF8.GetBytes(_passwordHasher.HashPassword(null, request.Password));
                    _context.Businesses.Update(business);
                    await _context.SaveChangesAsync();
                }

                var token = GenerateBusinessJwtToken(business);
                return new AuthResponse
                {
                    Id=business.Id,
                    Success = true,
                    Token = token,
                    Role = GetRoleName(business.RoleId),
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in business with email {Email}", request.Email);
                return new AuthResponse { Success = false, Message = "An error occurred. Please try again later." };
            }
        }


        private string GenerateBusinessJwtToken(Business business)
        {
            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, business.Email),
                new Claim(ClaimTypes.Name, business.Name),
            new Claim(ClaimTypes.Role, GetRoleName(business.RoleId)),
    new Claim("BusinessId", business.Id.ToString())
};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration set to 1 hour
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRoleName(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
            return role?.RoleName ?? "User";
        }
    }
}


/*using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLocatorApp.Services
{
    public class AuthServices:IAuthServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse> Register(UserRegisterDto request)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new AuthResponse { Success = false, Message = "User/Business already exists" };
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ContactNo = request.ContactNo,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = 1,
                ProfilePic = new byte[0]
            };

            var passwordHashString = _passwordHasher.HashPassword(user, request.Password);
            user.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString); // Convert the string hash to byte[]

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "Business/User registered successfully." };
        }


        public async Task<AuthResponse> Login(UserLoginDto request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "Invalid credentials" };
            }

            // Convert the stored password hash back to a string
            var storedPasswordHashString = Encoding.UTF8.GetString(user.PasswordHash);

            // Verify the password
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, storedPasswordHashString, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return new AuthResponse { Success = false, Message = "Invalid credentials" };
            }

            var token = GenerateJwtToken(user);
            return new AuthResponse { Success = true, Token = token, Message = "Login successful." };
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
             new Claim(ClaimTypes.Name, user.FirstName),
             new Claim(ClaimTypes.Role, GetRoleName(user.RoleId)),
             new Claim("UserId", user.Id.ToString())
         };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRoleName(int roleId)
        {
            switch (roleId)
            {
                case 1: return "User";
                case 2: return "Business";
                case 3: return "Admin";
                default: return "User";
            }
        }
    }
}
*/