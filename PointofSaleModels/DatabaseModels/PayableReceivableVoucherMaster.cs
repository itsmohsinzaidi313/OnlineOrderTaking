using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PayableReceivableVoucherMaster
{
    public int PayableReceivableVoucherMasterId { get; set; }

    public string VoucherNumber { get; set; } = null!;

    public DateTime VoucherDate { get; set; }

    public int? VendorId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int? CustomerId { get; set; }

    public int CompanyId { get; set; }

    public int BranchId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<PayableReceivableVoucherDetail> PayableReceivableVoucherDetails { get; set; } = new List<PayableReceivableVoucherDetail>();

    public virtual InvSetupVendor? Vendor { get; set; }
}
