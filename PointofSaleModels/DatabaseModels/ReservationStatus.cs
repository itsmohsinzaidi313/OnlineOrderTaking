namespace PointofSaleModels.DatabaseModels;

public partial class ReservationStatus
{
    public int ReservationStatusId { get; set; }

    public string ReservationStatus1 { get; set; } = null!;

    public int CompanyId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool IsInitial { get; set; }

    public bool IsConfirm { get; set; }

    public bool IsClosed { get; set; }

    public bool IsCancelable { get; set; }
}
