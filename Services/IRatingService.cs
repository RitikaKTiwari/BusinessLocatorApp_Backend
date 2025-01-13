using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Dto;

namespace BusinessLocatorApp.Services
{
    public interface IRatingService
    {
        Rating AddRating(RatingDto ratingDto);
        Rating GetRating(int userId, int businessServiceId);
    }
}
