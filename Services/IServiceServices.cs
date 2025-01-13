using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IServiceServices
    {
        Task<List<ListServiceDto>> GetServices();
        Task<List<ListServiceDto>> GetAllServices();
        Task<ListServiceDto> GetServiceById(int serviceid);
        Task<List<Service>> GetServiceBySubCategoryId(int subcategoryid);
        Task<Service> PostService(ServiceDto request);
        Task<Service> PutService(int serviceid, ServiceDto request);
        Task<Service> DeleteService(int serviceid);
        Task<List<ListServiceDto>> GetTopServices(int top = 5);
        Task<int> GetTotalActiveServicesAsync();
    }
}
