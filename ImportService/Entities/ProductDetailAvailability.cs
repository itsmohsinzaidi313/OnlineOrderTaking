using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("product_detail_availability")] // Table name in lowercase with underscores for PostgreSQL
    public class ProductDetailAvailability
    {
        [Key]

        public int ProductDetailAvailableId { get; set; }


        public int? ProductBranchId { get; set; }


        public int? DayId { get; set; }


        public TimeSpan? StartTime { get; set; }


        public TimeSpan? EndTime { get; set; }


        public bool? IsActive { get; set; }

        // ?? Navigation properties
        [ForeignKey(nameof(ProductBranchId))]
        public virtual ProductDetailBranchMapping? ProductBranch { get; set; }

        [ForeignKey(nameof(DayId))]
        public virtual SetupMasterDetail? Day { get; set; }
    }
}
