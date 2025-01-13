namespace BusinessLocatorApp.Dto
{
    public class AddressDto
    {
        public string Street { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsPrimary { get; set; }
    }
}
