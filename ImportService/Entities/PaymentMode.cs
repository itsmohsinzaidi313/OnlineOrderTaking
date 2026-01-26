using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
   
    public class PaymentMode
    {
        public int PaymentModeId { get; set; }

        public string? PaymentModeName { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; }

        public bool IsFoc { get; set; }
        
        public bool? IsPosType { get; set; }
        
        public bool? IsCashType { get; set; }

        public bool IsThirdParty { get; set; }

        public bool InstantDiscount { get; set; }
        
        public bool IsCreditType { get; set; }

        public bool IsPartyAccount { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }
        public PaymentMode CopyWith(PaymentMode instance)
        {
            return new PaymentMode
            {
                PaymentModeId = instance.PaymentModeId,
                PaymentModeName = instance.PaymentModeName,
                CompanyId = instance.CompanyId,
                IsActive = instance.IsActive,
                IsFoc = instance.IsFoc,
                IsPosType = instance.IsPosType,
                IsCashType = instance.IsCashType,
                IsThirdParty = instance.IsThirdParty,
                InstantDiscount = instance.InstantDiscount,
                IsCreditType = instance.IsCreditType,
                IsPartyAccount = instance.IsPartyAccount,
                Description = instance.Description
            };
        }
    }
}
