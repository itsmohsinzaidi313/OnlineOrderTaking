using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.ServicePayloads
{
    public class DataServicePayload : ServicePayload
    {
        public DataServicePayload() : base()
        {
        }

        public DataServicePayload(DataServicePayload payload) : base(payload)
        {
            RequestType = payload.RequestType;
        }
        public string DataRequestType { get; set; } = string.Empty;
        public string? OrderToken { get; set; }
        public object? DataPayload { get; set; }
        public bool Success { get; set; }
        public int? OrderUserId { get; set; }
    }
}
