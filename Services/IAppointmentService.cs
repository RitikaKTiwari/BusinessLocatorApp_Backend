using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLocatorApp.Dto;

namespace BusinessLocatorApp.Services
{
        public interface IAppointmentService
        {
            // Method to create a new appointment and return notification details
            Task<AppointmentServiceNotificationDto> CreateAppointmentAsync(AppointmentRequestDto request);

            // Method to approve or reject an appointment and return notification details
            Task<NotificationDto> ApproveAppointmentAsync(AppointmentApprovalDto approvalDto);

            // Method to fetch pending appointments for a specific business service
            Task<List<AppointmentServiceNotificationDto>> GetPendingAppointmentsForBusinessAsync(int businessServiceId);

            // Method to get the status of a specific appointment
            Task<string> GetAppointmentStatusAsync(int appointmentId);

            // Method to fetch all appointments for a specific business
            Task<List<AppointmentServiceNotificationDto>> GetAppointmentForBusinessAsync(int businessId);
        Task<List<AppointmentServiceNotificationDto>> GetServedServicesAsync(int userId);
        Task<List<AppointmentServiceNotificationDto>> GetBookedServicesAsync(int userId);
        }
}