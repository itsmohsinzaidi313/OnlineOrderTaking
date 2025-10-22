using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ExpenseType
{
    public int ExpenseTypeId { get; set; }

    public string ExpenseTypeName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;
}
