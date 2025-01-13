namespace BusinessLocatorApp.Dto
{
    public class ListSubcategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
