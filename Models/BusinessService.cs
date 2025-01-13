using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Models
{
    public class BusinessService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DaysOfWeek { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int BusinessId { get; set; }
        public Business business { get; set; }

        public int ServiceId { get; set; }
        public Service service { get; set; }
    }
}