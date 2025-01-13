using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IBusinessServices
    {
        Task<Business> RegisterBusinessAsync(BusinessRegisterDto request);
        Task<Business> UpdateBusinessAsync(int id, BusinessRegisterDto request);
        Task<BusinessListDto> GetBusinessByIdAsync(int id);
        Task<bool> DeleteBusinessAsync(int businessId);
        Task<IEnumerable<BusinessListDto>> GetAllBusinessesAsync();
        Task<IEnumerable<BusinessRegisterDto>> GetActiveBusinessesAsync();
        Task<int> GetTotalActiveBusinessesAsync();
        Task<Business> UpdatePasswordAsync(int id, UpdatePasswordRequestDto request);
        Task<bool> ApproveOrRejectAsync(int requestId, string status, string adminComments);

        // New methods for BusinessRequest table
        Task<BusinessRequest> CreateBusinessRequestAsync(BusinessRequestDto request);
        Task<IEnumerable<BusinessRequest>> GetPendingBusinessRequestsAsync();
        Task<bool> UpdateProfilePicAsync(int businessId, IFormFile profilePic);
        Task<List<BusinessListDto>> GetTopBusiness(int top = 5);
    }

}