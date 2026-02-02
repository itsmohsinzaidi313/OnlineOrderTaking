using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("BranchDayMapping")]
    public class BranchDayMapping
    {
        public int BranchDayMappingId { get; set; }

        public int BranchId { get; set; }

        public int DayId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool IsActive { get; set; } = true;
        
    }
}
