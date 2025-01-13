using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface ISubCategoryServices
    {
        Task<List<ListSubcategoryDto>> GetSubCategories();
        Task<List<ListSubcategoryDto>> GetAllSubCategories();
        Task<ListSubcategoryDto> GetSubCategoryById(int subcategoryid);
        Task<List<SubCategory>> GetSubCategoryByCategoryID(int categoryid);
        Task<SubCategory> PostSubCategory(SubCategoryDto request);
        Task<SubCategory> PutSubCategory(int subcategoryid, SubCategoryDto request);
        Task<SubCategory> DeleteSubCategory(int subcategoryid);

    }
}
