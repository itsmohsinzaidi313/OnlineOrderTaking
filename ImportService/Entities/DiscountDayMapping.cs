using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class DiscountDayMapping
    {
        public int DiscountDayMappingId { get; set; }

        public int DiscountId { get; set; }

        public int DayId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }

        public DiscountDayMapping CopyWith(DiscountDayMapping instance)
        {
            return new DiscountDayMapping
            {
                DiscountDayMappingId = instance.DiscountDayMappingId,
                DiscountId = instance.DiscountId,
                DayId = instance.DayId,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                IsActive = instance.IsActive
            };
        }

    }
}
