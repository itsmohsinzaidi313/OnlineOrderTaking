using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
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
        public SetupMasterDetail CopyWith(SetupMasterDetail instance)
        {
            return new SetupMasterDetail
            {
                SetupDetailId = instance.SetupDetailId,
                SetupMasterId = instance.SetupMasterId,
                SetupDetailName = instance.SetupDetailName,
                ParentId = instance.ParentId,
                Flex1 = instance.Flex1,
                Flex2 = instance.Flex2,
                Flex3 = instance.Flex3,
                IsActive = instance.IsActive,
                CompanyId = instance.CompanyId,
                Constant_Value = instance.Constant_Value,
                Description = instance.Description
            };
        }
    }
}
