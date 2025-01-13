using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Data;
using System;
using System.Linq;

namespace BusinessLocatorApp.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _context;

        public RatingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Rating AddRating(RatingDto ratingDto)
        {
            // Check if the user has already rated this service
            var existingRating = _context.Ratings
                .FirstOrDefault(r => r.UserId == ratingDto.UserId && r.BusinessServiceId == ratingDto.BusinessServiceId);

            if (existingRating != null)
            {
                throw new InvalidOperationException("You have already rated this service.");
            }

            var rating = new Rating
            {
                Star = ratingDto.Star,
                StarDescription = ratingDto.StarDescription,
                Comment = ratingDto.Comment,
                UserId = ratingDto.UserId,
                BusinessServiceId = ratingDto.BusinessServiceId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            return rating;
        }

        public Rating GetRating(int userId, int businessServiceId)
        {
            return _context.Ratings
                .FirstOrDefault(r => r.UserId == userId && r.BusinessServiceId == businessServiceId);
        }

    //     public Rating GetRatingForBusiness(int businessId)
    //    {
    //        return _context.Ratings
    //            .Where(ratings => ratings.businessService.BusinessId == businessId)
    //            .Select(r=> new RatingDto
    //            {
    //                r.Star,
    //                r.StarDescription,
    //                r.Comment,
    //                r.UserId,
    //                r.BusinessServiceId,
                
    //})
    //            .FirstOrDefault();
    //    }  
    }
}
