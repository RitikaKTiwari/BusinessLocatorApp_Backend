using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;
using Microsoft.AspNetCore.Authorization;

namespace LocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryServices.GetCategories();
            if (categories == null)
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }

        [HttpGet("Admin/GetAllCategories")]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryServices.GetAllCategories();
            if (categories == null)
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryServices.GetCategoryById(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _categoryServices.PostCategory(request);
            if (createdCategory == null)
            {
                return BadRequest("Category already exists.");
            }

            return Ok("Category Added");
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCategory = await _categoryServices.PutCategory(id, request);
            if (updatedCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(updatedCategory);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deletedCategory = await _categoryServices.DeleteCategory(id);
            if (deletedCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok($"Category with ID {id} is now inactive.");
        }
    }
}