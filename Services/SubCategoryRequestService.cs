using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public class SubCategoryRequestService : ISubCategoryRequestService
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Submit a new subcategory request
        public async Task<SubCategoryRequest> SubmitSubCategoryRequestAsync(SubCategoryRequestDto request)
        {
            var subCategoryRequest = new SubCategoryRequest
            {
                Name = request.Name,
                Description = request.Description,
                Status = "Pending", // Initial status is Pending
                AdminComments = request.AdminComments,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = request.CategoryId,
                BusinessId = request.BusinessId // Add BusinessId
            };

            _context.SubCategoryRequests.Add(subCategoryRequest);
            await _context.SaveChangesAsync();

            return subCategoryRequest;
        }

        // Get all subcategory requests
        public async Task<IEnumerable<SubCategoryRequest>> GetAllSubCategoryRequestsAsync()
        {
            return await _context.SubCategoryRequests
                .Include(scr => scr.Category) // Include related Category data
                .Include(scr => scr.Business) // Include related Business data
                .ToListAsync();
        }

        public async Task<IEnumerable<SubCategoryRequest>> GetAllPendingRequestsAsync()
        {
            return await _context.SubCategoryRequests
                .Include(scr => scr.Category) // Include related Category data
                .Include(scr => scr.Business)
                .Where(scr => scr.Status == "Pending")// Include related Business data
                .ToListAsync();
        }

        public async Task<IEnumerable<SubCategoryRequest>> GetAllSubCategoryRequestsForBusinessAsync(int businessId)
        {
            return await _context.SubCategoryRequests
                .Include(scr => scr.Category)
                .Include(scr => scr.Business)
                .Where(scr => scr.BusinessId == businessId)
                .ToListAsync();
        }

        // Approve or reject a subcategory request
        public async Task<bool> ApproveOrRejectSubCategoryAsync(int requestId, string status, string adminComments)
        {
            var subCategoryRequest = await _context.SubCategoryRequests
                .Include(scr => scr.Category) // Load associated Category
                .Include(scr => scr.Business) // Load associated Business
                .FirstOrDefaultAsync(scr => scr.Id == requestId);

            if (subCategoryRequest == null)
            {
                return false; // Subcategory request not found
            }

            // Ensure the subcategory request is still pending
            if (subCategoryRequest.Status != "Pending" && subCategoryRequest.Status != "Rejected")
            {
                return false; // The request has already been processed
            }

            // Update the status of the request
            subCategoryRequest.Status = status;
            subCategoryRequest.AdminComments = adminComments;

            // If the request is approved, create a new subcategory record
            if (status == "Approved")
            {
                var subCategory = new SubCategory
                {
                    Name = subCategoryRequest.Name,
                    Description = subCategoryRequest.Description,
                    CategoryId = subCategoryRequest.CategoryId,
                    IsActive = true, // Set the new subcategory as active
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SubCategories.Add(subCategory);
                await _context.SaveChangesAsync(); // Save the subcategory to get the Id
            }

            // Save the subcategory request changes
            subCategoryRequest.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}