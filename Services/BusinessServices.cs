using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using BusinessLocatorApp.Data;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Azure.Core;

namespace BusinessLocatorApp.Services
{
    public class BusinessServices : IBusinessServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Business> _passwordHasher;

        public BusinessServices(ApplicationDbContext context, IPasswordHasher<Business> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Business> RegisterBusinessAsync(BusinessRegisterDto request)
        {
            // Step 1: Create and save the address first without a BusinessId
            var address = new Address
            {
                Street = request.Address.Street,
                Location = request.Address.Location,
                Latitude = request.Address.Latitude,
                Longitude = request.Address.Longitude,
                IsPrimary = request.Address.IsPrimary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(); // Save to generate AddressId

            // Step 2: Now create the business with the generated AddressId
            var business = new Business
            {
                Name = request.Name,
                Email = request.Email,
                AddressId = address.Id,  // Assign AddressId here
                Description = request.Description,
                ContactNo = request.ContactNo,
                RoleId = 2, // Assuming role for Business is 2
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var passwordHashString = _passwordHasher.HashPassword(business, request.PasswordHash);
            business.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString);

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync(); // Save business to generate BusinessId

            // Step 3: Update the address with the generated BusinessId
            address.BusinessId = business.Id;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync(); // Save the updated address

            return business;
        }

        public async Task<BusinessRequest> CreateBusinessRequestAsync(BusinessRequestDto request)
        {
            var address = new Address
            {
                Street = request.Address.Street,
                Location = request.Address.Location,
                Latitude = request.Address.Latitude,
                Longitude = request.Address.Longitude,
                IsPrimary = request.Address.IsPrimary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(); // Save to generate AddressId

            var businessRequest = new BusinessRequest
            {
                Name = request.Name,
                Description = request.Description,
                ContactNo = request.ContactNo,
                Email = request.Email,
                CreatedAt = DateTime.Now,
                RoleId = 2,
                Status = "Pending", // Status is set to "Pending"
                AdminComments = request.AdminComments,
            };
            if (!string.IsNullOrEmpty(request.Password))
            {
                var passwordHash = _passwordHasher.HashPassword(null, request.Password);
                businessRequest.PasswordHash = Encoding.UTF8.GetBytes(passwordHash);
            }
            businessRequest.Address = address;

            _context.BusinessRequests.Add(businessRequest);
            await _context.SaveChangesAsync(); // Save business to generate BusinessId

            // Step 3: Update the address with the generated BusinessId
            address.BusinessId = businessRequest.Id;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync(); // Save the updated address

            return businessRequest;
        }

        public async Task<IEnumerable<BusinessRequest>> GetPendingBusinessRequestsAsync()
        {
            return await _context.BusinessRequests.Where(br => br.Status == "Pending").ToListAsync();
        }

        public async Task<bool> ApproveOrRejectAsync(int requestId, string status, string adminComments)
        {
            var request = await _context.BusinessRequests
                .Include(br => br.Address)
                .FirstOrDefaultAsync(br => br.Id == requestId);

            if (request == null || request.Status != "Pending" && request.Status != "Rejected")
                return false;

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            if (status == "Rejected")
            {
                if (string.IsNullOrEmpty(adminComments))
                    return false;

                request.AdminComments = adminComments;
            }
            else if (status == "Approved")
            {
                Address address = null;
                if (request.Address != null)
                {
                    address = new Address
                    {
                        Street = request.Address.Street,
                        Location = request.Address.Location,
                        Latitude = request.Address.Latitude,
                        Longitude = request.Address.Longitude,
                        IsPrimary = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Addresses.Add(address);
                    await _context.SaveChangesAsync(); // Save to get AddressId
                }
                else
                {
                    return false;
                }

                var business = new Business
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = request.PasswordHash,
                    Description = request.Description,
                    ContactNo = request.ContactNo,
                    RoleId = request.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AddressId = address.Id
                };

                _context.Businesses.Add(business);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Business> UpdateBusinessAsync(int id, BusinessRegisterDto request)
        {
            var existingBusiness = await _context.Businesses.FindAsync(id);
            if (existingBusiness == null)
            {
                return null;
            }

            // Retrieve the associated address
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.BusinessId == id);
            if (address != null)
            {
                address.Street = request.Address.Street;
                address.Location = request.Address.Location;
                address.Latitude = request.Address.Latitude;
                address.Longitude = request.Address.Longitude;
                address.IsPrimary = request.Address.IsPrimary;
                address.UpdatedAt = DateTime.UtcNow;
            }

            existingBusiness.Name = request.Name;
            existingBusiness.Email = request.Email;
            existingBusiness.Description = request.Description;
            existingBusiness.ContactNo = request.ContactNo;
            existingBusiness.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingBusiness;
        }

        public async Task<bool> UpdateProfilePicAsync(int businessId, IFormFile profilePic)
        {
            // Find the business by ID
            var business = await _context.Businesses.FindAsync(businessId);
            if (business == null)
            {
                return false; // Business not found
            }

            if (profilePic != null && profilePic.Length > 0)
            {
                // Convert IFormFile to byte array
                using var memoryStream = new MemoryStream();
                await profilePic.CopyToAsync(memoryStream);
                business.ProfilePic = memoryStream.ToArray();
                business.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Business> UpdatePasswordAsync(int id, UpdatePasswordRequestDto request)
        {
            var existingBusiness = await _context.Businesses.FindAsync(id);
            if (existingBusiness == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.PasswordHash))
            {
                var passwordHashString = _passwordHasher.HashPassword(existingBusiness, request.PasswordHash);
                existingBusiness.PasswordHash = Encoding.UTF8.GetBytes(passwordHashString);
            }

            await _context.SaveChangesAsync();
            return existingBusiness;
        }

        public async Task<BusinessListDto> GetBusinessByIdAsync(int id)
        {
            var business = await _context.Businesses
                .Include(b => b.Address)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
            {
                return null;
            }

            return new BusinessListDto
            {
                Id = business.Id,
                Name = business.Name,
                Email = business.Email,
                Description = business.Description,
                ProfilePic = business.ProfilePic != null ? Convert.ToBase64String(business.ProfilePic) : null,
                ContactNo = business.ContactNo,
                PasswordHash = Convert.ToBase64String(business.PasswordHash),
                IsActive = business.IsActive,
                CreatedAt = business.CreatedAt,
                UpdatedAt = business.UpdatedAt,
                Address = new AddressDto
                {
                    Street = business.Address.Street,
                    Location = business.Address.Location,
                    Latitude = business.Address.Latitude,
                    Longitude = business.Address.Longitude,
                    IsPrimary = business.Address.IsPrimary
                }
            };
        }

        public async Task<IEnumerable<BusinessRegisterDto>> GetActiveBusinessesAsync()
        {
            return await _context.Businesses
                .Where(b => b.IsActive)
                .Include(b => b.Address)
                .Select(b => new BusinessRegisterDto
                {
                    Name = b.Name,
                    Email = b.Email,
                    Description = b.Description,
                    ContactNo = b.ContactNo,
                    PasswordHash = Convert.ToBase64String(b.PasswordHash),
                    Address = new AddressDto
                    {
                        Street = b.Address.Street,
                        Location = b.Address.Location,
                        Latitude = b.Address.Latitude,
                        Longitude = b.Address.Longitude,
                        IsPrimary = b.Address.IsPrimary
                    }
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteBusinessAsync(int id)
        {
            var business = await _context.Businesses
                .Include(b => b.Address)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business != null)
            {
                if (business.Address != null)
                {
                    business.IsActive = false;
                    business.Address.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BusinessListDto>> GetAllBusinessesAsync()
        {
            return await _context.Businesses
                .Include(b => b.Address)
                .Select(b => new BusinessListDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Email = b.Email,
                    Description = b.Description,
                    ContactNo = b.ContactNo,
                    PasswordHash = Convert.ToBase64String(b.PasswordHash),
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    Address = new AddressDto
                    {
                        Street = b.Address.Street,
                        Location = b.Address.Location,
                        Latitude = b.Address.Latitude,
                        Longitude = b.Address.Longitude,
                        IsPrimary = b.Address.IsPrimary
                    }
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalActiveBusinessesAsync()
        {
            return await _context.Businesses.CountAsync(b => b.IsActive);
        }


        public async Task<List<BusinessListDto>> GetTopBusiness(int top = 5)
        {
            var topBusinesses = await _context.BusinessServices
                .Where(businessService => businessService.IsActive)  // Filter active business services
                .GroupBy(businessService => businessService.BusinessId)  // Group by BusinessId
                .Select(group => new
                {
                    BusinessId = group.Key,
                    UsageCount = group.Count()  // Count how many active services each business has
                })
                .OrderByDescending(service => service.UsageCount)  // Order by UsageCount (desc)
                .Take(top)  // Limit to the top 'n' businesses
                .ToListAsync();

            var topBusinessesDetails = await _context.Businesses
                .Where(business => topBusinesses.Select(tb => tb.BusinessId).Contains(business.Id))
                .Include(business => business.Address)  // Optionally include address if needed
                .Select(business => new BusinessListDto
                {
                    Id = business.Id,
                    Name = business.Name,
                    Email = business.Email,
                    Description = business.Description,
                    ContactNo = business.ContactNo,
                    ProfilePic = business.ProfilePic != null ? Convert.ToBase64String(business.ProfilePic) : null,
                    IsActive = business.IsActive,
                    CreatedAt = business.CreatedAt,
                    UpdatedAt = business.UpdatedAt,
                    Address = new AddressDto
                    {
                        Street = business.Address.Street,
                        Location = business.Address.Location,
                        Latitude = business.Address.Latitude,
                        Longitude = business.Address.Longitude,
                        IsPrimary = business.Address.IsPrimary
                    }
                })
                .ToListAsync();

            return topBusinessesDetails;
        }

    }
}