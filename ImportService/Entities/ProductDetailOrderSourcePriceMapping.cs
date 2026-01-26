using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class ProductDetailOrderSourcePriceMapping
    {
        public int MapId { get; set; }

        public int? OrderSourceId { get; set; }

        public int? ProductDetailId { get; set; }

        public double Price { get; set; }

        public bool IsActive { get; set; }

        public double? FuturePrice { get; set; }

        public double? PreviousPrice { get; set; }

        public int? BranchId { get; set; }
    
        public ProductDetailOrderSourcePriceMapping CopyWith(ProductDetailOrderSourcePriceMapping instance)
        {
            return new ProductDetailOrderSourcePriceMapping
            {
                MapId = instance.MapId,
                OrderSourceId = instance.OrderSourceId,
                ProductDetailId = instance.ProductDetailId,
                Price = instance.Price,
                IsActive = instance.IsActive,
                FuturePrice = instance.FuturePrice,
                PreviousPrice = instance.PreviousPrice,
                BranchId = instance.BranchId
            };
        }
    }
}


