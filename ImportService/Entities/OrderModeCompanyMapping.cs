using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("order_mode_company_mapping")] // PostgreSQL table name in lowercase with underscores
    public class OrderModeCompanyMapping
    {
        [Key]
        [Column("OrderModeMappingId", TypeName = "INTEGER")]
        public int OrderModeMappingId { get; set; }

        [Column("OrderModeId", TypeName = "INTEGER")]
        public int? OrderModeId { get; set; }

        [Column("CompanyId", TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        [Column("IsActive", TypeName = "BOOLEAN")]
        public bool IsActive { get; set; }

        // Navigation property (optional)
        [ForeignKey(nameof(CompanyId))]
        public virtual SetupCompany? Company { get; set; }
    }
}
