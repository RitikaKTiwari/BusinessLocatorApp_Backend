using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Models
{
    public class SubCategoryRequest
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

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
