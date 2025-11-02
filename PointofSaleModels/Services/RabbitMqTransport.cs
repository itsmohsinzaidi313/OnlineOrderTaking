namespace PointofSaleModels.Services
{
    public class RabbitMqTransport
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public object Payload { get; set; } = default!;
    }
}