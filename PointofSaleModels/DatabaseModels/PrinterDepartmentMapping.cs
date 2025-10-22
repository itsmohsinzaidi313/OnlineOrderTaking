using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PrinterDepartmentMapping
{
    public int PrinterMappingId { get; set; }

    public int? DepartmentId { get; set; }

    public int? OrderModeId { get; set; }

    public int? PrinterId { get; set; }

    public bool IsActive { get; set; }

    public string? UserIp { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Department? Department { get; set; }

    public virtual SetupMasterDetail? OrderMode { get; set; }

    public virtual Printer? Printer { get; set; }
}
