using System.ComponentModel.DataAnnotations.Schema;

namespace ExportService.Entities
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
    }
}
