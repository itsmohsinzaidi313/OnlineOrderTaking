using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TblPocCostCenter
{
    public int CostCenterPocId { get; set; }

    public string? Pocname { get; set; }

    public string? Pocemail { get; set; }

    public string? PoccontactNo { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public int? ModifiedBy { get; set; }

    public string? Ip { get; set; }

    public int CostCenterId { get; set; }

    public virtual CostCenter CostCenter { get; set; } = null!;
}
