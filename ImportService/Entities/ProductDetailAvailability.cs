using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class ProductDetailAvailability
    {
        public int ProductDetailAvailableId { get; set; }

        public int? ProductBranchId { get; set; }

        public int? DayId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool? IsActive { get; set; }
        public ProductDetailAvailability CopyWith(ProductDetailAvailability instance)
        {
            return new ProductDetailAvailability
            {
                ProductDetailAvailableId = instance.ProductDetailAvailableId,
                ProductBranchId = instance.ProductBranchId,
                DayId = instance.DayId,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                IsActive = instance.IsActive
            };
        }
    }
}
