using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IBusinessServiceServices
    {
        Task<List<ListBusinessServiceDto>> GetBusinessServices();
        Task<ListBusinessServiceDto> GetBusinessServiceById(int id);
        Task<List<ListBusinessServiceDto>> GetBusinessServicesForBusiness(int businessId);
        Task<BusinessService> PostBusinessService(BusinessServiceDto request);
        Task<BusinessService> PutBusinessService(int businessserviceid, BusinessServiceDto request);
        Task<BusinessService> DeleteBusinessService(int businessserviceid);
        Task<List<Image>> GetImagesBusinessService(int businessServiceId);
        Task<List<ListBusinessServiceDto>> GetBusinessServiceByServiceId(int serviceId);
        Task<int> GetTotalActiveServicesOfaParticularBusiness(int businessId);
    }
}
