using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("branch_detail")]
    public class BranchDetail
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int? BranchDetailId { get; set; }

        [Column(TypeName = "INTEGER")]
        [Required]
        public int BranchId { get; set; }

        [Column(TypeName = "INTEGER")]
        [Required]
        public int AreaId { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? AreaName { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? AreaStartTime { get; set; }

        [Column(TypeName = "TIME")]
        public TimeSpan? AreaEndTime { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? DeliveryTime { get; set; }

        [Column(TypeName = "DOUBLE PRECISION")]
        public double? MinimumOrder { get; set; }

        [Column(TypeName = "DOUBLE PRECISION")]
        public double? DeliveryCharges { get; set; }

        
        public bool? IsEnabled { get; set; }

        [Column(TypeName = "DOUBLE PRECISION")]
        public double? DeliveryChargesWaiveOffLimit { get; set; }

        
        public bool IsActive { get; set; } = true;

        // 🔗 Navigation properties
        [ForeignKey(nameof(BranchId))]
        public virtual BranchMaster? Branch { get; set; }

        [ForeignKey(nameof(AreaId))]
        public virtual Area? Area { get; set; }
    }
}
