using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouritesService _favouriteService;

        public FavouritesController(IFavouritesService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavourites(int userId)
        {
            var favourites = await _favouriteService.GetUserFavouritesAsync(userId);
            return Ok(favourites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavourite(int userId, int businessServiceId)
        {
            var favourite = await _favouriteService.AddFavouriteAsync(userId, businessServiceId);
            return CreatedAtAction(nameof(GetFavourites), new { userId = favourite.UserId }, favourite);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavourite(int userId, int businessServiceId)
        {
            await _favouriteService.RemoveFavouriteAsync(userId, businessServiceId);
            return NoContent();
        }

        [HttpGet("isFavourite")]
        public async Task<IActionResult> IsFavourite(int userId, int businessServiceId)
        {
            var isFavourite = await _favouriteService.IsFavouriteAsync(userId, businessServiceId);
            return Ok(isFavourite);
        }
    }
}