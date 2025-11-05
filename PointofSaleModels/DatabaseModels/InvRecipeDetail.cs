namespace PointofSaleModels.DatabaseModels;

public partial class InvRecipeDetail
{
    public int RecipeDetailId { get; set; }

    public int? RecipeId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? ConsumeQty { get; set; }

    public int? ConsumeUnitId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? OrderModeId { get; set; }

    public bool EliminateInDeal { get; set; }

    public virtual InvSetupUnit? ConsumeUnit { get; set; }

    public virtual SetupMasterDetail? OrderMode { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvRecipeMaster? Recipe { get; set; }
}
