using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public class CategoryRequestService : ICategoryRequestService
    {
        private readonly ApplicationDbContext _context;

        public CategoryRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Approve or reject category request
        public async Task<bool> ApproveOrRejectCategoryAsync(int requestId, string status, string adminComments)
        {
            var categoryRequest = await _context.CategoryRequests
                .Include(cr => cr.Business)  // Load the associated Business
                .FirstOrDefaultAsync(cr => cr.Id == requestId);

            if (categoryRequest == null)
            {
                return false; // Category request not found
            }

            // Ensure the category request is still pending
            if (categoryRequest.Status != "Pending" && categoryRequest.Status != "Rejected")
            {
                return false; // The request has already been processed
            }

            // Update the status of the request
            categoryRequest.Status = status;
            categoryRequest.AdminComments = adminComments;

            // If the request is approved, create a new category record
            if (status == "Approved")
            {
                // Insert a new category record
                var category = new Category
                {
                    Name = categoryRequest.Name,
                    IsActive = true,  // Set the new category as active
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Insert the category into the database
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); // Save the category to get the Id

                // Optionally, you may link the new category to the business if needed
                // For example, if you want to add it as a category for the specific business
                // var business = categoryRequest.Business;
                // business.Categories.Add(category);  // Assuming a business has categories linked to it
                // await _context.SaveChangesAsync();
            }

            // Save the category request changes
            categoryRequest.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        // Get all category requests
        public async Task<IEnumerable<CategoryRequest>> GetAllCategoryRequestsAsync()
        {
            return await _context.CategoryRequests
                .Include(cr => cr.Business) // Optionally include related business data
                .ToListAsync();
        }
        public async Task<IEnumerable<CategoryRequest>> GetAllPendingRequestsAsync()
        {
            return await _context.CategoryRequests
               .Include(cr => cr.Business)
               .Where(cr => cr.Status == "Pending")
               .ToListAsync();
        }

        public async Task<IEnumerable<CategoryRequest>> GetCategoryRequestsForBusinessAsync(int businessId)
        {
            return await _context.CategoryRequests
                .Include(cr => cr.Business)
                .Where(cr => cr.BusinessId == businessId)
                .ToListAsync();
        }

        // Submit a new category request
        public async Task<CategoryRequest> SubmitCategoryRequestAsync(CategoryRequestDto request)
        {
            var categoryRequest = new CategoryRequest
            {
                Name = request.Name,
                Status = "Pending", // Initial status is Pending
                AdminComments = request.AdminComments,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BusinessId = request.BusinessId
            };

            _context.CategoryRequests.Add(categoryRequest);
            await _context.SaveChangesAsync();

            return categoryRequest;
        }
    }
}