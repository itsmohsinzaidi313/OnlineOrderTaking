using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class OrderModeCompanyMapping
{
    public int OrderModeMappingId { get; set; }

    public int? OrderModeId { get; set; }

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public virtual SetupCompany? Company { get; set; }
}
