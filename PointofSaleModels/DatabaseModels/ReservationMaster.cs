using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ReservationMaster
{
    public int ReservationId { get; set; }

    public string? ReservationNumber { get; set; }

    public string? Email { get; set; }

    public string? Cnic { get; set; }

    public string? Event { get; set; }

    public DateTime? ReservationDate { get; set; }

    public int NoOfAdults { get; set; }

    public int NoOfChildrens { get; set; }

    public int TotalCovers { get; set; }

    public double TotalAdvance { get; set; }

    public string? Comments { get; set; }

    public int? ReservationStatusId { get; set; }

    public int? CustomerId { get; set; }

    public int? PhoneId { get; set; }

    public int? CustomerAddressId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public int? BranchId { get; set; }

    public double? AdditionalCharges { get; set; }

    public double? TotalAmount { get; set; }

    public int? TableId { get; set; }

    public string? CommentsManagement { get; set; }

    public int? PaymentModeId { get; set; }

    public int? GuestTypeId { get; set; }

    public int? SlotId { get; set; }

    public bool IsWalkIn { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public DateTime? CutOffTime { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual CustomerAddressDetail? CustomerAddress { get; set; }

    public virtual SetupMasterDetail? GuestType { get; set; }

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual PaymentMode? PaymentMode { get; set; }

    public virtual CustomerPhone? Phone { get; set; }

    public virtual ICollection<ReservationDetail> ReservationDetails { get; set; } = new List<ReservationDetail>();

    public virtual SetupMasterDetail? Slot { get; set; }
}
