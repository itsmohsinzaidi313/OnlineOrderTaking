using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvRecipeMaster
{
    public int RecipeId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? SubRecipeItemId { get; set; }

    public string? ItemCode { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<InvConsumptionMaster> InvConsumptionMasters { get; set; } = new List<InvConsumptionMaster>();

    public virtual ICollection<InvRecipeDetail> InvRecipeDetails { get; set; } = new List<InvRecipeDetail>();

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual ProductDetail? SubRecipeItem { get; set; }
}
