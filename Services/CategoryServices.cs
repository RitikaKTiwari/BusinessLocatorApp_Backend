using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public CategoryServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _context.Categories.Where(category => category.IsActive == true).ToListAsync();
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int categoryid)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(category1 => category1.Id == categoryid && category1.IsActive == true);
            if (category == null)
            {
                return null;
            }

            return category;
        }


        public async Task<Category> PostCategory(CategoryDto request)
        {
            var existingCategory = await _context.Categories.SingleOrDefaultAsync(category1 => category1.Name == request.Name && category1.IsActive == true);
            if (existingCategory != null)
            {
                return null;
            }

            var newCategory = new Category
            {
                Name = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return newCategory;
        }

        public async Task<Category> PutCategory(int categoryid, CategoryDto request)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(category1 => category1.Id == categoryid);

            if (category == null)
            {
                return null;
            }

            category.Name = request.Name;
            category.IsActive = request.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            if (request.IsActive == false)
            {
                var subcategories = await _context.SubCategories.Where(subcategory1 => subcategory1.CategoryId == categoryid && subcategory1.IsActive == true).ToListAsync();

                foreach (var subcategory in subcategories)
                {
                    var services = await _context.Services.Where(service1 => service1.SubCategoryId == subcategory.Id && service1.IsActive == true).ToListAsync();

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
                }

            }

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> DeleteCategory(int categoryid)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(category1 => category1.Id == categoryid && category1.IsActive == true);
            var subcategories = await _context.SubCategories.Where(subcategory1 => subcategory1.CategoryId == categoryid && subcategory1.IsActive == true).ToListAsync();

            if (category == null)
            {
                return null;
            }

            foreach (var subcategory in subcategories)
            {
                var services = await _context.Services.Where(service1 => service1.SubCategoryId == subcategory.Id && service1.IsActive == true).ToListAsync();

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
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}