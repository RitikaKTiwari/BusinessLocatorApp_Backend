using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApproveOrRejectServiceAsync(int requestId, string status, string adminComments)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(scr => scr.SubCategory) // Load associated Category
                .Include(scr => scr.Business) // Load associated Business
                .FirstOrDefaultAsync(scr => scr.Id == requestId);

            if (serviceRequest == null)
            {
                return false; // Subcategory request not found
            }

            // Ensure the subcategory request is still pending
            if (serviceRequest.Status != "Pending" && serviceRequest.Status != "Rejected")
            {
                return false; // The request has already been processed
            }

            // Update the status of the request
            serviceRequest.Status = status;
            serviceRequest.AdminComments = adminComments;

            // If the request is approved, create a new subcategory record
            if (status == "Approved")
            {
                var service = new Service
                {
                    Name = serviceRequest.Name,
                    Description = serviceRequest.Description,
                    SubCategoryId = serviceRequest.SubCategoryId,
                    IsActive = true, // Set the new subcategory as active
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync(); // Save the subcategory to get the Id
            }

            // Save the subcategory request changes
            serviceRequest.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            return await _context.ServiceRequests
               .Include(scr => scr.SubCategory) // Include related Category data
               .Include(scr => scr.Business) // Include related Business data
               .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllPendingRequestsAsync()
        {
            return await _context.ServiceRequests
               .Include(scr => scr.SubCategory) // Include related Category data
               .Include(scr => scr.Business)
               .Where(scr => scr.Status == "Pending")// Include related Business data
               .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsForBusinessAsync(int businessId)
        {
            return await _context.ServiceRequests
             .Include(scr => scr.SubCategory) // Include related Category data
             .Include(scr => scr.Business) // Include related Business data
             .Where(scr => scr.BusinessId == businessId)
             .ToListAsync();
        }

        public async Task<ServiceRequest> SubmitServiceRequestAsync(ServiceRequestDto request)
        {
            var serviceRequest = new ServiceRequest
            {
                Name = request.Name,
                Description = request.Description,
                Status = "Pending", // Initial status is Pending
                AdminComments = request.AdminComments,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SubCategoryId = request.SubCategoryId,
                BusinessId = request.BusinessId // Add BusinessId
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }
    }
}