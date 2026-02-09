namespace PointofSaleModels.ServicePayloads
{
    public class DataServicePayload : ServicePayload
    {
        public const string DATA_REQUEST_TYPE_MENU = "MENU";
        public const string DATA_REQUEST_TYPE_SETTINGS = "SETTINGS";
        public const string DATA_REQUEST_TYPE_BRANCHES = "BRANCHES";
        public const string DATA_REQUEST_TYPE_AREAS = "AREAS";

        public DataServicePayload() : base() { }
        public DataServicePayload(DataServicePayload payload) : base(payload)
        {
            DataRequestType = payload.DataRequestType;
        }
        public string DataRequestType { get; set; } = string.Empty;
        public object? DataPayload { get; set; }
        public bool Success { get; set; }
        public int? OrderUserId { get; set; }
    }
}
