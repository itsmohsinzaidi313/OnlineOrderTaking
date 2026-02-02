namespace ExportService.Entities
{
    public class City
    {
        public int CityId { get; set; }

        public string CityName { get; set; }

        public int? CountryId { get; set; }

        public int? ProvinceId { get; set; }
        public City CopyWith(City instance)
        {
            return new City
            {
                CityId = instance.CityId,
                CityName = instance.CityName,
                CountryId = instance.CountryId,
                ProvinceId = instance.ProvinceId
            };
        }
    }
}
