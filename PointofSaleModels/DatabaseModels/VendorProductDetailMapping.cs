using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class VendorProductDetailMapping
{
    public int VpdmId { get; set; }

    public int VendorId { get; set; }

    public int ProductDetailId { get; set; }

    public int CompanyId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
}
