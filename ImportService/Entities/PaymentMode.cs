using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
   
    public class PaymentMode
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int PaymentModeId { get; set; }

        [Column("PaymentMode", TypeName = "varchar(150)")] // <--- FIX IS HERE
        public string? PaymentModeName { get; set; }

        [Column(TypeName = "INTEGER")]
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

        // 🔗 Optional relationship
        [ForeignKey(nameof(CompanyId))]
        public virtual SetupCompany? Company { get; set; }
    }
}
