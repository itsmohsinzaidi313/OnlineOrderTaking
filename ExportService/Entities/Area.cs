namespace ExportService.Entities
{
    public class Area
    {
        public int AreaId { get; set; }

        public string AreaName { get; set; }

        public int? CityId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int CompanyId { get; set; }
        
        public bool? IsEnable { get; set; } = true;
        
        public bool IsActive { get; set; } = true;
        
    }
}
