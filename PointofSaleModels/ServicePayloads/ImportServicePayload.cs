namespace PointofSaleModels.ServicePayloads
{
    public class ImportServicePayload : ServicePayload
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
