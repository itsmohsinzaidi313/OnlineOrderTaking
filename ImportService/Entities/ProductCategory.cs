using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("ProductCategory")]
    public class ProductCategory
    {
        [Key]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Column("CategoryName")]
        [StringLength(200)]
        public string? CategoryName { get; set; }

        [Column("CompanyId")]
        public int? CompanyId { get; set; }

        [Column("CategoryBgColor")]
        [StringLength(50)]
        public string? CategoryBgColor { get; set; }

        [Column("CategoryForeColor")]
        [StringLength(50)]
        public string? CategoryForeColor { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Required]
        [Column("IsEnable")]
        public bool IsEnable { get; set; }

        [Required]
        [Column("IsInventoryCategory")]
        public bool IsInventoryCategory { get; set; }

        [Column("DepartmentId")]
        public int? DepartmentId { get; set; }

        [Column("CategoryImage")]
        [StringLength(300)]
        public string? CategoryImage { get; set; }

        [Required]
        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("ProductCardStyle")]
        [StringLength(100)]
        public string? ProductCardStyle { get; set; }

        [Column("CategoryIcon")]
        [StringLength(100)]
        public string? CategoryIcon { get; set; }

        // ?? Navigation Property
        [ForeignKey(nameof(CompanyId))]
        public virtual SetupCompany? Company { get; set; }
    }
}
