using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ExportServicePayload : ServicePayload
    {
        public ExportServicePayload()
        {

        }

        public ExportServicePayload(ServicePayload payload) : base(payload)
        {
        }

        public ExportServicePayload(ExportServicePayload payload) : base(payload)
        {
        }
        public string ExportType { get; set; }
        public string OrderNumber { get; set; }
    }
}