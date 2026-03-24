using System.ComponentModel.DataAnnotations;

namespace PointofSaleModels.Entities
{
    public class ProductDetail
    {
        public int ProductDetailId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public int SizeId { get; set; }

        public string? SizeName { get; set; }

        public double Price { get; set; }

        public double TaxPercent { get; set; }

        public bool IsActive { get; set; } = true;

        public bool OnlyForDeal { get; set; }

        public bool IsEnable { get; set; } = true;

        public int? FlavourId { get; set; }

        public string? FlavourName { get; set; }

        public bool IsTopping { get; set; }

        public bool IsSaleable { get; set; } = true;

        public int? ParentProductDetailId { get; set; }

        public double? FuturePrice { get; set; }

        public double? PreviousPrice { get; set; }

        public bool IsDealDirectPunch { get; set; }

        public bool IsOpen { get; set; }

        public bool IsPromotion { get; set; }

        public string? RemoteId { get; set; }

        public bool IsBestSeller { get; set; }

        public double? PriceBeforeDiscount { get; set; }
    }
}
