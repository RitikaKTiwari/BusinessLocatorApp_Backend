using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface ISubCategoryRequestService
    {
        Task<SubCategoryRequest> SubmitSubCategoryRequestAsync(SubCategoryRequestDto request);
        Task<IEnumerable<SubCategoryRequest>> GetAllSubCategoryRequestsAsync();
        Task<IEnumerable<SubCategoryRequest>> GetAllSubCategoryRequestsForBusinessAsync(int businessId);
        Task<bool> ApproveOrRejectSubCategoryAsync(int requestId, string status, string adminComments);
        Task<IEnumerable<SubCategoryRequest>> GetAllPendingRequestsAsync();
    }
}
