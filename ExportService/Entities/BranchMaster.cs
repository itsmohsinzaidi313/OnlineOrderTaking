namespace ExportService.Entities
{
    public class BranchMaster
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public int CompanyId { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public bool? IsEnable { get; set; }

        public string? NTNName { get; set; }
        
        public string? NTNNumber { get; set; }

        public TimeSpan? BusinessDayStartTime { get; set; }

        public TimeSpan? BusinessDayEndTime { get; set; }

        public bool IsCallCenter { get; set; } = false;

        public string? BranchAddress { get; set; }

        public string? BranchPhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
