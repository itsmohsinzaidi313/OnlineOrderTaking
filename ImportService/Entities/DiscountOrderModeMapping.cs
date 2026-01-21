using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("DiscountOrderModeMapping")]
    public class DiscountOrderModeMapping
    {
        [Key]
        [Column("DiscountOrderModeMappingId")]
        public int DiscountOrderModeMappingId { get; set; }

        [Required]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Required]
        [Column("OrderModeId")]
        public int OrderModeId { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

        // 🔗 Navigation Property
        [ForeignKey(nameof(DiscountId))]
        public virtual Discount? Discount { get; set; }
    }
}
