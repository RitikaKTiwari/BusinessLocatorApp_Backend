using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressServices;

        public AddressController(IAddressService addressServices)
        {
            _addressServices = addressServices;
        }


        [HttpPost]
        public async Task<ActionResult<Business>> AddAddressForUser(int userId, [FromBody] AddressDto request)
        {
            if (request == null)
            {
                return BadRequest("Address registration details are missing.");
            }

            try
            {
                var address = await _addressServices.AddAddressForUser(userId, request);
                return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            var address = await _addressServices.GetAddressByID(id);
            if (address == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            return Ok(address);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Address>> UpdateAddress(int id,AddressDto request)
        {
            var address = await _addressServices.UpdateAddress(id,request);
            if (address == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            return Ok(address);
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<Address>> GetAddressForUserId(int userId)
        {
            var address = await _addressServices.GetAddressForUser(userId);
            if (address == null)
            {
                return Ok(null);
            }

            return Ok(address);
        }

        [HttpGet("Business/{businessId}")]
        public async Task<ActionResult<Address>> GetAddressForBusinessId(int businessId)
        {
            var address = await _addressServices.GetAddressForBusiness(businessId);
            if (address == null)
            {
                return NotFound($"Address with ID {businessId} not found.");
            }

            return Ok(address);
        }
    }
}
