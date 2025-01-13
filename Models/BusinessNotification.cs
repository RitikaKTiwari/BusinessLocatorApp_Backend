using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Models
{
    public class BusinessNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }  // Navigation property to Appointment

        public string NotificationMessage { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }

        public int BusinessServiceId { get; set; }
        public BusinessService BusinessService { get; set; }  // Navigation property to BusinessService

        public int UserId { get; set; }
        public User User { get; set; }  // Navigation property to User
    }
}