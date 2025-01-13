namespace BusinessLocatorApp.Dto
{
    public class BusinessRegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public AddressDto Address { get; set; } // Nested Address DTO to handle address details
        public string Description { get; set; }
        public string ContactNo { get; set; }
    }
}
