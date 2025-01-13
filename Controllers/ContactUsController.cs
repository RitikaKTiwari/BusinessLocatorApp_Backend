using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendContactUsMessage([FromBody] ContactUsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                await _contactUsService.SendContactUsMessageAsync(request.Name, request.Email, request.Message);
                return Ok(new { success = true, message = "Email sent successfully." });
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while sending message.");
            }
        }
    }

    public class ContactUsRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}