using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("SetupCompanySetting")]
    public class SetupCompanySetting
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int SettingId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? SetupDetailId { get; set; }

        public string? SettingValue { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? BranchId { get; set; }
    }
}
