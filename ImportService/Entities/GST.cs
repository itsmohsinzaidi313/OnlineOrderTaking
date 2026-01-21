using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("gst")]
    public class GST
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int GSTId { get; set; }

        [Column(TypeName = "DOUBLE PRECISION")]
        public double? GSTPercentage { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CityId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        
        public bool? IsActive { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? GSTName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? PaymentModeId { get; set; }

        // 🔗 Relationships
        [ForeignKey(nameof(CityId))]
        public virtual City? City { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual SetupCompany? Company { get; set; }

        [ForeignKey(nameof(PaymentModeId))]
        public virtual PaymentMode? PaymentMode { get; set; }
    }
}
