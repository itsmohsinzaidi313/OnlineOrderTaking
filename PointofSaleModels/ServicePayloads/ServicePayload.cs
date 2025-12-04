namespace PointofSaleModels.ServicePayloads
{
    public abstract class ServicePayload
    {
        public ServicePayload() { }
        public ServicePayload(ServicePayload payload)
        {
            ConnectionId = payload.ConnectionId;
            UserId = payload.UserId;
            ConnectionId = payload.ConnectionId;
            RestaurantId = payload.RestaurantId;
            BranchId = payload.BranchId;
        }
        public string CorrelationId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public int BranchId { get; set; }

        public string RestaurantIdToString => RestaurantId.ToString();

        public string BranchIdToString => BranchId.ToString();
    }
}
