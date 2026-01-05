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
            DomainName = payload.DomainName;
            DataRequestType = payload.DataRequestType;
        }
        public string DomainName { get; set; }
        public string DataRequestType { get; set; } = string.Empty;
        public object? DataPayload { get; set; }
    }
}
