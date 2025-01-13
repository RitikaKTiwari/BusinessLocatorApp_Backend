using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryRequestController : ControllerBase
    {
        private readonly ICategoryRequestService _categoryRequestService;

        public CategoryRequestController(ICategoryRequestService categoryRequestService)
        {
            _categoryRequestService = categoryRequestService;
        }

        // Submit a new category request
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitCategoryRequest([FromBody] CategoryRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Invalid request data.");
            }

            var categoryRequest = await _categoryRequestService.SubmitCategoryRequestAsync(request);
            return CreatedAtAction(nameof(GetCategoryRequestById), new { id = categoryRequest.Id }, categoryRequest);
        }

        // Approve or reject a category request
        [HttpPost("approve-reject")]
        public async Task<IActionResult> ApproveOrRejectCategoryRequest([FromBody] CategoryApprovalDto approvalRequest)
        {
            if (approvalRequest == null || string.IsNullOrEmpty(approvalRequest.Status))
            {
                return BadRequest("Invalid approval request.");
            }

            var result = await _categoryRequestService.ApproveOrRejectCategoryAsync(approvalRequest.Id, approvalRequest.Status, approvalRequest.AdminComments);

            if (!result)
            {
                return NotFound("Category request not found or already processed.");
            }

            return Ok("Category request processed successfully.");
        }

        // Get all category requests
        [HttpGet]
        public async Task<IActionResult> GetAllCategoryRequests()
        {
            var requests = await _categoryRequestService.GetAllCategoryRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Pending/")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPendingRequests()
        {
            var requests = await _categoryRequestService.GetAllPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Business/{businessId}")]
        public async Task<IActionResult> GetAllCategoryRequestsForBusiness(int businessId)
        {
            var requests = await _categoryRequestService.GetCategoryRequestsForBusinessAsync(businessId);
            return Ok(requests);
        }

        // Get a specific category request by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryRequestById(int id)
        {
            var request = await _categoryRequestService.GetAllCategoryRequestsAsync();
            var categoryRequest = request.FirstOrDefault(cr => cr.Id == id);

            if (categoryRequest == null)
            {
                return NotFound("Category request not found.");
            }

            return Ok(categoryRequest);
        }
    }
}