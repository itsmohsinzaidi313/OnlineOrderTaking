namespace ImportService.Entities
{
    public class OrderStatusLog
    {
        public int OrderStatusLogId { get; set; }
        public int OrderMasterId { get; set; }
        public int OrderStatusId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
