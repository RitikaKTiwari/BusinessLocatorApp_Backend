using MimeKit.Encodings;

namespace BusinessLocatorApp.Dto
{
    public class AppointmentApprovalDto
    {
        public int AppointmentId { get; set; }
        public bool IsApproved { get; set; }  // true if approved, false if rejected
        public int? TechnicianId { get; set; }
    }

}
