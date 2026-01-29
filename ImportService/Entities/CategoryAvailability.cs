namespace ImportService.Entities
{
    public class CategoryAvailability
    {
        public int CategoryAvailableId { get; set; }

        public int? CategoryId { get; set; }

        public int? DayId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool? IsActive { get; set; }
        public CategoryAvailability CopyWith(CategoryAvailability instance)
        {
            return new CategoryAvailability
            {
                CategoryAvailableId = instance.CategoryAvailableId,
                CategoryId = instance.CategoryId,
                DayId = instance.DayId,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                IsActive = instance.IsActive
            };
        }
    }
}
