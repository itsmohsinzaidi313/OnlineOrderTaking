namespace PointofSaleModels.DatabaseModels;

public partial class OrderMaster
{
    public int OrderMasterId { get; set; }

    public int CompanyId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int BranchId { get; set; }

    public int? AreaId { get; set; }

    public int? CustomerId { get; set; }

    public int? PhoneId { get; set; }

    public int? CustomerAddressId { get; set; }

    public int? RiderId { get; set; }

    public int OrderStatusId { get; set; }

    public bool IsAdvanceOrder { get; set; }

    public string? SpecialInstruction { get; set; }

    public DateTime OrderDate { get; set; }

    public TimeOnly OrderTime { get; set; }

    public double? TotalAmountWithoutGst { get; set; }

    public int? Gstid { get; set; }

    public double? TotalAmountWithGst { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public string? AlternateNumber { get; set; }

    public DateTime? AdvanceOrderDate { get; set; }

    public int? DeliveryTime { get; set; }

    public long? Clinumber { get; set; }

    public int? OrderSourceId { get; set; }

    public string? OrderSourceValue { get; set; }

    public int? DiscountId { get; set; }

    public double? DeliveryCharges { get; set; }

    public int? OrderCancelReasonId { get; set; }

    public int? WaiterId { get; set; }

    public int? ShiftDetailId { get; set; }

    public int? TerminalDetailId { get; set; }

    public int? OrderModeId { get; set; }

    public int? Cover { get; set; }

    public int? PaymentTypeId { get; set; }

    public double? DiscountAmount { get; set; }

    public double? Gstamount { get; set; }

    public int? CareOfId { get; set; }

    public int? BillPrintCount { get; set; }

    public int? PreviousOrderMasterId { get; set; }

    public string? Remarks { get; set; }

    public double DiscountPercent { get; set; }

    public double Gstpercent { get; set; }

    public string? FinishWasteRemarks { get; set; }

    public int? FinishWasteReasonId { get; set; }

    public int? TableId { get; set; }

    public string? EmailAddress { get; set; }

    public string? OrderJson { get; set; }

    public string? SrbInvoiceId { get; set; }

    public string? FbrInvoiceId { get; set; }

    public int? ReservationId { get; set; }

    public double? TotalAdvance { get; set; }

    public bool IsSyncToPos { get; set; }

    public int TerminalId { get; set; }

    public virtual Area? Area { get; set; }

    public virtual BranchMaster Branch { get; set; } = null!;

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ICollection<ComplainMaster> ComplainMasters { get; set; } = new List<ComplainMaster>();

    public virtual Customer? Customer { get; set; }

    public virtual CustomerAddressDetail? CustomerAddress { get; set; }

    public virtual SetupMasterDetail? FinishWasteReason { get; set; }

    public virtual Gst? Gst { get; set; }

    public virtual ICollection<LoyaltyCardBalance> LoyaltyCardBalances { get; set; } = new List<LoyaltyCardBalance>();

    public virtual SetupMasterDetail? OrderCancelReason { get; set; }

    public virtual ICollection<OrderDetailLog> OrderDetailLogs { get; set; } = new List<OrderDetailLog>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderExtraCharge> OrderExtraCharges { get; set; } = new List<OrderExtraCharge>();

    public virtual SetupMasterDetail? OrderMode { get; set; }

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual SetupMasterDetail? OrderSource { get; set; }

    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();

    public virtual ICollection<PayableReceivableVoucherDetail> PayableReceivableVoucherDetails { get; set; } = new List<PayableReceivableVoucherDetail>();

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();

    public virtual CustomerPhone? Phone { get; set; }

    public virtual ReservationMaster? Reservation { get; set; }

    public virtual Rider? Rider { get; set; }

    public virtual ICollection<SalesReturnMaster> SalesReturnMasters { get; set; } = new List<SalesReturnMaster>();

    public virtual ShiftDetail? ShiftDetail { get; set; }

    public virtual Table? Table { get; set; }

    public virtual ICollection<TableMergeDetail> TableMergeDetails { get; set; } = new List<TableMergeDetail>();

    public virtual ICollection<TableMerge> TableMerges { get; set; } = new List<TableMerge>();

    public virtual Waiter? Waiter { get; set; }
}
