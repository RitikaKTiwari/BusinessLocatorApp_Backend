using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        // private readonly ISearchService _searchService;

        /* public SearchController(ISearchService searchService)
         {
             _searchService = searchService;
         }
 */
        //  [HttpGet]
        /* public async Task<IActionResult> Search(string query)
         {
             if (string.IsNullOrWhiteSpace(query))
             {
                 return BadRequest("Search query cannot be empty.");
             }*/

        /*  var results = await _searchService.SearchServicesAsync(query);

          if (results == null || results.Count == 0)
          {
              return NotFound("No results found.");
          }*/

        //  return Ok(results); // Return results as JSON
    }
}