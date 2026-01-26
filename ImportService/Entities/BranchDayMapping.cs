using System.ComponentModel.DataAnnotations;
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
        public BranchDayMapping CopyWith(BranchDayMapping instance)
        {
            return new BranchDayMapping
            {
                BranchDayMappingId = instance.BranchDayMappingId,
                BranchId = instance.BranchId,
                DayId = instance.DayId,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                IsActive = instance.IsActive
            };
        }
    }
}
