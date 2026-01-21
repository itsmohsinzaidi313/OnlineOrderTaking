namespace PointofSaleModels.ServicePayloads
{
    public abstract class ServicePayload
    {
        public ServicePayload() { }
        public ServicePayload(ServicePayload payload)
        {
            CorrelationId = payload.CorrelationId;
            ConnectionId = payload.ConnectionId;
            UserId = payload.UserId;
            RestaurantId = payload.RestaurantId;
            BranchId = payload.BranchId;
            ResponseKey = payload.ResponseKey;
        }
        public string CorrelationId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ResponseKey { get; set; }
        public int RestaurantId { get; set; }
        public int BranchId { get; set; }

        public string RestaurantIdToString => RestaurantId.ToString();

        public string BranchIdToString => BranchId.ToString();
    }
}
