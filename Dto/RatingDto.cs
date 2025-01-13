namespace BusinessLocatorApp.Dto
{
    public class RatingDto
    {
        public int Star { get; set; }
        public string StarDescription { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int BusinessServiceId { get; set; }
    }
}
