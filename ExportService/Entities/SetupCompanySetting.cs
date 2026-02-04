namespace ExportService.Entities
{
    public class SetupCompanySetting
    {
        public int SettingId { get; set; }

        public int? SetupDetailId { get; set; }

        public string? SettingValue { get; set; }

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public int? BranchId { get; set; }
        public SetupCompanySetting CopyWith(SetupCompanySetting instance)
        {
            return new SetupCompanySetting
            {
                SettingId = instance.SettingId,
                SetupDetailId = instance.SetupDetailId,
                SettingValue = instance.SettingValue,
                IsActive = instance.IsActive,
                CompanyId = instance.CompanyId,
                BranchId = instance.BranchId
            };
        }
    }
}
