using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Models
{
    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string AdminComments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
