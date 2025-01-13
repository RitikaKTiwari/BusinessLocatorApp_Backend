using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IServiceRequestService
    {
        Task<ServiceRequest> SubmitServiceRequestAsync(ServiceRequestDto request);
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsForBusinessAsync(int businessId);
        Task<bool> ApproveOrRejectServiceAsync(int requestId, string status, string adminComments);
        Task<IEnumerable<ServiceRequest>> GetAllPendingRequestsAsync();
    }
}
