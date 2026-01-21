using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("deal_description")]
    public class DealDescription
    {
        [Key]
        public int DealDescId { get; set; }

        public int? DealItemId { get; set; }

        public int? ProductDetailId { get; set; }

        public bool? IsActive { get; set; }

        public int? SortOrder { get; set; }

        public double? Price { get; set; }

        // ?? Navigation properties
        [ForeignKey(nameof(DealItemId))]
        public virtual DealItemDetail? DealItemDetail { get; set; }

        [ForeignKey(nameof(ProductDetailId))]
        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
