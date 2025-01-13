using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [Authorize] // Ensures only logged-in users can access this route
        public async Task<IActionResult> GetMyDetails()
        {
            // Get the logged-in user's ID from the token claims
            var userIdClaim = User.FindFirst("UserId")?.Value; // This should match with the claim in GenerateJwtToken


            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token.");

            // Parse userId from token
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user ID.");

            // Fetch the user's details from the database
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PasswordHash,
                    u.ContactNo,
                    ProfilePic = u.ProfilePic != null ? Convert.ToBase64String(u.ProfilePic) : null,
                    u.RoleId, // Return Role or any other details if needed
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .SingleOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            // Return the user's details in response
            return Ok(user);
        }
    }
}

