using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class Printer
{
    public int PrinterId { get; set; }

    public int BranchId { get; set; }

    public string PrinterName { get; set; } = null!;

    public string? PrinterIp { get; set; }

    public bool IsActive { get; set; }

    public string? UserIp { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual BranchMaster Branch { get; set; } = null!;

    public virtual ICollection<PrinterDepartmentMapping> PrinterDepartmentMappings { get; set; } = new List<PrinterDepartmentMapping>();
}
