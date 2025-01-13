using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicianController : ControllerBase
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        // Register Technician
        [HttpPost]
        public async Task<ActionResult<Technician>> RegisterTechnician([FromBody] TechnicianDto request)
        {
            var technician = await _technicianService.RegisterTechnicianAsync(request);
            if (technician == null)
            {
                return BadRequest("Technician registration failed.");
            }
            return CreatedAtAction(nameof(GetTechnicianById), new { id = technician.Id }, technician);
        }

        // Get Technician by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Technician>> GetTechnicianById(int id)
        {
            var technician = await _technicianService.GetTechnicianByIdAsync(id);
            if (technician == null)
            {
                return NotFound();
            }
            return Ok(technician);
        }

        [HttpGet("businessservice/{businessServiceId}/technicians")]
        public async Task<IActionResult> GetTechniciansByBusinessServiceId(int businessServiceId)
        {
            try
            {
                // Fetch technicians using the service layer
                var technicians = await _technicianService.GetTechniciansByBusinessServiceId(businessServiceId);

                if (technicians == null || technicians.Count == 0)
                {
                    return NotFound("No active technicians found for the specified business service.");
                }

                return Ok(technicians);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetTechniciansByBusinessId(int businessId)
        {
            try
            {
                // Fetch technicians using the service layer
                var technicians = await _technicianService.GetTechniciansByBusinessId(businessId);

                if (technicians == null || technicians.Count == 0)
                {
                    return NotFound("No active technicians found for the specified business.");
                }

                return Ok(technicians);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Get All Technicians
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Technician>>> GetAllTechnicians()
        {
            var technicians = await _technicianService.GetAllTechniciansAsync();
            return Ok(technicians);
        }

        // Update Technician
        [HttpPut("{id}")]
        public async Task<ActionResult<Technician>> UpdateTechnician(int id, [FromBody] TechnicianDto request)
        {
            var updatedTechnician = await _technicianService.UpdateTechnicianAsync(id, request);
            if (updatedTechnician == null)
            {
                return NotFound();
            }
            return Ok(updatedTechnician);
        }

        // Delete Technician
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTechnician(int id)
        {
            var deleted = await _technicianService.DeleteTechnicianAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("total-active-technicians")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalActiveTechnicians()
        {
            var totalActiveTechnicians = await _technicianService.GetTotalActiveTechniciansAsync();
            return Ok(new { totalActiveTechnicians = totalActiveTechnicians });
        }

        [HttpGet("countTechniciansById/{businessId}")]
        public async Task<IActionResult> GetTotalActiveTechniciansOfaParticularBusiness(int businessId)
        {
            var totalActiveTechnicians = await _technicianService.GetTotalActiveTechniciansOfaParticularBusiness(businessId);
            return Ok(new { totalActiveTechnicians = totalActiveTechnicians });
        }
    }
}