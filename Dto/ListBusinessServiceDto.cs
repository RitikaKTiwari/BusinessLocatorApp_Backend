using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Dto
{
    public class ListBusinessServiceDto
    {
        public int Id { get; set; }
        public string DaysOfWeekFormatted { get; set; }
        public int Price { get; set; }
        public string imageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; } // Business name to display
        public int ServiceId { get; set; }
        public Service Service { get; set; } // Service name to display
    }
}
