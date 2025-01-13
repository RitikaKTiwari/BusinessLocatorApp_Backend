using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("most-booked-services/{businessId}")]
        public async Task<IActionResult> GetServicesWithCountsForBusinessAsync(int businessId)
        {
            var services = await _service.GetServicesWithCountsForBusinessAsync(businessId);

            if (!services.Any())
                return NotFound("No booked services found for this business.");

            return Ok(services);
        }

        [HttpGet("CurrentMonthWeeklyEarnings")]
        public async Task<IActionResult> GetWeeklyEarningsForCurrentMonth(int businessId)
        {
            try
            {
                var result = await _service.GetWeeklyEarningsForCurrentMonthAsync(businessId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("BusinessEarningsComparison")]
        public async Task<IActionResult> GetBusinessEarningsComparison()
        {
            try
            {
                var businessEarnings = await _service.GetBusinessEarningsComparisonAsync();
                return Ok(businessEarnings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching data.", error = ex.Message });
            }
        }

        [HttpGet("countPendingAppointmentById/{businessId}")]
        public async Task<IActionResult> GetAppointmentRequestCount(int businessId)
        {
            var totalPendingAppointment = await _service.GetAppointmentRequestCount(businessId);
            return Ok(new { totalPendingAppointment = totalPendingAppointment });
        }
    }
}