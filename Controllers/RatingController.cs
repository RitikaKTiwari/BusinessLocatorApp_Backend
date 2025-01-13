using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Services;

namespace BusinessLocatorApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // Endpoint to add a new rating
        [HttpPost]
        public IActionResult AddRating([FromBody] RatingDto ratingDto)
        {
            try
            {
                var rating = _ratingService.AddRating(ratingDto);
                return Ok(rating);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the rating.", error = ex.Message });
            }
        }

        // Endpoint to retrieve a rating by userId and businessServiceId
        [HttpGet("{userId}/{businessServiceId}")]
        public IActionResult GetRating(int userId, int businessServiceId)
        {
            try
            {
                var rating = _ratingService.GetRating(userId, businessServiceId);
                if (rating == null)
                {
                    return NotFound(new { message = "Rating not found." });
                }
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the rating.", error = ex.Message });
            }
        }
    }
}
