using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("product_detail_branch_mapping")] // PostgreSQL table name convention
    public class ProductDetailBranchMapping
    {
        [Key]
        [Column("ProductDetailBranchMappingId", TypeName = "integer")]
        public int ProductDetailBranchMappingId { get; set; }

        [Column("ProductDetailId", TypeName = "integer")]
        public int? ProductDetailId { get; set; }

        [Column("BranchId", TypeName = "integer")]
        public int? BranchId { get; set; }

        [Column("IsActive", TypeName = "boolean")]
        public bool? IsActive { get; set; }

        [Column("IsDayWise", TypeName = "boolean")]
        public bool IsDayWise { get; set; }

        [Column("IsEnable", TypeName = "boolean")]
        public bool IsEnable { get; set; }

        [Column("RemoteId", TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string? RemoteId { get; set; }

        // ?? Navigation properties
        [ForeignKey(nameof(ProductDetailId))]
        public virtual ProductDetail? ProductDetail { get; set; }

        [ForeignKey(nameof(BranchId))]
        public virtual BranchMaster? Branch { get; set; }
    }
}
