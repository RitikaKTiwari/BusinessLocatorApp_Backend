namespace BusinessLocatorApp.Models
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}