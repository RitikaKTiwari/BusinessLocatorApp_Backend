using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface ICategoryRequestService
    {
        Task<CategoryRequest> SubmitCategoryRequestAsync(CategoryRequestDto request);
        Task<IEnumerable<CategoryRequest>> GetAllCategoryRequestsAsync();
        Task<IEnumerable<CategoryRequest>> GetCategoryRequestsForBusinessAsync(int businessId);
        Task<bool> ApproveOrRejectCategoryAsync(int requestId, string status, string adminComments);
        Task<IEnumerable<CategoryRequest>> GetAllPendingRequestsAsync();
    }
}
