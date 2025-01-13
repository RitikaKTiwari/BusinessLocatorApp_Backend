using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Services;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessServices _businessServices;

        public BusinessController(IBusinessServices businessServices)
        {
            _businessServices = businessServices;
        }


        [HttpPost("admin/addbusiness")]
        public async Task<ActionResult<Business>> RegisterBusiness([FromBody] BusinessRegisterDto request)
        {
            if (request == null)
            {
                return BadRequest("Business registration details are missing.");
            }

            try
            {
                var business = await _businessServices.RegisterBusinessAsync(request);
                return CreatedAtAction(nameof(GetBusinessById), new { id = business.Id }, business);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Endpoint to create a business request
        [HttpPost("user/request")]
        public async Task<IActionResult> CreateBusinessAsync([FromBody] BusinessRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var businessRequest = await _businessServices.CreateBusinessRequestAsync(request);
            if (businessRequest != null)
            {
                return Ok(new { message = "Business registration request created successfully", requestId = businessRequest.Id });
            }
            else
            {
                return StatusCode(500, "An error occurred while creating the business request.");
            }
        }

        // Endpoint to get pending business requests
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBusinessRequestsAsync()
        {
            var pendingRequests = await _businessServices.GetPendingBusinessRequestsAsync();
            return Ok(pendingRequests);
        }

        [HttpPost("approve-or-reject/{requestId}")]
        public async Task<IActionResult> ApproveOrRejectAsync(int requestId, [FromBody] BusinessApprovalDto approveRejectDto)
        {
            if (string.IsNullOrEmpty(approveRejectDto.Status) || (approveRejectDto.Status != "Approved" && approveRejectDto.Status != "Rejected"))
            {
                return BadRequest("Invalid status. Status must be either 'Approved' or 'Rejected'.");
            }

            if (approveRejectDto.Status == "Rejected" && string.IsNullOrEmpty(approveRejectDto.AdminComments))
            {
                return BadRequest("Admin comments are required when rejecting the request.");
            }
            var result = await _businessServices.ApproveOrRejectAsync(requestId, approveRejectDto.Status, approveRejectDto.AdminComments);

            if (!result)
            {
                return NotFound("Business request not found or already processed.");
            }
            return Ok($"Business request {approveRejectDto.Status} successfully.");
        }


        // Other business management methods can be added as needed (e.g., update, delete, etc.)
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> GetBusinessById(int id)
        {
            var business = await _businessServices.GetBusinessByIdAsync(id);
            if (business == null)
            {
                return NotFound($"Business with ID {id} not found.");
            }

            return Ok(business);
        }

        // PUT: api/Business/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Business>> UpdateBusiness(int id, [FromBody] BusinessRegisterDto request)
        {
            var business = await _businessServices.UpdateBusinessAsync(id, request); // No profile pic update here
            if (business == null)
            {
                return NotFound($"Business with ID {id} not found.");
            }

            return Ok(business);
        }

        [HttpPut("{id}/UpdateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic(int id, [FromForm] IFormFile profilePic)
        {
            var result = await _businessServices.UpdateProfilePicAsync(id, profilePic);

            if (!result)
            {
                return NotFound(new { message = "Business not found." });
            }

            return Ok(new { message = "Profile picture updated successfully." });
        }

        [HttpPut("{id}/update_password")]
        public async Task<IActionResult> UpdatePasswordAsync(int id, [FromBody] UpdatePasswordRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.PasswordHash))
            {
                return BadRequest("Password is required.");
            }

            // Call the service to update the password
            var updatedBusiness = await _businessServices.UpdatePasswordAsync(id, request);

            if (updatedBusiness == null)
            {
                return NotFound("Business not found.");
            }

            return Ok(updatedBusiness);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            try
            {
                // Call the service method to delete the business
                await _businessServices.DeleteBusinessAsync(id);
                return NoContent(); // Return a successful response
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return an error message if business not found or another issue occurs
            }
        }

        // GET: api/Business
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessRegisterDto>>> GetActiveBusinesses()
        {
            var businesses = await _businessServices.GetActiveBusinessesAsync();
            return Ok(businesses);
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Business>>> GetAllBusinessesAsync()
        {
            var businesses = await _businessServices.GetAllBusinessesAsync();
            return Ok(businesses);
        }

        [HttpGet("total-active-businesses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalActiveBusinessesAsync()
        {
            var totalActiveBusinesses = await _businessServices.GetTotalActiveBusinessesAsync();
            return Ok(new { TotalActiveBusinesses = totalActiveBusinesses });
        }

        [HttpGet("top")]
        public async Task<ActionResult<List<BusinessListDto>>> GetTopBusinesses(int top = 5)
        {
            // Call the GetTopBusiness method from the service layer
            var topBusinesses = await _businessServices.GetTopBusiness(top);

            // Check if data is returned and handle accordingly
            if (topBusinesses == null || topBusinesses.Count == 0)
            {
                return NotFound("No businesses found.");
            }

            return Ok(topBusinesses); // Return the data with HTTP 200 status
        }
    }
}