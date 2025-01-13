using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context; // Inject DbContext here

        // Constructor to inject DbContext
        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        /*   public async Task<List<SearchResultDto>> SearchServicesAsync(string query)
           {
               if (string.IsNullOrWhiteSpace(query))
               {
                   return new List<SearchResultDto>(); // Return empty list if the query is null or empty
               }

               var results=await _context.Businesses.Where(b=>b.Name.ToLower().Contains(query.ToLower())).ToListAsync();*/

        //if (results == null)
        //{
        //     results = await _context.Categories.
        //        Where(b => b.Name.ToLower().Contains(query.ToLower()))
        //        .Select(b=> new SearchResultDto{
        //            //BusinessName = bs.business.Name,
        //            //ServiceName = bs.service.Name,
        //            CategoryName = b.Name,
        //            //SubCategoryName = bs.service.subCategory.Name,
        //            //Description = bs.service.Description, // Example of additional info to return
        //            //BusinessId = bs.BusinessId, // Include business id if needed
        //            //ServiceId = bs.ServiceId // Include service id if needed
        //        }).ToListAsync()
        //        ;
        //}
        //var results = await _context.BusinessServices
        //    .Include(bs => bs.business) // Business name
        //    .Include(bs => bs.service)  // Service name
        //    .Include(bs => bs.service.subCategory)  // Category name
        //    .Include(bs => bs.service.subCategory.category) // Subcategory name
        //    .Where(bs => bs.business.Name.ToLower().Contains(query.ToLower()) || // Business name
        //                bs.service.Name.ToLower().Contains(query.ToLower()) || // Service name
        //                bs.service.subCategory.Name.ToLower().Contains(query.ToLower()) || // Category name
        //                bs.service.subCategory.category.Name.ToLower().Contains(query.ToLower())) // Subcategory name
        //    .Select(bs => new SearchResultDto
        //    {
        //        BusinessName = bs.business.Name,
        //        ServiceName = bs.service.Name,
        //        CategoryName = bs.service.subCategory.category.Name,
        //        SubCategoryName = bs.service.subCategory.Name,
        //        Description = bs.service.Description, // Example of additional info to return
        //        BusinessId = bs.BusinessId, // Include business id if needed
        //        ServiceId = bs.ServiceId // Include service id if needed
        //    })
        //    .ToListAsync();

        //  return results;
    }
}