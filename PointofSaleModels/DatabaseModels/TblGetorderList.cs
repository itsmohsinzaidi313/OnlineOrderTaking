using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TblGetorderList
{
    public long? RowNum { get; set; }

    public int OrderMasterId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string? OrderDate { get; set; }

    public int BranchId { get; set; }

    public DateTime? AdvanceOrderDate { get; set; }

    public bool IsAdvanceOrder { get; set; }

    public int? AreaId { get; set; }

    public string? AlternateNumber { get; set; }

    public int CompanyId { get; set; }

    public int? TerminalDetailId { get; set; }

    public int? CustomerId { get; set; }

    public double DeliveryCharges { get; set; }

    public string? DeliveryTime { get; set; }

    public double? DiscountAmount { get; set; }

    public int? DiscountId { get; set; }

    public double DiscountPercent { get; set; }

    public double? Gstamount { get; set; }

    public int? Gstid { get; set; }

    public double Gstpercent { get; set; }

    public int? OrderSourceId { get; set; }

    public int OrderStatusId { get; set; }

    public TimeOnly OrderTime { get; set; }

    public string? SpecialInstruction { get; set; }

    public double? TotalAmountWithGst { get; set; }

    public double? TotalAmountWithoutGst { get; set; }

    public int? BillPrintCount { get; set; }

    public int? CareOfId { get; set; }

    public long? Clinumber { get; set; }

    public int? Cover { get; set; }

    public int CreatedBy { get; set; }

    public int? CustomerAddressId { get; set; }

    public int? FinishWasteReasonId { get; set; }

    public string? FinishWasteRemarks { get; set; }

    public bool IsActive { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? OrderCancelReasonId { get; set; }

    public int? OrderModeId { get; set; }

    public string? OrderSourceValue { get; set; }

    public int? PaymentTypeId { get; set; }

    public int? PhoneId { get; set; }

    public int? PreviousOrderMasterId { get; set; }

    public string? Remarks { get; set; }

    public int? RiderId { get; set; }

    public string? RiderName { get; set; }

    public int? ShiftDetailId { get; set; }

    public int? WaiterId { get; set; }

    public string? UserIp { get; set; }

    public string BranchName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? CustomerName { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string? OrderDateTime { get; set; }

    public string? OrderDeliveryDateTime { get; set; }

    public bool? IsPaid { get; set; }

    public string? OrderMode { get; set; }

    public string? OrderSource { get; set; }

    public double AdditionalServiceCharges { get; set; }

    public string Address { get; set; } = null!;

    public string? CompleteAddress { get; set; }

    public bool? IsFinishWaste { get; set; }

    public bool? IsRefund { get; set; }
}
