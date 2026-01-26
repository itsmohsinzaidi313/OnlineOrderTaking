namespace ImportService.Entities
{
    public class SetupMaster
    {
        public int SetupMasterId { get; set; }
        public string SetupMasterName { get; set; }
        public bool IsActive { get; set; }
        public SetupMaster CopyWith(SetupMaster instance)
        {
            return new SetupMaster
            {
                SetupMasterId = instance.SetupMasterId,
                SetupMasterName = instance.SetupMasterName,
                IsActive = instance.IsActive
            };
        }
    }

}
