using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("DiscountBranchMapping")]
    public class DiscountBranchMapping
    {
        [Key]
        [Column("DiscountBranchMappingId")]
        public int DiscountBranchMappingId { get; set; }

        [Required]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Required]
        [Column("BranchId")]
        public int BranchId { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
