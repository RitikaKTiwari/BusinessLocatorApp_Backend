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
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitServiceRequest([FromBody] ServiceRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Invalid request data.");
            }

            var serviceRequest = await _serviceRequestService.SubmitServiceRequestAsync(request);
            return CreatedAtAction(nameof(GetServiceRequestById), new { id = serviceRequest.Id }, serviceRequest);
        }

        // Approve or reject a subcategory request
        [HttpPost("approve-reject")]
        public async Task<IActionResult> ApproveOrRejectSubCategoryRequest([FromBody] ServiceApprovalDto approvalRequest)
        {
            if (approvalRequest == null || string.IsNullOrEmpty(approvalRequest.Status))
            {
                return BadRequest("Invalid approval request.");
            }

            var result = await _serviceRequestService.ApproveOrRejectServiceAsync(approvalRequest.Id, approvalRequest.Status, approvalRequest.AdminComments);

            if (!result)
            {
                return NotFound("Service request not found or already processed.");
            }

            return Ok("Service request processed successfully.");
        }

        // Get all subcategory requests
        [HttpGet]
        public async Task<IActionResult> GetAllServiceRequests()
        {
            var requests = await _serviceRequestService.GetAllServiceRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Pending/")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPendingRequests()
        {
            var requests = await _serviceRequestService.GetAllPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("Business/{businessId}")]
        public async Task<IActionResult> GetAllServiceRequestsForBusinessAsync(int businessId)
        {
            var requests = await _serviceRequestService.GetAllServiceRequestsForBusinessAsync(businessId);
            return Ok(requests);
        }

        // Get a specific subcategory request by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequestById(int id)
        {
            var requests = await _serviceRequestService.GetAllServiceRequestsAsync();
            var serviceRequest = requests.FirstOrDefault(scr => scr.Id == id);

            if (serviceRequest == null)
            {
                return NotFound("Service request not found.");
            }

            return Ok(serviceRequest);
        }

    }
}