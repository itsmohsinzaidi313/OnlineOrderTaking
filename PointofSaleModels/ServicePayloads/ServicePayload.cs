namespace PointofSaleModels.ServicePayloads
{
    public abstract class ServicePayload
    {
        public ServicePayload()
        {
        }

        public ServicePayload(ServicePayload payload)
        {
            ClientId = payload.ClientId;
            DomainName = payload.DomainName;
            ConnectionId = payload.ConnectionId;
            CorrelationId = payload.CorrelationId;
            ResponseKey = payload.ResponseKey;
            BranchId = payload.BranchId;
            SignalRMethod = payload.SignalRMethod;
        }

        public string ClientId { get; set; } = string.Empty;
        public string DomainName { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public string ResponseKey { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public string SignalRMethod { get; set; } = string.Empty;
        public string? DataCode { get; set; }
    }
}
