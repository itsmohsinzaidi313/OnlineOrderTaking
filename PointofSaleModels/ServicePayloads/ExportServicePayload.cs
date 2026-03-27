using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ExportServicePayload : ServicePayload
    {
        public ExportServicePayload(ServicePayload payload) : base(payload)
        {
        }

        public string OrderToken { get; set; }
    }
}