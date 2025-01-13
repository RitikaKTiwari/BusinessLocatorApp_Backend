namespace BusinessLocatorApp.Dto
{
    public class FavouriteDto
    {
        public int Id { get; set; }
        public int BusinessServiceId { get; set; }
        public string BusinessServiceName { get; set; }
        public string BusinessServiceDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
