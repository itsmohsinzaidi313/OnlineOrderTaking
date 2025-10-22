using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TblFiscalYear
{
    public int YearId { get; set; }

    public DateTime? YearFrom { get; set; }

    public DateTime? YearTo { get; set; }

    public bool IsLock { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? FiscalYearName { get; set; }

    public bool Isclosed { get; set; }
}
