using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class Restaurant
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ConnectionString { get; set; } = null!;

    public string? DomainName { get; set; }
}
