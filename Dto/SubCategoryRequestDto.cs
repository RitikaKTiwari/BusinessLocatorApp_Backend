using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Dto
{
    public class SubCategoryRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string AdminComments { get; set; }
        public int CategoryId { get; set; }
        public int BusinessId { get; set; }
    }
}
