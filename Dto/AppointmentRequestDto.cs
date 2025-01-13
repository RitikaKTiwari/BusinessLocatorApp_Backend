namespace BusinessLocatorApp.Dto
{
    public class AppointmentRequestDto
    {
        public DateTime AppointmentDateTime { get; set; }
        public int UserId { get; set; }
        public int BusinessServiceId { get; set; }
    }
}
