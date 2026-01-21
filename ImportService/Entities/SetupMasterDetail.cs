using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("setup_master_detail")]
    public class SetupMasterDetail
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int SetupDetailId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? SetupMasterId { get; set; }

        
        public string? SetupDetailName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? ParentId { get; set; }

        
        public string? Flex1 { get; set; }

        
        public string? Flex2 { get; set; }

        
        public string? Flex3 { get; set; }

        
        public bool IsActive { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? Constant_Value { get; set; }


        public string? Description { get; set; }
    }
}
