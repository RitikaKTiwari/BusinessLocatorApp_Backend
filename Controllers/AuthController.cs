//using Google.Apis.Auth;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.SqlServer.Server;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace BusinessLocatorApp.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        [HttpPost("google-login")]
//        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
//        {
//            try
//            {
//                var payload = await GoogleJsonWebSignature.ValidateAsync(request.TokenId, new GoogleJsonWebSignature.ValidationSettings
//                {
//                    Audience = new[] { "681829799975-f4sud3i9ie37vib3h5jbm4sr9tfvitfu.apps.googleusercontent.com" } // Replace with your actual client ID
//                });

//                // You can retrieve user info from the payload
//                var email = payload.Email;
//                var name = payload.Name;

//                // Perform additional backend logic here (e.g., create a user, issue a JWT, etc.)
//                return Ok(new
//                {
//                    success = true,
//                    message = "Login successful",
//                    user = new
//                    {
//                        email,
//                        name
//                    }
//                });
//            }
//            catch (InvalidJwtException ex)
//            {
//                return BadRequest(new { success = false, message = "Invalid token", error = ex.Message });
//            }
//        }
//    }

//    public class GoogleLoginRequest
//    {
//        public string TokenId { get; set; }
//    }
//}

