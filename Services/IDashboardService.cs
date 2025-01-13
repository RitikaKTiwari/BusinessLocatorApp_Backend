using BusinessLocatorApp.Dto;

namespace BusinessLocatorApp.Services
{
    public interface IDashboardService
    {
        Task<IEnumerable<ServiceUsageDto>> GetServicesWithCountsForBusinessAsync(int businessId);
        Task<IEnumerable<WeeklyEarningsDto>> GetWeeklyEarningsForCurrentMonthAsync(int businessId);
        Task<int> GetAppointmentRequestCount(int businessId);
        Task<IEnumerable<BusinessEarningsDto>> GetBusinessEarningsComparisonAsync();
    }
}