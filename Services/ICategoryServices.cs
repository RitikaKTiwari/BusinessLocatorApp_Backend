using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public interface ICategoryServices
    {
        Task<List<Category>> GetCategories();
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Category> PostCategory(CategoryDto request);
        Task<Category> PutCategory(int categoryId, CategoryDto request);
        Task<Category> DeleteCategory(int categoryId);
    }
}
