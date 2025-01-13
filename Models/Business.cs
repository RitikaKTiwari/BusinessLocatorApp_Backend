using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessLocatorApp.Models
{
    public class Business
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public string Description { get; set; }
        public string ContactNo { get; set; }
        public byte[]? ProfilePic { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int AddressId { get; set; }
        [JsonIgnore]
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
    }
}
