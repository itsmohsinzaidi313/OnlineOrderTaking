using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class Template
{
    public int TemplateId { get; set; }

    public string? TemplateName { get; set; }

    public int? TemplateTypeId { get; set; }

    public string? TemplateHtml { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsEnable { get; set; }

    public bool IsSelected { get; set; }

    public int CompanyId { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual SetupMasterDetail? TemplateType { get; set; }
}
