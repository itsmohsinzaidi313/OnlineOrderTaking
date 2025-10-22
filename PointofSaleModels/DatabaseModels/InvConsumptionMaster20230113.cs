using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvConsumptionMaster20230113
{
    public int ConsumptionId { get; set; }

    public DateTime? Date { get; set; }

    public int? RecipeId { get; set; }

    public int? BranchId { get; set; }

    public double? TotalQty { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? OrderMasterId { get; set; }

    public int? ProductionId { get; set; }

    public int? EmployeeMealMasterId { get; set; }

    public bool IsRefund { get; set; }

    public double RefundQty { get; set; }
}
