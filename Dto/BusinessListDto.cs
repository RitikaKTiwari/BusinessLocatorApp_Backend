namespace BusinessLocatorApp.Dto
{
    public class BusinessListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfilePic { get; set; }
        public AddressDto Address { get; set; } // Nested Address DTO to handle address details
        public string Description { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
