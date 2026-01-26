using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class ProductSize
    {
        public int SizeId { get; set; }

        public string SizeName { get; set; }

        public int? CompanyId { get; set; }
        
        public bool IsActive { get; set; }
        public ProductSize CopyWith(ProductSize instance)
        {
            return new ProductSize
            {
                SizeId = instance.SizeId,
                SizeName = instance.SizeName,
                CompanyId = instance.CompanyId,
                IsActive = instance.IsActive
            };
        }
    }
}
