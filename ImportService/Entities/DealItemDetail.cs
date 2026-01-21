using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("DealItemDetail")]
    public class DealItemDetail
    {
        [Key]
      
        public int DealItemId { get; set; }

        
        public string? DealOptionName { get; set; }

        [Required]
        
        public int ProductDetailId { get; set; }

  
        public int? Quantity { get; set; }

      
        public bool? IsToppingAllowed { get; set; }


        public int? SizeId { get; set; }

        public bool IsActive { get; set; }

        public int? SortOrder { get; set; }
        public int? TempDealItemId { get; set; }

        public int? MaxQuantity { get; set; }
    }
}
