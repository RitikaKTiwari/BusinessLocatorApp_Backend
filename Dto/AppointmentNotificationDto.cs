public class AppointmentNotificationDto
{
    public int NotificationId { get; set; }
    public int AppointmentId { get; set; }
    public string ServiceName { get; set; }
    public string UserName { get; set; }
    public string AppointmentDate { get; set; }
    public string AppointmentTime { get; set; }
    public string NotificationMessage { get; set; }
}
