using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userService;

        public UserController(IUserServices userService)
        {
            _userService = userService;
        }

        // Create a new User with Profile Picture as BLOB
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var response = await _userService.Register(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Update user and optionally update profile picture
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto user)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || int.Parse(userIdClaim) != id)
            {
                return Unauthorized("You are not authorized to update this user.");
            }

            var updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser); // Return the updated user in the response
        }

        [HttpPut("{id}/update_password")]
        public async Task<IActionResult> UpdatePasswordAsync(int id, [FromBody] UpdatePasswordRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.PasswordHash))
            {
                return BadRequest("Password is required.");
            }

            // Call the service to update the password
            var updatedBusiness = await _userService.UpdatePasswordAsync(id, request);

            if (updatedBusiness == null)
            {
                return NotFound("Business not found.");
            }

            return Ok(updatedBusiness);
        }

        [HttpPut("{id}/UpdateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic(int id, [FromForm] IFormFile profilePic)
        {
            var result = await _userService.UpdateProfilePicAsync(id, profilePic);

            if (!result)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "Profile picture updated successfully." });
        }

        // Get all users (Require admin role for this operation)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only allow users with "Admin" role to access this
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Get user by Id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")] // Allow both Admin and User roles to access this endpoint
        public async Task<IActionResult> GetById(int id)
        {
            // If the user is in the "User" role, check if they are trying to access their own data
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Check if the logged-in user is an Admin or trying to access their own data
            if (User.IsInRole("User"))
            {
                // Ensure the user making the request is the same as the one being requested
                if (int.Parse(userIdClaim) != id)
                {
                    return Unauthorized("You are not authorized to view this user.");
                }
            }

            // If the user is an Admin, they can access any user
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        // Delete user by Id (Only admin or the same user can delete)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id)
        {
            // Ensure the user making the request is the same as the one being deleted or is an admin
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Check if the logged-in user is an Admin or trying to access their own data
            if (User.IsInRole("User"))
            {
                // Ensure the user making the request is the same as the one being requested
                if (int.Parse(userIdClaim) != id)
                {
                    return Unauthorized("You are not authorized to view this user.");
                }
            }

            // If the user is an Admin, they can access any user
            var user = await _userService.DeleteUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpGet("total-active-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalActiveUsers()
        {
            var totalActiveUsers = await _userService.GetTotalActiveUsersAsync();
            return Ok(new { TotalActiveUsers = totalActiveUsers });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var response = await _userService.ForgotPassword(request.Email);
            if (response.Success)
            {
                return Ok(new { Message = response.Message });
            }

            return BadRequest(new { Message = response.Message });
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers()
        {
            try
            {
                var result = await _userService.GetTopCustomers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}