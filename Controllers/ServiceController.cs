using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;
using Microsoft.AspNetCore.Authorization;

namespace LocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceServices _serviceServices;

        public ServiceController(IServiceServices serviceServices)
        {
            _serviceServices = serviceServices;
        }

        // GET: api/Service
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var services = await _serviceServices.GetServices();
            if (services == null || services.Count == 0)
            {
                return Ok(null); 
            }
            return Ok(services);
        }

        [HttpGet("Admin/GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceServices.GetAllServices();
            if (services == null || services.Count == 0)
            {
                return Ok(null);
            }
            return Ok(services);
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceServices.GetServiceById(id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }
            return Ok(service);
        }

        // GET: api/Service/BySubCategoryId?subcategoryid=someSubCategory
        [HttpGet("BySubCategoryId")]
        public async Task<IActionResult> GetServiceBySubCategoryId(int subcategoryid)
        {
            var services = await _serviceServices.GetServiceBySubCategoryId(subcategoryid);
            if (services == null || services.Count == 0)
            {
                return NotFound($"No services found for subcategory {subcategoryid}.");
            }
            return Ok(services);
        }

        // POST: api/Service
        [HttpPost]
        public async Task<IActionResult> PostService([FromBody] ServiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdService = await _serviceServices.PostService(request);
                return Ok("Service Added");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Subcategory not found"))
                {
                    return NotFound("Subcategory does not exist.");
                }
                else if (ex.Message.Contains("Service with the same name already exists"))
                {
                    return BadRequest("Service with the same name already exists in this subcategory.");
                }
                return BadRequest("An error occurred.");
            }
        }

        // PUT: api/Service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, [FromBody] ServiceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedService = await _serviceServices.PutService(id, request);
                return Ok(updatedService);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Subcategory not found"))
                {
                    return NotFound("Subcategory does not exist.");
                }
                else if (ex.Message.Contains("Service with the same name already exists"))
                {
                    return BadRequest("Service with the same name already exists in this subcategory.");
                }
                else if (ex.Message.Contains("Service not found"))
                {
                    return NotFound("Service not found.");
                }
                return BadRequest("An error occurred.");
            }
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var deletedService = await _serviceServices.DeleteService(id);
            if (deletedService == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }

            return Ok($"Service with ID {id} is now inactive.");
        }

        [HttpGet("total-active-services")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalActiveServicesAsync()
        {
            var TotalActiveServices = await _serviceServices.GetTotalActiveServicesAsync();
            return Ok(new { TotalActiveServices = TotalActiveServices });
        }

        [HttpGet("top-services")]
        public async Task<IActionResult> GetTopServices([FromQuery] int top = 5)
        {
            try
            {
                var services = await _serviceServices.GetTopServices(top);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
