using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public class BusinessRequestService : IBusinessRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<BusinessRequest> _passwordHasher;

        public BusinessRequestService(ApplicationDbContext context, IPasswordHasher<BusinessRequest> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Submit a new business request
        public async Task<BusinessRequest> SubmitBusinessRequestAsync(BusinessRequestDto request)
        {
            var businessRequest = new BusinessRequest
            {
                Name = request.Name,
                Email = request.Email,
                Description = request.Description,
                ContactNo = request.ContactNo,
                RoleId = 2, // Assuming role for Business is 2
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = "Pending", // Initial status is Pending
                AdminComments = request.AdminComments
            };

            if (!string.IsNullOrEmpty(request.Password))
            {
                var passwordHash = _passwordHasher.HashPassword(businessRequest, request.Password);
                businessRequest.PasswordHash = Encoding.UTF8.GetBytes(passwordHash);
            }

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

            businessRequest.Address = address;

            _context.BusinessRequests.Add(businessRequest);
            await _context.SaveChangesAsync();

            return businessRequest;
        }

        // Approve or reject a business request
        public async Task<bool> ApproveOrRejectAsync(int requestId, string status, string adminComments)
        {
            // Fetch the request by ID
            var request = await _context.BusinessRequests
                .Include(br => br.Address) // Include the Address if necessary
                .FirstOrDefaultAsync(br => br.Id == requestId);

            if (request == null)
            {
                return false; // Request not found
            }

            // Ensure that the request is still pending
            if (request.Status != "Pending" && request.Status != "Rejected")
            {
                return false; // Request has already been processed
            }

            // Update the status of the request
            request.Status = status;
            request.AdminComments = adminComments;
            // If the request is being rejected, add admin comments
            //if (status == "Rejected")
            //{
            //    if (string.IsNullOrEmpty(adminComments))
            //    {
            //        return false; // Admin comment is mandatory when rejecting
            //    }
            //    request.AdminComments = adminComments; // Add comments
            //}

            // If the request is approved, handle the business and address creation
            if (status == "Approved")
            {
                // Step 1: Insert Address into the Address table
                var address = new Address
                {
                    Street = request.Address.Street,
                    Location = request.Address.Location,
                    Latitude = request.Address.Latitude,
                    Longitude = request.Address.Longitude,
                    IsPrimary = true,  // Assuming this is the primary address
                    IsActive = true,   // Assuming the address is active by default
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Step 2: Insert the Address record into the database
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync(); // Save to get the AddressId

                // Step 3: Create Business record and link to the newly created Address
                var business = new Business
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = request.PasswordHash,
                    Description = request.Description,
                    ContactNo = request.ContactNo,
                    RoleId = 2,  // Assuming the role for a business is 2
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AddressId = address.Id  // Link to the newly created Address
                };

                // Step 4: Insert the Business record into the database
                _context.Businesses.Add(business);
            }

            // Update the UpdatedAt timestamp for the BusinessRequest
            request.UpdatedAt = DateTime.UtcNow;

            // Save all changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BusinessRequest>> GetAllBusinessRequestsAsync()
        {
            return await _context.BusinessRequests
                .Include(br => br.Address)  // Include related Address data if needed
                .ToListAsync();
        }

        public async Task<IEnumerable<BusinessRequest>> GetAllPendingRequestsAsync()
        {
            return await _context.BusinessRequests
                .Include(br => br.Address)
                .Where(br => br.Status == "Pending")// Include related Address data if needed
                .ToListAsync();
        }
    }
}