namespace PointofSaleModels.DatabaseModels;

public partial class SalesReturnDetail
{
    public int SalesReturnDetailId { get; set; }

    public int SalesReturnId { get; set; }

    public int ReturnQuantity { get; set; }

    public double ReturnAmount { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    public int UserId { get; set; }

    public string? UserIp { get; set; }

    public int ProductDetailId { get; set; }

    public int? BatchId { get; set; }

    public int? OrderDetailId { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual OrderDetail? OrderDetail { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual SalesReturnMaster SalesReturn { get; set; } = null!;
}
