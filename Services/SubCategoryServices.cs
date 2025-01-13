using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class SubCategoryServices : ISubCategoryServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SubCategoryServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<SubCategory> DeleteSubCategory(int subcategoryid)
        {
            var subcategory = await _context.SubCategories.SingleOrDefaultAsync(subcategory1 => subcategory1.Id == subcategoryid && subcategory1.IsActive == true);
            var services=await _context.Services.Where(service1=>service1.SubCategoryId==subcategoryid && service1.IsActive==true).ToListAsync();

            if (subcategory == null)
            {
                return null;
            }

            foreach (var service in services)
            {
                service.IsActive = false;
                service.UpdatedAt = DateTime.UtcNow;

                _context.Services.Update(service);
                await _context.SaveChangesAsync();
            }

            subcategory.IsActive = false;
            subcategory.UpdatedAt = DateTime.UtcNow;

            _context.SubCategories.Update(subcategory);
            await _context.SaveChangesAsync();

            return subcategory;
        }

        public async Task<List<SubCategory>> GetSubCategoryByCategoryID(int categoryid)
        {
            var subCategories = await _context.SubCategories.Where(subcategory => subcategory.CategoryId == categoryid && subcategory.IsActive == true).ToListAsync();
            return subCategories;
        }

        public async Task<List<ListSubcategoryDto>> GetSubCategories()
        {
            return await _context.SubCategories
                .Include(subcategory=>subcategory.category)
                .Where(subcategory => subcategory.IsActive == true)
                .Select(subcategory=>new ListSubcategoryDto
                {
                    Id=subcategory.Id,
                    Name=subcategory.Name,
                    Description=subcategory.Description,
                    CategoryId=subcategory.CategoryId,
                    CategoryName=subcategory.category.Name,
                    isActive=subcategory.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<ListSubcategoryDto>> GetAllSubCategories()
        {
            return await _context.SubCategories
                .Include(subcategory => subcategory.category)
                .Select(subcategory => new ListSubcategoryDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Description = subcategory.Description,
                    CategoryId = subcategory.CategoryId,
                    CategoryName = subcategory.category.Name,
                    isActive = subcategory.IsActive
                })
                .ToListAsync();
        }

        public async Task<ListSubcategoryDto> GetSubCategoryById(int subcategoryid)
        { 
            var subcategory1 = await _context.SubCategories
                 .Include(subcategory => subcategory.category)
                .Where(subcategory => subcategory.Id == subcategoryid && subcategory.IsActive == true)
                .Select(subcategory => new ListSubcategoryDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Description = subcategory.Description,
                    CategoryId = subcategory.CategoryId,
                    CategoryName = subcategory.category.Name,
                    isActive = subcategory.IsActive
                }).SingleOrDefaultAsync();
                

            if (subcategory1 == null)
            {
                return null;
            }

            return subcategory1;
        }

        public async Task<SubCategory> PostSubCategory(SubCategoryDto request)
        {
            var categoryExists = await _context.Categories.SingleOrDefaultAsync(category => category.Id == request.CategoryId && category.IsActive == true);
            if (categoryExists == null)
            {
                throw new Exception("Category not found.");
            }

            var existingSubCategory = await _context.SubCategories.SingleOrDefaultAsync(subcategory => subcategory.Name == request.Name && subcategory.CategoryId == request.CategoryId && subcategory.IsActive == true);
            if (existingSubCategory != null)
            {
                throw new Exception("Subcategory with the same name already exists in this category.");
            }

            var newSubCategory = new SubCategory
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SubCategories.Add(newSubCategory);
            await _context.SaveChangesAsync();

            return newSubCategory;
        }

        public async Task<SubCategory> PutSubCategory(int subcategoryid, SubCategoryDto request)
        {
            var categoryExists = await _context.Categories.SingleOrDefaultAsync(category => category.Id == request.CategoryId && category.IsActive == true);
            if (categoryExists == null)
            {
                throw new Exception("Category not found.");
            }

            var subcategory = await _context.SubCategories.SingleOrDefaultAsync(subcategory1 => subcategory1.Id == subcategoryid);
            if (subcategory == null)
            {
                throw new Exception("Subcategory not found.");
            }

            var existingSubCategory = await _context.SubCategories.SingleOrDefaultAsync(subcategory1 => subcategory1.Name == request.Name && subcategory1.CategoryId == request.CategoryId && subcategory1.Id != subcategoryid);
            if (existingSubCategory != null)
            {
                throw new Exception("Subcategory with the same name already exists in this category.");
            }

            subcategory.Name = request.Name;
            subcategory.Description = request.Description;
            subcategory.CategoryId = request.CategoryId;
            subcategory.IsActive = request.IsActive;
            subcategory.UpdatedAt = DateTime.UtcNow;

            if (request.IsActive == false)
            {
                var services = await _context.Services.Where(service1 => service1.SubCategoryId == subcategoryid && service1.IsActive==true).ToListAsync();

                foreach (var service in services)
                {
                    service.IsActive = false;
                    service.UpdatedAt = DateTime.UtcNow;

                    _context.Services.Update(service);
                    await _context.SaveChangesAsync();
                }
            }

            _context.SubCategories.Update(subcategory);
            await _context.SaveChangesAsync();

            return subcategory;
        }

    
    }
}
