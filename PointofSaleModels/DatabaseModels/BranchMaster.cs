using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class BranchMaster
{
    public int BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public int CompanyId { get; set; }

    public int? CityId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CreatedDateInt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedDateInt { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool? IsEnable { get; set; }

    public string? Ntnname { get; set; }

    public string? Ntnnumber { get; set; }

    public TimeOnly? BusinessDayStartTime { get; set; }

    public TimeOnly? BusinessDayEndTime { get; set; }

    public string? Code { get; set; }

    public bool IsCallCenter { get; set; }

    public bool IsWarehouse { get; set; }

    public string? BranchAddress { get; set; }

    public string? BranchPhoneNumber { get; set; }

    public virtual ICollection<BranchDetail> BranchDetailAlternateBranch1Navigations { get; set; } = new List<BranchDetail>();

    public virtual ICollection<BranchDetail> BranchDetailAlternateBranch2Navigations { get; set; } = new List<BranchDetail>();

    public virtual ICollection<BranchDetail> BranchDetailAlternateBranch3Navigations { get; set; } = new List<BranchDetail>();

    public virtual ICollection<BranchDetail> BranchDetailBranches { get; set; } = new List<BranchDetail>();

    public virtual ICollection<BranchOrderSourceMapping> BranchOrderSourceMappings { get; set; } = new List<BranchOrderSourceMapping>();

    public virtual ICollection<BusinessDay> BusinessDays { get; set; } = new List<BusinessDay>();

    public virtual City? City { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ICollection<DiscountBranchMapping> DiscountBranchMappings { get; set; } = new List<DiscountBranchMapping>();

    public virtual ICollection<InvAdjustmentMaster> InvAdjustmentMasters { get; set; } = new List<InvAdjustmentMaster>();

    public virtual ICollection<InvClosingMaster> InvClosingMasters { get; set; } = new List<InvClosingMaster>();

    public virtual ICollection<InvConsumptionMaster> InvConsumptionMasters { get; set; } = new List<InvConsumptionMaster>();

    public virtual ICollection<InvEmployeeMealMaster> InvEmployeeMealMasters { get; set; } = new List<InvEmployeeMealMaster>();

    public virtual ICollection<InvGoodReceivingMaster> InvGoodReceivingMasters { get; set; } = new List<InvGoodReceivingMaster>();

    public virtual ICollection<InvGoodReceivingReturnMaster> InvGoodReceivingReturnMasters { get; set; } = new List<InvGoodReceivingReturnMaster>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvIssuanceMaster> InvIssuanceMasters { get; set; } = new List<InvIssuanceMaster>();

    public virtual ICollection<InvPomaster> InvPomasters { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvPurchaseInvoiceMaster> InvPurchaseInvoiceMasters { get; set; } = new List<InvPurchaseInvoiceMaster>();

    public virtual ICollection<InvReceivingMaster> InvReceivingMasters { get; set; } = new List<InvReceivingMaster>();

    public virtual ICollection<InvRequisitionMaster> InvRequisitionMasters { get; set; } = new List<InvRequisitionMaster>();

    public virtual ICollection<InvSubRecipeProductionMaster> InvSubRecipeProductionMasters { get; set; } = new List<InvSubRecipeProductionMaster>();

    public virtual ICollection<InvTransferMaster> InvTransferMasterBranchIdfromNavigations { get; set; } = new List<InvTransferMaster>();

    public virtual ICollection<InvTransferMaster> InvTransferMasterBranchIdtoNavigations { get; set; } = new List<InvTransferMaster>();

    public virtual ICollection<InvWastageMaster> InvWastageMasters { get; set; } = new List<InvWastageMaster>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual ICollection<Printer> Printers { get; set; } = new List<Printer>();

    public virtual ICollection<ProductDetailBranchMapping> ProductDetailBranchMappings { get; set; } = new List<ProductDetailBranchMapping>();

    public virtual ICollection<ReservationMaster> ReservationMasters { get; set; } = new List<ReservationMaster>();

    public virtual ICollection<Rider> Riders { get; set; } = new List<Rider>();

    public virtual ICollection<SalesReturnMaster> SalesReturnMasters { get; set; } = new List<SalesReturnMaster>();

    public virtual ICollection<SetupCompanySetting> SetupCompanySettings { get; set; } = new List<SetupCompanySetting>();

    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

    public virtual ICollection<TableMerge> TableMerges { get; set; } = new List<TableMerge>();

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<TempOrderMaster> TempOrderMasters { get; set; } = new List<TempOrderMaster>();

    public virtual ICollection<TerminalDetail> TerminalDetails { get; set; } = new List<TerminalDetail>();

    public virtual ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();

    public virtual ICollection<UserBranchMapping> UserBranchMappings { get; set; } = new List<UserBranchMapping>();

    public virtual ICollection<Waiter> Waiters { get; set; } = new List<Waiter>();
}
