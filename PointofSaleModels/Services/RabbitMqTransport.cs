namespace PointofSaleModels.Services
{
    public class RabbitMqTransport
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public object Payload { get; set; } = default!;
    }
}