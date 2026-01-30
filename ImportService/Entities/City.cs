namespace ImportService.Entities
{
    public class City
    {
        public int CityId { get; set; }

        public string CityName { get; set; }

        public int? CountryId { get; set; }

        public int? ProvinceId { get; set; }
        
    }
}
