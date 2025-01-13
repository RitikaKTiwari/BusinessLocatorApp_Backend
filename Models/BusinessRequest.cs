using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLocatorApp.Models
{
    public class BusinessRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public string Description { get; set; }
        public string ContactNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string AdminComments { get; set; }
    }
}