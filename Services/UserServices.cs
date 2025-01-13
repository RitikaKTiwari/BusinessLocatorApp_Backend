using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BusinessLocatorApp.Data;

namespace BusinessLocatorApp.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAuthServices _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserServices> _logger;
        private readonly IPasswordHasher<Business> _businessPasswordHasher;

        public UserServices(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher,
            IPasswordHasher<Business> businessPasswordHasher,
            IAuthServices authService,
            IEmailService emailService,
            ILogger<UserServices> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _businessPasswordHasher = businessPasswordHasher;
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
        }


        public async Task<AuthResponse> Register(UserRegisterDto request)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new AuthResponse { Success = false, Message = "User already exists" };
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
                RoleId = 1, // Default role for new user
                ProfilePic = new byte[0]
            };

            // Hash the password and store it as a byte array
            var passwordHashString = _passwordHasher.HashPassword(user, request.Password);
            user.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString); // Store as byte[]

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "User registered successfully." };
        }

        public async Task<User> UpdatePasswordAsync(int id, UpdatePasswordRequestDto request)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.PasswordHash))
            {
                var passwordHashString = _passwordHasher.HashPassword(existingUser, request.PasswordHash);
                existingUser.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString);
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> UpdateProfilePicAsync(int userId, IFormFile profilePic)
        {
            // Find the business by ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // Business not found
            }

            if (profilePic != null && profilePic.Length > 0)
            {
                // Convert IFormFile to byte array
                using var memoryStream = new MemoryStream();
                await profilePic.CopyToAsync(memoryStream);
                user.ProfilePic = memoryStream.ToArray();
                user.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> UpdateUserAsync(int id, UpdateUserDto user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Update properties
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.ContactNo = user.ContactNo;
            existingUser.IsActive = true;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Hash and update the password only if it's provided
            //if (!string.IsNullOrEmpty(user.Password))
            //{
            //    var passwordHash = _passwordHasher.HashPassword(existingUser, user.Password);
            //    existingUser.PasswordHash = Encoding.UTF8.GetBytes(passwordHash);
            //}

            //// Update profile picture if available
            //if (profilePic != null && profilePic.Length > 0)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        await profilePic.CopyToAsync(memoryStream);
            //        existingUser.ProfilePic = memoryStream.ToArray(); // Update profile picture as BLOB
            //    }
            //}

            await _context.SaveChangesAsync();
            return existingUser; // Return the updated user
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Where(user => user.RoleId == 1).ToListAsync();
        }

        public async Task<int> GetTotalActiveUsersAsync()
        {
            return await _context.Users.CountAsync(user => user.IsActive && user.RoleId == 1);
        }

        public async Task<AuthResponse> ForgotPassword(string email)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    return await ResetPasswordForUser(user);
                }

                var business = await _context.Businesses.SingleOrDefaultAsync(b => b.Email == email);
                if (business != null)
                {
                    return await ResetPasswordForBusiness(business);
                }

                return new AuthResponse { Success = false, Message = "No account found with this email." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password for email {Email}", email);
                return new AuthResponse { Success = false, Message = "An error occurred. Please try again later." };
            }
        }

        private async Task<AuthResponse> ResetPasswordForUser(User user)
        {
            try
            {
                var newPassword = GenerateRandomPassword();
                var hashedPassword = _passwordHasher.HashPassword(user, newPassword);

                user.PasswordHash = Encoding.UTF8.GetBytes(hashedPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var emailBody = $"Your password has been reset. Use the following password to log in: {newPassword}";
                await _emailService.SendEmailAsync(user.Email, "Password Reset - Business Locator", emailBody);

                return new AuthResponse { Success = true, Message = "A new password has been sent to your email." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password for user {UserId}", user.Id);
                return new AuthResponse { Success = false, Message = "An error occurred while resetting the password." };
            }
        }

        private async Task<AuthResponse> ResetPasswordForBusiness(Business business)
        {
            try
            {
                var newPassword = GenerateRandomPassword();
                var hashedPassword = _businessPasswordHasher.HashPassword(business, newPassword);

                business.PasswordHash = Encoding.UTF8.GetBytes(hashedPassword);
                business.UpdatedAt = DateTime.UtcNow;

                _context.Businesses.Update(business);
                await _context.SaveChangesAsync();

                var emailBody = $"Your password has been reset. Use the following password to log in: {newPassword}";
                await _emailService.SendEmailAsync(business.Email, "Password Reset - Business Locator", emailBody);

                return new AuthResponse { Success = true, Message = "A new password has been sent to your email." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password for business {BusinessId}", business.Id);
                return new AuthResponse { Success = false, Message = "An error occurred while resetting the password." };
            }
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }


        public async Task<List<TopCustomerDto>> GetTopCustomers()
        {
            // Step 1: Get top customers grouped by UserId
            var topCustomers = await _context.BusinessNotifications
                .GroupBy(n => n.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    BookingCount = group.Count()
                })
                .OrderByDescending(x => x.BookingCount)
                .Take(5)
                .ToListAsync();

            // Step 2: Fetch user details separately and map to DTO
            var userIds = topCustomers.Select(tc => tc.UserId).ToList();

            var users = await _context.Users
                .Where(user => userIds.Contains(user.Id))
                .ToListAsync();

            // Step 3: Combine data in memory
            var result = topCustomers
                .Join(users,
                      tc => tc.UserId,
                      user => user.Id,
                      (tc, user) => new TopCustomerDto
                      {
                          UserId = user.Id,
                          UserName = user.FirstName,

                          Email = user.Email,
                          BookingCount = tc.BookingCount
                      })
                .ToList();

            return result;
        }

    }
}