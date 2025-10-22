using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductDetailOrderSourcePriceMapping
{
    public int MapId { get; set; }

    public int? OrderSourceId { get; set; }

    public int? ProductDetailId { get; set; }

    public double Price { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public double? FuturePrice { get; set; }

    public double? PreviousPrice { get; set; }

    public virtual SetupMasterDetail? OrderSource { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }
}
