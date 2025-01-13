using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceUsageDto>> GetServicesWithCountsForBusinessAsync(int businessId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Where(a => a.Status == "Approved" && a.businessService.BusinessId == businessId)
                .ToListAsync();

            // Group by service and count the number of bookings for this particular business
            var groupedServices = appointments
                .GroupBy(a => a.businessService.ServiceId)
                .Select(group => new ServiceUsageDto
                {
                    ServiceName = group.FirstOrDefault()?.businessService?.service?.Name ?? "Unknown",
                    BookedCount = group.Count()
                })
                .OrderByDescending(s => s.BookedCount)
                .ToList();

            return groupedServices;
        }

        // New method for monthly earnings
        public async Task<IEnumerable<WeeklyEarningsDto>> GetWeeklyEarningsForCurrentMonthAsync(int businessId)
        {
            // Get the current year and month
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            // Filter appointments for the current month
            var appointments = await _context.Appointments
                .Include(a => a.businessService)
                .Where(a => a.Status == "Approved" &&
                            a.businessService.BusinessId == businessId &&
                            a.AppointmentDate.Year == currentYear &&
                            a.AppointmentDate.Month == currentMonth)
                .ToListAsync();

            // Group by week
            var weeklyEarnings = appointments
                .GroupBy(a => GetWeekOfMonth(a.AppointmentDate))
                .Select(group => new WeeklyEarningsDto
                {
                    WeekNumber = group.Key,
                    TotalEarnings = group.Sum(a => a.businessService.Price)
                })
                .OrderBy(week => week.WeekNumber)
                .ToList();

            return weeklyEarnings;
        }

        public async Task<int> GetAppointmentRequestCount(int businessId)
        {
            return await _context.Appointments.CountAsync(apt => apt.Status == "Pending" && apt.businessService.BusinessId == businessId);
        }

        public async Task<IEnumerable<BusinessEarningsDto>> GetBusinessEarningsComparisonAsync()
        {
            var businessEarnings = await _context.Appointments
                .Include(a => a.businessService)
                .ThenInclude(bs => bs.business)
                .Where(a => a.Status == "Approved")
                .GroupBy(a => a.businessService.BusinessId)
                .Select(group => new BusinessEarningsDto
                {
                    BusinessName = group.FirstOrDefault() != null &&
                                   group.FirstOrDefault().businessService != null &&
                                   group.FirstOrDefault().businessService.business != null
                                   ? group.FirstOrDefault().businessService.business.Name
                                   : "Unknown",
                    TotalEarnings = group.Sum(a => a.businessService.Price)
                })
                .OrderByDescending(b => b.TotalEarnings)
                .ToListAsync();

            return businessEarnings;
        }


        // Helper method to determine the week number of the month
        private int GetWeekOfMonth(DateOnly date)
        {
            int day = date.Day;
            return (day - 1) / 7 + 1;
        }
    }
}