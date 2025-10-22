using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? CompanyId { get; set; }

    public string? CategoryBgColor { get; set; }

    public string? CategoryForeColor { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool IsEnable { get; set; }

    public bool IsInventoryCategory { get; set; }

    public int? DepartmentId { get; set; }

    public string? CategoryImage { get; set; }

    public int SortOrder { get; set; }

    public virtual SetupCompany? Company { get; set; }
}
