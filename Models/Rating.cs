using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Star { get; set; }
        public string StarDescription { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public User user { get; set; }

        public int BusinessServiceId { get; set; }
        public BusinessService businessService { get; set; }
    }
}
