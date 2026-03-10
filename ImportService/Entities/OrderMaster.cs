namespace ImportService.Entities
{
    public partial class OrderMaster
    {
        public int OrderMasterId { get; set; }

        public int CompanyId { get; set; }

        public string OrderNumber { get; set; }

        public string OrderToken { get; set; }
        public int CreatedBy { get; set; }

        public int BranchId { get; set; }

        public int? AreaId { get; set; }

        public int? CustomerId { get; set; }

        public int? PhoneId { get; set; }

        public int? CustomerAddressId { get; set; }

        public int? RiderId { get; set; }

        public int OrderStatusId { get; set; }

        public bool IsAdvanceOrder { get; set; }

        public string? SpecialInstruction { get; set; }

        public DateTime? OrderDate { get; set; }

        public TimeOnly OrderTime { get; set; }

        public double? TotalAmountWithoutGst { get; set; }

        public int? Gstid { get; set; }

        public double? TotalAmountWithGst { get; set; }

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

        public double Tip { get; set; }

        public double ChangeAmount { get; set; }

        public string? VoucherCode { get; set; }

        public int? VoucherId { get; set; }

        public double? VoucherAmount { get; set; }

        public string? CareOfName { get; set; }

        public string? BankName { get; set; }

        public string? CardNumber { get; set; }

        public int? PartyPhoneId { get; set; }

        public int? PartyCustomerId { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = [];
    }
}
