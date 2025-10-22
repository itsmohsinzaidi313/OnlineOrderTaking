using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PosRoleActionMapping
{
    public int Id { get; set; }

    public int? PosRoleId { get; set; }

    public int? PosActionId { get; set; }

    public virtual PosAction? PosAction { get; set; }

    public virtual PosRole? PosRole { get; set; }
}
