namespace BusinessLocatorApp.Dto
{
    public class BusinessRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string ContactNo { get; set; }
        public AddressDto Address { get; set; }
        public string AdminComments { get; set; }
    }
}
