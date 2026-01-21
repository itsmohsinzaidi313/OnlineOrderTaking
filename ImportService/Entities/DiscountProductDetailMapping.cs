using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("DiscountProductDetailMapping")]
    public class DiscountProductDetailMapping
    {
        [Key]
        [Column("DiscountProductDetailMappingId")]
        public int DiscountProductDetailMappingId { get; set; }

        [Required]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Required]
        [Column("ProductDetailId")]
        public int ProductDetailId { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

        // 🔗 Navigation Properties
        [ForeignKey(nameof(DiscountId))]
        public virtual Discount? Discount { get; set; }

        [ForeignKey(nameof(ProductDetailId))]
        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
