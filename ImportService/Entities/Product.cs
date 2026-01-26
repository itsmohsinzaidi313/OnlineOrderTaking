using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("product")]
    public class Product
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int? ProductCategoryId { get; set; }

        
        public bool IsActive { get; set; }

        
        public bool IsEnable { get; set; }

        
        public bool DisplayInPos { get; set; }

        
        public bool DisplayInWeb { get; set; }

        
        public bool DisplayInOdms { get; set; }

        
        public bool DisplayInMobile { get; set; }

        
        public bool IsDeal { get; set; }

        public string? ProductImage { get; set; }

        public bool IsExpiryMandatory { get; set; }

        public double CommisionValue { get; set; }

        public int? CommisionTypeId { get; set; }

        public string? ProductDescription { get; set; }

        public int SortOrder { get; set; }

        public int? ProductTagId { get; set; }

        public bool IsDealPackage { get; set; }
        public Product CopyWith(Product instance)
        {
            return new Product
            {
                ProductId = instance.ProductId,
                ProductName = instance.ProductName,
                ProductCategoryId = instance.ProductCategoryId,
                IsActive = instance.IsActive,
                IsEnable = instance.IsEnable,
                DisplayInPos = instance.DisplayInPos,
                DisplayInWeb = instance.DisplayInWeb,
                DisplayInOdms = instance.DisplayInOdms,
                DisplayInMobile = instance.DisplayInMobile,
                IsDeal = instance.IsDeal,
                ProductImage = instance.ProductImage,
                IsExpiryMandatory = instance.IsExpiryMandatory,
                CommisionValue = instance.CommisionValue,
                CommisionTypeId = instance.CommisionTypeId,
                ProductDescription = instance.ProductDescription,
                SortOrder = instance.SortOrder,
                ProductTagId = instance.ProductTagId,
                IsDealPackage = instance.IsDealPackage
            };
        }
    }
}
