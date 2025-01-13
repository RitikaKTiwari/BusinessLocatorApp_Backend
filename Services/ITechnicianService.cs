using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface ITechnicianService
    {
        Task<Technician> RegisterTechnicianAsync(TechnicianDto request);
        Task<Technician> GetTechnicianByIdAsync(int id);
        Task<List<Technician>> GetTechniciansByBusinessServiceId(int businessServiceId);
        Task<List<ListTechnicianDto>> GetTechniciansByBusinessId(int businessId);
        Task<IEnumerable<Technician>> GetAllTechniciansAsync();
        Task<Technician> UpdateTechnicianAsync(int id, TechnicianDto request);
        Task<bool> DeleteTechnicianAsync(int id);
        Task<int> GetTotalActiveTechniciansAsync();
        Task<int> GetTotalActiveTechniciansOfaParticularBusiness(int businessId);
    }
}