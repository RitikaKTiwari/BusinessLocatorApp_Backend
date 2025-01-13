using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IAuthServices _authService;

        public UserAuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var result = await _authService.Register(request);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var result = await _authService.Login(request);
            if (!result.Success)
                return BadRequest(result.Message);

            // Notice the use of 'success' (lowercase) to match the frontend expectation
            return Ok(new {Id=result.Id, success = true, token = result.Token, role = result.Role });
        }

        [HttpPost("business/login")]
        public async Task<IActionResult> BusinessLogin([FromBody] UserLoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data" });
            }

            var response = await _authService.BusinessLogin(request);
            if (!response.Success)
            {
                return Unauthorized(new { Message = response.Message });
            }

            return Ok(new
            {
                Id = response.Id,
                Token = response.Token,    // Accessing Token from AuthResponse
                Role = response.Role,      // Accessing Role from AuthResponse
                Message = response.Message // Accessing Message from AuthResponse
            });
        }
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _authService.GoogleLogin(request.TokenId);

            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new
            {
                success = true,
                message = result.Message,
                token = result.Token,
                role = result.Role,
                userId = result.Id
            });
        }
    }
}
