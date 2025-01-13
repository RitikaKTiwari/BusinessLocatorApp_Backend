using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusinessLocatorApp.Services
{
    public class FavouriteService : IFavouritesService
    {
        private readonly ApplicationDbContext _context;

        public FavouriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FavouriteDto>> GetUserFavouritesAsync(int userId)
        {
            return await _context.Favourites
                .Where(f => f.UserId == userId && f.IsActive)
                .Include(f => f.businessService) // Include related data
                .Select(f => new FavouriteDto
                {
                    Id = f.Id,
                    BusinessServiceId = f.BusinessServiceId,
                    BusinessServiceName = f.businessService.service.Name,
                    BusinessServiceDescription = f.businessService.service.Description,
                    IsActive = f.IsActive,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }


        public async Task<Favourite> AddFavouriteAsync(int userId, int businessServiceId)
        {
            var existingFavourite = await _context.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BusinessServiceId == businessServiceId);

            if (existingFavourite != null && existingFavourite.IsActive)
                return existingFavourite;

            if (existingFavourite != null)
            {
                existingFavourite.IsActive = true;
                existingFavourite.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                existingFavourite = new Favourite
                {
                    UserId = userId,
                    BusinessServiceId = businessServiceId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Favourites.AddAsync(existingFavourite);
            }

            await _context.SaveChangesAsync();
            return existingFavourite;
        }

        public async Task RemoveFavouriteAsync(int userId, int businessServiceId)
        {
            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BusinessServiceId == businessServiceId);

            if (favourite != null && favourite.IsActive)
            {
                favourite.IsActive = false;
                favourite.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFavouriteAsync(int userId, int businessServiceId)
        {
            return await _context.Favourites
                .AnyAsync(f => f.UserId == userId && f.BusinessServiceId == businessServiceId && f.IsActive);
        }
    }
}