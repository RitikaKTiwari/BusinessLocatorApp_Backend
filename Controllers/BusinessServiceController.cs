using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessServiceController : ControllerBase
    {
        private readonly IBusinessServiceServices _businessServiceServices;

        public BusinessServiceController(IBusinessServiceServices businessServiceServices)
        {
            _businessServiceServices = businessServiceServices;
        }

        // GET: api/BusinessService
        [HttpGet]
        public async Task<IActionResult> GetBusinessServices()
        {
            var businessServices = await _businessServiceServices.GetBusinessServices();
            if (businessServices == null)
            {
                return NotFound("No business services found.");
            }
            return Ok(businessServices);
        }

        [HttpGet("Images/{businessServiceId}")]
        public async Task<IActionResult> GetImagesForBusinessServices(int businessServiceId)
        {
            var image1 = await _businessServiceServices.GetImagesBusinessService(businessServiceId);
            if (image1 == null)
            {
                return NotFound("No business services found.");
            }
            return Ok(image1);
        }

        // GET: api/BusinessService/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessServiceById(int id)
        {
            var businessService = await _businessServiceServices.GetBusinessServiceById(id);
            if (businessService == null)
            {
                return NotFound($"Business service with ID {id} not found.");
            }
            return Ok(businessService);
        }

        [HttpGet("Business/{businessId}")]
        public async Task<IActionResult> GetBusinessServicesForBusiness(int businessId)
        {
            var businessService = await _businessServiceServices.GetBusinessServicesForBusiness(businessId);
            if (businessService == null)
            {
                return NotFound($"Business service with businessID {businessId} not found.");
            }
            return Ok(businessService);
        }

        // POST: api/BusinessService
        [HttpPost]
        public async Task<IActionResult> PostBusinessService([FromBody] BusinessServiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdBusinessService = await _businessServiceServices.PostBusinessService(request);
            if (createdBusinessService == null)
            {
                return BadRequest("Error adding business service.");
            }

            return Ok("Business service added successfully.");
        }

        // PUT: api/BusinessService/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusinessService(int id, [FromBody] BusinessServiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBusinessService = await _businessServiceServices.PutBusinessService(id, request);
            if (updatedBusinessService == null)
            {
                return NotFound($"Business service with ID {id} not found.");
            }
            return Ok(updatedBusinessService);
        }

        // DELETE: api/BusinessService/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusinessService(int id)
        {
            var deletedBusinessService = await _businessServiceServices.DeleteBusinessService(id);
            if (deletedBusinessService == null)
            {
                return NotFound($"Business service with ID {id} not found.");
            }
            return Ok($"Business service with ID {id} is now inactive.");
        }

        [HttpGet("Service/{serviceId}")]
        public async Task<IActionResult> GetBusinessServicesForServices(int serviceId)
        {
            var businessService = await _businessServiceServices.GetBusinessServiceByServiceId(serviceId);
            if (businessService == null)
            {
                return Ok(null);
            }
            return Ok(businessService);
        }

        [HttpGet("countServicesById/{businessId}")]
        public async Task<IActionResult> GetTotalActiveServicesOfaParticularBusiness(int businessId)
        {
            var totalActiveServices = await _businessServiceServices.GetTotalActiveServicesOfaParticularBusiness(businessId);
            return Ok(new { totalActiveServices = totalActiveServices });
        }
    }
}