namespace PointofSaleModels.DatabaseModels;

public partial class Waiter
{
    public int WaiterId { get; set; }

    public string? WaiterName { get; set; }

    public string? Address { get; set; }

    public string? Contact1 { get; set; }

    public string? Contact2 { get; set; }

    public string? Cnic { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();
}
