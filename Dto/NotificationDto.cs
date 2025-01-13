namespace BusinessLocatorApp.Dto
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSent { get; set; }
    }

}
