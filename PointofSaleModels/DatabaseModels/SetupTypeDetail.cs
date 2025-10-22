using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupTypeDetail
{
    public int TypeDetailId { get; set; }

    public int? TypeId { get; set; }

    public string? TypeName { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual SetupTypeMaster? Type { get; set; }
}
