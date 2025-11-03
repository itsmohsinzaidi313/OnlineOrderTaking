namespace PointofSaleModels.Services
{
    public class RabbitMqTransport
    {
        public string ConnectionId { get; set; } = string.Empty;
        // Optional user id (from JWT) - used to route responses to a user across connections
        public string UserId { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public object Payload { get; set; } = default!;
    }
}