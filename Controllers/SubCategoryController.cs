using Microsoft.AspNetCore.Mvc;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;

namespace LocatorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryServices _subCategoryServices;

        public SubCategoryController(ISubCategoryServices subCategoryServices)
        {
            _subCategoryServices = subCategoryServices;
        }

        // GET: api/SubCategory
        [HttpGet]
        public async Task<IActionResult> GetSubCategories()
        {
            var subCategories = await _subCategoryServices.GetSubCategories();
            if (subCategories == null || subCategories.Count == 0)
            {
                return NotFound("No subcategories found.");
            }
            return Ok(subCategories);
        }

        [HttpGet("Admin/GetAllSubCategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var subCategories = await _subCategoryServices.GetAllSubCategories();
            if (subCategories == null || subCategories.Count == 0)
            {
                return NotFound("No subcategories found.");
            }
            return Ok(subCategories);
        }

        // GET: api/SubCategory/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            var subCategory = await _subCategoryServices.GetSubCategoryById(id);
            if (subCategory == null)
            {
                return NotFound($"Subcategory with ID {id} not found.");
            }
            return Ok(subCategory);
        }

        // POST: api/SubCategory
        //[HttpPost]
        //public async Task<IActionResult> PostSubCategory([FromBody] SubCategoryDto request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var createdSubCategory = await _subCategoryServices.PostSubCategory(request);
        //    if (createdSubCategory == null)
        //    {
        //        return BadRequest("Subcategory already exists under the same category.");
        //    }

        //    return Ok("Subcategory Added");
        //}

        //// PUT: api/SubCategory/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutSubCategory(int id, [FromBody] SubCategoryDto request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var updatedSubCategory = await _subCategoryServices.PutSubCategory(id, request);
        //    if (updatedSubCategory == null)
        //    {
        //        return NotFound($"Subcategory with ID {id} not found or already exists.");
        //    }
    
        //    return Ok(updatedSubCategory);
        //} / 

        [HttpPost]
        public async Task<IActionResult> PostSubCategory([FromBody] SubCategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSubCategory = await _subCategoryServices.PostSubCategory(request);
                return Ok("Subcategory Added");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Category not found"))
                {
                    return NotFound("Category does not exist.");
                }
                else if (ex.Message.Contains("Subcategory with the same name already exists"))
                {
                    return BadRequest("Subcategory with the same name already exists in this category.");
                }
                return BadRequest("An error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(int id, [FromBody] SubCategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSubCategory = await _subCategoryServices.PutSubCategory(id, request);
                return Ok(updatedSubCategory);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Category not found"))
                {
                    return NotFound("Category does not exist.");
                }
                else if (ex.Message.Contains("Subcategory with the same name already exists"))
                {
                    return BadRequest("Subcategory with the same name already exists in this category.");
                }
                else if (ex.Message.Contains("Subcategory not found"))
                {
                    return NotFound("Subcategory not found.");
                }
                return BadRequest("An error occurred.");
            }
        }


        // DELETE: api/SubCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var deletedSubCategory = await _subCategoryServices.DeleteSubCategory(id);
            if (deletedSubCategory == null)
            {
                return NotFound($"Subcategory with ID {id} not found.");
            }

            return Ok($"Subcategory with ID {id} is now inactive.");
        }

        // GET: api/SubCategory/ByCategoryId?id=someCategory
        [HttpGet("ByCategoryId")]
        public async Task<IActionResult> GetSubCategoryByCategoryID(int id)
        {
            var subCategories = await _subCategoryServices.GetSubCategoryByCategoryID(id);
            if (subCategories == null || subCategories.Count == 0)
            {
                return NotFound($"No subcategories found for category {id}.");
            }

            return Ok(subCategories);
        }
    }
}
