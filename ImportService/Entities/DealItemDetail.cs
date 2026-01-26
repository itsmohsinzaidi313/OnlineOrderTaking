using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class DealItemDetail
    {
        public int DealItemId { get; set; }

        public string? DealOptionName { get; set; }

        public int ProductDetailId { get; set; }
  
        public int? Quantity { get; set; }

        public bool? IsToppingAllowed { get; set; }

        public int? SizeId { get; set; }

        public bool IsActive { get; set; }

        public int? SortOrder { get; set; }

        public int? TempDealItemId { get; set; }

        public int? MaxQuantity { get; set; }
        public DealItemDetail CopyWith(DealItemDetail instance)
        {
            return new DealItemDetail
            {
                DealItemId = instance.DealItemId,
                DealOptionName = instance.DealOptionName,
                ProductDetailId = instance.ProductDetailId,
                Quantity = instance.Quantity,
                IsToppingAllowed = instance.IsToppingAllowed,
                SizeId = instance.SizeId,
                IsActive = instance.IsActive,
                SortOrder = instance.SortOrder,
                TempDealItemId = instance.TempDealItemId,
                MaxQuantity = instance.MaxQuantity
            };
        }
    }
}
