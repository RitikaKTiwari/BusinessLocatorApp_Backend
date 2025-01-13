namespace BusinessLocatorApp.Dto
{
    public class AppointmentServiceNotificationDto
    {
        public int NotificationId { get; set; }
        public int AppointmentId { get; set; }
        public int businessServiceId { get; set; } // Add this property
        public string BusinessServiceName { get; set; }
        public string UserName { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string NotificationMessage { get; set; }
    }
}
