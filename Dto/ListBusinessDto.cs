namespace BusinessLocatorApp.Dto
{
    public class ListBusinessDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactNo { get; set; }
/*        public int AddressId { get; set; }*/        
        public string Street { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsPrimary { get; set; }
        public List<AddressDto> Addresses { get; set; }
    }
}
