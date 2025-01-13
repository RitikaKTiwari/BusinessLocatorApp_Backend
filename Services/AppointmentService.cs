using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AppointmentService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Create a new appointment
        public async Task<AppointmentServiceNotificationDto> CreateAppointmentAsync(AppointmentRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Fetch the BusinessService related to the AppointmentRequest
            var businessService = await _context.BusinessServices
                .Include(bs => bs.service) // Include related Service entity
                .Include(bs => bs.business) // Include related Business entity
                .FirstOrDefaultAsync(bs => bs.Id == request.BusinessServiceId);

            if (businessService == null)
                throw new Exception("Business service not found.");

            // Fetch user details
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
                throw new Exception("User not found.");

            var userName = $"{user.FirstName} {user.LastName}";

            // Create the appointment entity
            var appointment = new Appointment
            {
                UserId = request.UserId,
                BusinessServicesID = request.BusinessServiceId,
                AppointmentDate = DateOnly.FromDateTime(request.AppointmentDateTime),
                AppointmentTime = TimeOnly.FromDateTime(request.AppointmentDateTime),
                Status = "Pending", // Default status
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Prepare and add the notification
            var notificationMessage = $"New appointment request from {userName} for {businessService.service.Name} on {request.AppointmentDateTime.ToShortDateString()} at {request.AppointmentDateTime.ToShortTimeString()}.";

            var notification = new BusinessNotification
            {
                AppointmentId = appointment.Id,
                NotificationMessage = notificationMessage,
                IsSent = false, // Notification will be sent later
                CreatedAt = DateTime.UtcNow,
                BusinessServiceId = request.BusinessServiceId,
                UserId = request.UserId
            };

            await _context.BusinessNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Send emails
            var userSubject = "Appointment Requested";
            var userBody = $"Dear {userName}, your appointment request for {businessService.service.Name} has been received for {request.AppointmentDateTime}.";
            await _emailService.SendEmailAsync(user.Email, userSubject, userBody);

            var businessSubject = "New Appointment Request";
            var businessBody = $"A new appointment has been requested for {businessService.service.Name} by {userName} on {request.AppointmentDateTime}.";
            await _emailService.SendEmailAsync(businessService.business.Email, businessSubject, businessBody);

            // Return the response DTO
            return new AppointmentServiceNotificationDto
            {
                NotificationId = notification.Id,
                AppointmentId = appointment.Id,
                BusinessServiceName = businessService.service.Name,
                UserName = userName,
                AppointmentDate = appointment.AppointmentDate.ToString(),
                AppointmentTime = appointment.AppointmentTime.ToString(),
                NotificationMessage = notificationMessage
            };
        }

        // Approve or reject an appointment
        public async Task<NotificationDto> ApproveAppointmentAsync(AppointmentApprovalDto approvalDto)
        {
            // Fetch the appointment details
            var appointment = await _context.Appointments
                .Include(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Include(a => a.user)
                .FirstOrDefaultAsync(a => a.Id == approvalDto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            // Update the appointment status based on approval
            appointment.Status = approvalDto.IsApproved ? "Approved" : "Rejected";
            appointment.IsActive = false;  // Mark the appointment as inactive once processed
            appointment.UpdatedAt = DateTime.UtcNow;  // Update timestamp

            // Save the updated appointment to the database
            await _context.SaveChangesAsync();

            // Prepare notification message based on approval or rejection
            var notificationMessage = approvalDto.IsApproved
                ? $"Appointment ID {appointment.Id} has been approved."
                : $"Appointment ID {appointment.Id} has been rejected.";

            // Create the notification object
            var notification = new BusinessNotification
            {
                AppointmentId = appointment.Id,
                NotificationMessage = notificationMessage,
                IsSent = true,  // Mark as sent
                CreatedAt = DateTime.UtcNow,
                BusinessServiceId = appointment.BusinessServicesID,
                UserId = appointment.UserId
            };

            // Save the notification to the database
            await _context.BusinessNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Fetch the technician assigned to the business service
            var technician = await _context.Technicians
                .FirstOrDefaultAsync(t => t.BusinessServiceId == appointment.BusinessServicesID && t.IsActive);

            // Prepare technician information
            string technicianInfo = technician != null
                ? $"{technician.FullName}, with {technician.Experience} experience."
                : "No technician assigned currently.";

            // Prepare email subject and body
            var userSubject = "Appointment Approval Status";
            var userBody = approvalDto.IsApproved
                ? $"Dear {appointment.user.FirstName}, your appointment for {appointment.businessService.service.Name} has been approved. " +
                  $"The technician assigned to attend to your appointment is {technicianInfo}."
                : $"Dear {appointment.user.FirstName}, your appointment for {appointment.businessService.service.Name} has been rejected. " +
                  $"No technician was assigned for your appointment.";

            // Send email notification to the user
            await _emailService.SendEmailAsync(appointment.user.Email, userSubject, userBody);

            // Return the notification details
            return new NotificationDto
            {
                Message = notificationMessage,
                CreatedAt = DateTime.UtcNow,
                IsSent = true
            };
        }


        // Get pending appointments for a specific business service
        public async Task<List<AppointmentServiceNotificationDto>> GetPendingAppointmentsForBusinessAsync(int businessServiceId)
        {
            return await _context.BusinessNotifications
                .Include(n => n.Appointment)
                .ThenInclude(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Include(n => n.Appointment)
                .ThenInclude(a => a.user)
                .Where(n => n.BusinessServiceId == businessServiceId && n.Appointment.Status == "Pending" && n.Appointment.IsActive)
                .Select(n => new AppointmentServiceNotificationDto
                {
                    NotificationId = n.Id,
                    AppointmentId = n.AppointmentId,
                    UserName = $"{n.Appointment.user.FirstName} {n.Appointment.user.LastName}",
                    AppointmentDate = n.Appointment.AppointmentDate.ToString(),
                    AppointmentTime = n.Appointment.AppointmentTime.ToString(),
                    NotificationMessage = n.NotificationMessage
                })
                .ToListAsync();
        }

        // Get appointment status
        public async Task<string> GetAppointmentStatusAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception($"Appointment with ID {appointmentId} not found.");

            return appointment.Status;
        }

        // Get appointments for a specific business
        public async Task<List<AppointmentServiceNotificationDto>> GetAppointmentForBusinessAsync(int businessId)
        {
            //var bs = await _context.BusinessServices.Where(b1 => b1.BusinessId == businessId).ToListAsync();

            //var notifications = new List<AppointmentServiceNotificationDto>(); // List to hold all notifications

            //foreach (var i in bs)
            //{
            var serviceNotifications = await _context.BusinessNotifications
                .Include(n => n.Appointment)
                .ThenInclude(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Include(n => n.Appointment.user)
                .Where(n => n.BusinessService.BusinessId == businessId && n.Appointment.Status == "Pending" && n.Appointment.IsActive)
                .Select(n => new AppointmentServiceNotificationDto
                {
                    NotificationId = n.Id,
                    AppointmentId = n.AppointmentId,
                    UserName = $"{n.Appointment.user.FirstName} {n.Appointment.user.LastName}",
                    BusinessServiceName = n.BusinessService.service.Name,
                    businessServiceId=n.BusinessServiceId,
                    AppointmentDate = n.Appointment.AppointmentDate.ToString(),
                    AppointmentTime = n.Appointment.AppointmentTime.ToString(),
                    NotificationMessage = n.NotificationMessage
                }).ToListAsync();

            //    notifications.AddRange(serviceNotifications); // Add the service notifications to the list
            //}

            return serviceNotifications; // Return the list of notifications after the loop
        }

        // Get booked services
        public async Task<List<AppointmentServiceNotificationDto>> GetBookedServicesAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Include(a => a.businessService.business)
                .Where(a => a.UserId == userId && a.Status == "Approved" && a.AppointmentDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(a => new AppointmentServiceNotificationDto
                {
                    AppointmentId = a.Id,
                    businessServiceId = a.BusinessServicesID, // Include BusinessServiceId
                    BusinessServiceName = a.businessService.service.Name,
                    AppointmentDate = a.AppointmentDate.ToString(),
                    AppointmentTime = a.AppointmentTime.ToString(),
                    UserName = $"{a.user.FirstName} {a.user.LastName}",
                    NotificationMessage = $"Your appointment with {a.businessService.business.Name} for {a.businessService.service.Name} is confirmed on {a.AppointmentDate.ToShortDateString()} at {a.AppointmentTime.ToShortTimeString()}."
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentServiceNotificationDto>> GetServedServicesAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.businessService)
                .ThenInclude(bs => bs.service)
                .Include(a => a.businessService.business)
                .Where(a => a.UserId == userId && a.Status == "Approved" && a.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
                .Select(a => new AppointmentServiceNotificationDto
                {
                    AppointmentId = a.Id,
                    businessServiceId = a.BusinessServicesID, // Include BusinessServiceId
                    BusinessServiceName = a.businessService.service.Name,
                    AppointmentDate = a.AppointmentDate.ToString(),
                    AppointmentTime = a.AppointmentTime.ToString(),
                    UserName = $"{a.user.FirstName} {a.user.LastName}",
                    NotificationMessage = $"Your appointment with {a.businessService.business.Name} for {a.businessService.service.Name} has been served on {a.AppointmentDate.ToShortDateString()} at {a.AppointmentTime.ToShortTimeString()}."
                })
                .ToListAsync();
        }

    }
}