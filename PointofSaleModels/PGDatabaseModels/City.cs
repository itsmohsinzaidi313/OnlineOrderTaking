using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int? CountryId { get; set; }

    public int? ProvinceId { get; set; }
}
