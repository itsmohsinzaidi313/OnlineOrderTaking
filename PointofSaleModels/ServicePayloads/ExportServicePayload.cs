using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ExportServicePayload : ServicePayload
    {
        public CustomerOrder Order { get; set; }
    }
}