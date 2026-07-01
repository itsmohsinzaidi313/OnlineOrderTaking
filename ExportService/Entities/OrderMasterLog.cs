namespace ExportService.Entities
{
    public partial class OrderMasterLog
    {
        public int OrderMasterId { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int OrderStatusId { get; set; }
        public DateOnly? OrderDate { get; set; }
        public TimeOnly OrderTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsSyncToPos { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}