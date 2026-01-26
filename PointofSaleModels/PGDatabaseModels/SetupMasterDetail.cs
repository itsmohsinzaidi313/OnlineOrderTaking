using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class SetupMasterDetail
{
    public int SetupDetailId { get; set; }

    public int? SetupMasterId { get; set; }

    public string? SetupDetailName { get; set; }

    public int? ParentId { get; set; }

    public string? Flex1 { get; set; }

    public string? Flex2 { get; set; }

    public string? Flex3 { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public int? ConstantValue { get; set; }

    public string? Description { get; set; }
}
