using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // Create a new appointment
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var appointmentNotification = await _appointmentService.CreateAppointmentAsync(request);

            return Ok(appointmentNotification);
        }


        // Approve or reject an appointment
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveAppointmentAsync([FromBody] AppointmentApprovalDto approvalDto)
        {
            if (approvalDto == null)
            {
                return BadRequest("Invalid appointment approval data.");
            }

            try
            {
                var notification = await _appointmentService.ApproveAppointmentAsync(approvalDto);
                return Ok(new { AppointmentId = approvalDto.AppointmentId, Message = notification.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error approving/rejecting appointment: {ex.Message}");
            }
        }

        // Get the status of an appointment
        [HttpGet("status/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentStatusAsync(int appointmentId)
        {
            try
            {
                var status = await _appointmentService.GetAppointmentStatusAsync(appointmentId);
                return Ok(new { Status = status });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error fetching appointment status: {ex.Message}");
            }
        }

        // Get all pending appointments for a specific business service
        [HttpGet("pending/{businessServiceId}")]
        public async Task<IActionResult> GetPendingAppointmentsForBusinessAsync(int businessServiceId)
        {
            try
            {
                var appointments = await _appointmentService.GetPendingAppointmentsForBusinessAsync(businessServiceId);
                return Ok(appointments);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error fetching pending appointments: {ex.Message}");
            }
        }

        [HttpGet("Business/{businessId}")]
        public async Task<IActionResult> GetAppointmentsForBusinessAsync(int businessId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentForBusinessAsync(businessId);
                return Ok(appointments);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error fetching pending appointments: {ex.Message}");
            }
        }

        [HttpGet("booked/{userId}")]
        public async Task<IActionResult> GetBookedServices(int userId)
        {
            try
            {
                var services = await _appointmentService.GetBookedServicesAsync(userId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching booked services: {ex.Message}");
            }
        }


        [HttpGet("served/{userId}")]
        public async Task<IActionResult> GetServedServices(int userId)
        {
            try
            {
                var services = await _appointmentService.GetServedServicesAsync(userId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching served services: {ex.Message}");
            }
        }
    }
}


/*using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // Create a new appointment
        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] AppointmentRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid appointment request.");
            }

            try
            {
                var notification = await _appointmentService.CreateAppointmentAsync(request);
                return Ok(notification);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error creating appointment: {ex.Message}");
            }
        }

        // Approve or reject an appointment
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveAppointmentAsync([FromBody] AppointmentApprovalDto approvalDto)
        {
            if (approvalDto == null)
            {
                return BadRequest("Invalid appointment approval data.");
            }

            try
            {
                var notification = await _appointmentService.ApproveAppointmentAsync(approvalDto);
                return Ok(notification);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error approving/rejecting appointment: {ex.Message}");
            }
        }

        // Get the status of an appointment
        [HttpGet("status/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentStatusAsync(int appointmentId)
        {
            try
            {
                var status = await _appointmentService.GetAppointmentStatusAsync(appointmentId);
                return Ok(new { Status = status });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error fetching appointment status: {ex.Message}");
            }
        }

        // Get all pending appointments for a specific business service
        [HttpGet("pending/{businessServiceId}")]
        public async Task<IActionResult> GetPendingAppointmentsForBusinessAsync(int businessServiceId)
        {
            try
            {
                var appointments = await _appointmentService.GetPendingAppointmentsForBusinessAsync(businessServiceId);
                return Ok(appointments);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error fetching pending appointments: {ex.Message}");
            }
        }
    }
}
*/