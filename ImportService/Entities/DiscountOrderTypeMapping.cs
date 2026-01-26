using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("DiscountOrderTypeMapping")]
    public class DiscountOrderTypeMapping
    {
        [Key]
        [Column("DiscountOrderTypeMappingId")]
        public int DiscountOrderTypeMappingId { get; set; }

        [Required]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Required]
        [Column("OrderTypeId")]
        public int OrderTypeId { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
