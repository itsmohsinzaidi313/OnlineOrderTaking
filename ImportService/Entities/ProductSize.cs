using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("product_size")]
    public class ProductSize
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int SizeId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string SizeName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        
        public bool IsActive { get; set; }
    }
}
