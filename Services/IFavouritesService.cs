using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IFavouritesService
    {
        Task<IEnumerable<FavouriteDto>> GetUserFavouritesAsync(int userId);
        Task<Favourite> AddFavouriteAsync(int userId, int businessServiceId);
        Task RemoveFavouriteAsync(int userId, int businessServiceId);
        Task<bool> IsFavouriteAsync(int userId, int businessServiceId);
    }
}