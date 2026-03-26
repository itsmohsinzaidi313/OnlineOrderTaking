namespace ExportService.Entities
{
    public class SetupMasterDetail
    {
        public int SetupDetailId { get; set; }

        public int? SetupMasterId { get; set; }
        
        public string? SetupDetailName { get; set; }

        public int? ParentId { get; set; }

        public string? Flex1 { get; set; }
        
        public string? Flex2 { get; set; }
        
        public string? Flex3 { get; set; }
        
        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public int? Constant_Value { get; set; }

        public string? Description { get; set; }
    }
}
