using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("area")]
    public class Area
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int AreaId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string AreaName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CityId { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? StartTime { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? EndTime { get; set; }

        [Column(TypeName = "INTEGER")]
        public int CompanyId { get; set; }

        
        public bool? IsEnable { get; set; } = true;

        
        public bool IsActive { get; set; } = true;
    }
}
