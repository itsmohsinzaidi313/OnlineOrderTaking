using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string OrderStatus1 { get; set; } = null!;

    public int CompanyId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool IsInitial { get; set; }

    public bool IsCancelable { get; set; }

    public bool IsProcess { get; set; }

    public bool IsClosed { get; set; }

    public bool IsUnconfirm { get; set; }

    public bool IsDineIn { get; set; }

    public bool IsTakeAway { get; set; }

    public bool IsDelivery { get; set; }

    public bool IsReady { get; set; }

    public bool IsFinishWaste { get; set; }

    public bool IsRefund { get; set; }

    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();

    public virtual ICollection<OrderStatusModeMapping> OrderStatusModeMappings { get; set; } = new List<OrderStatusModeMapping>();
}
