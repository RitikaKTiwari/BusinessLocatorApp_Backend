using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLocatorApp.Dto
{
    public class ServiceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
