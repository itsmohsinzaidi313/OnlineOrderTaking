using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ExportServicePayload : ServicePayload
    {
        public string OrderToken { get; set; }
    }
}