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
    public class BusinessRequestController : ControllerBase
    {
        private readonly IBusinessRequestService _businessRequestService;

        public BusinessRequestController(IBusinessRequestService businessRequestService)
        {
            _businessRequestService = businessRequestService;
        }

        // POST: api/businessrequest/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitBusinessRequest([FromBody] BusinessRequestDto request)
        {
            if (request == null)
                return BadRequest("Request data cannot be null.");

            var businessRequest = await _businessRequestService.SubmitBusinessRequestAsync(request);

            if (businessRequest == null)
                return StatusCode(500, "There was an error while submitting the business request.");

            return Ok(businessRequest);
        }

        // GET: api/businessrequest/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBusinessRequests()
        {
            var requests = await _businessRequestService.GetAllBusinessRequestsAsync();

            if (requests == null)
                return NotFound("No business requests found.");

            return Ok(requests);
        }

        [HttpGet("Pending/")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPendingRequests()
        {
            var requests = await _businessRequestService.GetAllPendingRequestsAsync();

            if (requests == null)
                return NotFound("No pending requests found.");

            return Ok(requests);
        }

        // POST: api/businessrequest/approve-or-reject
        [HttpPost("approve-or-reject")]
        public async Task<IActionResult> ApproveOrReject([FromBody] BusinessApprovalDto model)
        {
            if (ModelState.IsValid)
            {
                bool isProcessed = await _businessRequestService.ApproveOrRejectAsync(
                    model.Id, model.Status, model.AdminComments);

                if (isProcessed)
                {
                    return Ok(new { message = "Request processed successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to process request. Ensure comments are provided for rejection or the request has not been processed already." });
                }
            }

            return BadRequest(ModelState);
        }

    }
}