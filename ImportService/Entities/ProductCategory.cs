using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    public class ProductCategory
    {
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? CompanyId { get; set; }

        public string? CategoryBgColor { get; set; }

        public string? CategoryForeColor { get; set; }

        public bool IsActive { get; set; }

        public bool IsEnable { get; set; }

        public bool IsInventoryCategory { get; set; }

        public int? DepartmentId { get; set; }

        public string? CategoryImage { get; set; }

        public int SortOrder { get; set; }

        public string? ProductCardStyle { get; set; }

        public string? CategoryIcon { get; set; }

        public ProductCategory CopyWith(ProductCategory instance)
        {
            return new ProductCategory
            {
                CategoryId = instance.CategoryId,
                CategoryName = instance.CategoryName,
                CompanyId = instance.CompanyId,
                CategoryBgColor = instance.CategoryBgColor,
                CategoryForeColor = instance.CategoryForeColor,
                IsActive = instance.IsActive,
                IsEnable = instance.IsEnable,
                IsInventoryCategory = instance.IsInventoryCategory,
                DepartmentId = instance.DepartmentId,
                CategoryImage = instance.CategoryImage,
                SortOrder = instance.SortOrder,
                ProductCardStyle = instance.ProductCardStyle,
                CategoryIcon = instance.CategoryIcon
            };
        }

    }
}
