using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("product_detail_order_source_price_mapping")]
    public class ProductDetailOrderSourcePriceMapping
    {
        [Key]

        public int MapId { get; set; }


        public int? OrderSourceId { get; set; }


        public int? ProductDetailId { get; set; }

        public double Price { get; set; }


        public bool IsActive { get; set; }

        public double? FuturePrice { get; set; }

        public double? PreviousPrice { get; set; }

        public int? BranchId { get; set; }

        // 🔗 Navigation properties
        [ForeignKey(nameof(OrderSourceId))]
        public virtual SetupMasterDetail? OrderSource { get; set; }

        [ForeignKey(nameof(ProductDetailId))]
        public virtual ProductDetail? ProductDetail { get; set; }

        [ForeignKey(nameof(BranchId))]
        public virtual BranchMaster? Branch { get; set; }
    }
}


