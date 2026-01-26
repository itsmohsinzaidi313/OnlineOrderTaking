using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("DiscountDayMapping")]
    public class DiscountDayMapping
    {
        [Key]
        [Column("DiscountDayMappingId")]
        public int DiscountDayMappingId { get; set; }

        [Required]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Required]
        [Column("DayId")]
        public int DayId { get; set; }

        [Required]
        [Column("StartTime", TypeName = "time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("EndTime", TypeName = "time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

    }
}
