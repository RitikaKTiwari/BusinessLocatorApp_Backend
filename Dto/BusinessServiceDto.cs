namespace BusinessLocatorApp.Dto
{
    public class BusinessServiceDto
    {
        public int BusinessId { get; set; }
        public int ServiceId { get; set; }
        public string DaysOfWeek { get; set; }
        public int Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get;set; }
    }
}
