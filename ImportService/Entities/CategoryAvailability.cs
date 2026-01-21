using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("category_availability")]
    public class CategoryAvailability
    {
        [Key]
        [Column("CategoryAvailableId", TypeName = "integer")]
        public int CategoryAvailableId { get; set; }

        [Column("CategoryId", TypeName = "integer")]
        public int? CategoryId { get; set; }

        [Column("DayId", TypeName = "integer")]
        public int? DayId { get; set; }

        [Column("StartTime", TypeName = "time")]
        public TimeSpan? StartTime { get; set; }

        [Column("EndTime", TypeName = "time")]
        public TimeSpan? EndTime { get; set; }

        public bool? IsActive { get; set; }

        // 🔗 Navigation Properties
        [ForeignKey(nameof(CategoryId))]
        public virtual ProductCategory? Category { get; set; }

        [ForeignKey(nameof(DayId))]
        public virtual SetupMasterDetail? Day { get; set; }
    }
}
