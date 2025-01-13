using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryRequestController : ControllerBase
    {
        private readonly ISubCategoryRequestService _subCategoryRequestService;

        public SubCategoryRequestController(ISubCategoryRequestService subCategoryRequestService)
        {
            _subCategoryRequestService = subCategoryRequestService;
        }

        // Submit a new subcategory request
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitSubCategoryRequest([FromBody] SubCategoryRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Invalid request data.");
            }

            var subCategoryRequest = await _subCategoryRequestService.SubmitSubCategoryRequestAsync(request);
            return CreatedAtAction(nameof(GetSubCategoryRequestById), new { id = subCategoryRequest.Id }, subCategoryRequest);
        }

        // Approve or reject a subcategory request
        [HttpPost("approve-reject")]
        public async Task<IActionResult> ApproveOrRejectSubCategoryRequest([FromBody] SubCategoryApprovalDto approvalRequest)
        {
            if (approvalRequest == null || string.IsNullOrEmpty(approvalRequest.Status))
            {
                return BadRequest("Invalid approval request.");
            }

            var result = await _subCategoryRequestService.ApproveOrRejectSubCategoryAsync(approvalRequest.Id, approvalRequest.Status, approvalRequest.AdminComments);

            if (!result)
            {
                return NotFound("Subcategory request not found or already processed.");
            }

            return Ok("Subcategory request processed successfully.");
        }

        // Get all subcategory requests
        [HttpGet]
        public async Task<IActionResult> GetAllSubCategoryRequests()
        {
            var requests = await _subCategoryRequestService.GetAllSubCategoryRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Pending/")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPendingRequests()
        {
            var requests = await _subCategoryRequestService.GetAllPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Business/{businessId}")]
        public async Task<IActionResult> GetAllSubCategoryRequestsForBusiness(int businessId)
        {
            var requests = await _subCategoryRequestService.GetAllSubCategoryRequestsForBusinessAsync(businessId);
            return Ok(requests);
        }

        // Get a specific subcategory request by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryRequestById(int id)
        {
            var requests = await _subCategoryRequestService.GetAllSubCategoryRequestsAsync();
            var subCategoryRequest = requests.FirstOrDefault(scr => scr.Id == id);

            if (subCategoryRequest == null)
            {
                return NotFound("Subcategory request not found.");
            }

            return Ok(subCategoryRequest);
        }
    }
}