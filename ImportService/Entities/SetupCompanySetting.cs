namespace ImportService.Entities
{
    public class SetupCompanySetting
    {
        public int SettingId { get; set; }

        public int? SetupDetailId { get; set; }

        public string? SettingValue { get; set; }

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public int? BranchId { get; set; }
        
    }
}
