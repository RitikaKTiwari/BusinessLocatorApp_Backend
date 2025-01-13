namespace BusinessLocatorApp.Dto
{
    public class ListServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}
