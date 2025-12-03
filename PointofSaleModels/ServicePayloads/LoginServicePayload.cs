using PointofSaleModels.Application;
using System.ComponentModel.DataAnnotations;

namespace PointofSaleModels.ServicePayloads
{
    public class LoginServicePayload : ServicePayload
    {
        public LoginServicePayload() { }
        public LoginServicePayload(ServicePayload payload) : base(payload) { }
        [Required]
        public Customer Customer { get; set; }
    }
}
