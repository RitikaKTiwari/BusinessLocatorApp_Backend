namespace BusinessLocatorApp.Dto
{
    public class ServiceRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string AdminComments { get; set; }
        public int SubCategoryId { get; set; }
        public int BusinessId { get; set; }
    }
}
