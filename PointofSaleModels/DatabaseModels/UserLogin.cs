namespace PointofSaleModels.DatabaseModels;

public partial class UserLogin
{
    public int UserId { get; set; }

    public int? CompanyId { get; set; }

    public string? Name { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public bool IsEnable { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public string? EmailAddress { get; set; }

    public int? PosRoleId { get; set; }

    public virtual ICollection<ComplainDetail> ComplainDetails { get; set; } = new List<ComplainDetail>();

    public virtual ICollection<DiscountAreaMapping> DiscountAreaMappingCreatedByNavigations { get; set; } = new List<DiscountAreaMapping>();

    public virtual ICollection<DiscountAreaMapping> DiscountAreaMappingModifiedByNavigations { get; set; } = new List<DiscountAreaMapping>();

    public virtual ICollection<DiscountDayMapping> DiscountDayMappingCreatedByNavigations { get; set; } = new List<DiscountDayMapping>();

    public virtual ICollection<DiscountDayMapping> DiscountDayMappingModifiedByNavigations { get; set; } = new List<DiscountDayMapping>();

    public virtual ICollection<DiscountOrderModeMapping> DiscountOrderModeMappingCreatedByNavigations { get; set; } = new List<DiscountOrderModeMapping>();

    public virtual ICollection<DiscountOrderModeMapping> DiscountOrderModeMappingModifiedByNavigations { get; set; } = new List<DiscountOrderModeMapping>();

    public virtual ICollection<DiscountOrderTypeMapping> DiscountOrderTypeMappingCreatedByNavigations { get; set; } = new List<DiscountOrderTypeMapping>();

    public virtual ICollection<DiscountOrderTypeMapping> DiscountOrderTypeMappingModifiedByNavigations { get; set; } = new List<DiscountOrderTypeMapping>();

    public virtual ICollection<DiscountProductDetailMapping> DiscountProductDetailMappingCreatedByNavigations { get; set; } = new List<DiscountProductDetailMapping>();

    public virtual ICollection<DiscountProductDetailMapping> DiscountProductDetailMappingModifiedByNavigations { get; set; } = new List<DiscountProductDetailMapping>();

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetailCreatedByNavigations { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetailModifiedByNavigations { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvAdjustmentMaster> InvAdjustmentMasterCreatedByNavigations { get; set; } = new List<InvAdjustmentMaster>();

    public virtual ICollection<InvAdjustmentMaster> InvAdjustmentMasterModifiedByNavigations { get; set; } = new List<InvAdjustmentMaster>();

    public virtual ICollection<InvPomaster> InvPomasterCreatedByNavigations { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvPomaster> InvPomasterModifiedByNavigations { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvPomaster> InvPomasterUsers { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetailCreatedByNavigations { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetailModifiedByNavigations { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvReceivingMaster> InvReceivingMasterCreatedByNavigations { get; set; } = new List<InvReceivingMaster>();

    public virtual ICollection<InvReceivingMaster> InvReceivingMasterModifiedByNavigations { get; set; } = new List<InvReceivingMaster>();

    public virtual ICollection<InvReceivingMaster> InvReceivingMasterUsers { get; set; } = new List<InvReceivingMaster>();

    public virtual ICollection<InvRequisitionMaster> InvRequisitionMasterCreatedByNavigations { get; set; } = new List<InvRequisitionMaster>();

    public virtual ICollection<InvRequisitionMaster> InvRequisitionMasterModifiedByNavigations { get; set; } = new List<InvRequisitionMaster>();

    public virtual ICollection<InvRequisitionMaster> InvRequisitionMasterUsers { get; set; } = new List<InvRequisitionMaster>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetailCreatedByNavigations { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetailModifiedByNavigations { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvSubRecipeProductionMaster> InvSubRecipeProductionMasterCreatedByNavigations { get; set; } = new List<InvSubRecipeProductionMaster>();

    public virtual ICollection<InvSubRecipeProductionMaster> InvSubRecipeProductionMasterModifiedByNavigations { get; set; } = new List<InvSubRecipeProductionMaster>();

    public virtual ICollection<InvSubRecipeProductionMaster> InvSubRecipeProductionMasterUsers { get; set; } = new List<InvSubRecipeProductionMaster>();

    public virtual ICollection<InvWastageMaster> InvWastageMasterCreatedByNavigations { get; set; } = new List<InvWastageMaster>();

    public virtual ICollection<InvWastageMaster> InvWastageMasterModifiedByNavigations { get; set; } = new List<InvWastageMaster>();

    public virtual ICollection<InvWastageMaster> InvWastageMasterUsers { get; set; } = new List<InvWastageMaster>();

    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();

    public virtual PosRole? PosRole { get; set; }

    public virtual ICollection<SetupRoleMenuItemFeatureMapping> SetupRoleMenuItemFeatureMappingCreatedByNavigations { get; set; } = new List<SetupRoleMenuItemFeatureMapping>();

    public virtual ICollection<SetupRoleMenuItemFeatureMapping> SetupRoleMenuItemFeatureMappingModifiedByNavigations { get; set; } = new List<SetupRoleMenuItemFeatureMapping>();

    public virtual ICollection<UserBranchMapping> UserBranchMappings { get; set; } = new List<UserBranchMapping>();
}
