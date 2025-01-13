using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public interface IBusinessRequestService
    {
        Task<BusinessRequest> SubmitBusinessRequestAsync(BusinessRequestDto request);
        Task<IEnumerable<BusinessRequest>> GetAllBusinessRequestsAsync();
        Task<bool> ApproveOrRejectAsync(int requestId, string status, string adminComments);
        Task<IEnumerable<BusinessRequest>> GetAllPendingRequestsAsync();

    }
}