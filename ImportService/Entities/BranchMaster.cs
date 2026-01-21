using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("branch_master")]
    public class BranchMaster
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int BranchId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string BranchName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int CompanyId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CityId { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? CityName { get; set; }


        public bool? IsEnable { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? NTNName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? NTNNumber { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? BusinessDayStartTime { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? BusinessDayEndTime { get; set; }


        public bool IsCallCenter { get; set; } = false;

        [Column(TypeName = "varchar(300)")]
        public string? BranchAddress { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? BranchPhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
