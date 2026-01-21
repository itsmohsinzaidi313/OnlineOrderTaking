using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("BranchDayMapping")]
    public class BranchDayMapping
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int BranchDayMappingId { get; set; }

        [Column(TypeName = "INTEGER")]
        [Required]
        public int BranchId { get; set; }

        [Column(TypeName = "INTEGER")]
        [Required]
        public int DayId { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? StartTime { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
