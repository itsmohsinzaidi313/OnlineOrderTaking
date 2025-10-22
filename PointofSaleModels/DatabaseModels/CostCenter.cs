using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class CostCenter
{
    public int CostCenterId { get; set; }

    public string? CostCenterCode { get; set; }

    public string? CostCenterName { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int? ParentId { get; set; }

    public virtual ICollection<CostCenter> InverseParent { get; set; } = new List<CostCenter>();

    public virtual CostCenter? Parent { get; set; }

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();

    public virtual ICollection<TblPocCostCenter> TblPocCostCenters { get; set; } = new List<TblPocCostCenter>();
}
