using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigration.Domain.Entities
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int ProductId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? ProductName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? ProductCategoryId { get; set; }

        
        public bool IsActive { get; set; }

        
        public bool IsEnable { get; set; }

        
        public bool DisplayInPos { get; set; }

        
        public bool DisplayInWeb { get; set; }

        
        public bool DisplayInOdms { get; set; }

        
        public bool DisplayInMobile { get; set; }

        
        public bool IsDeal { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string? ProductImage { get; set; }

        
        public bool IsExpiryMandatory { get; set; }

        [Column(TypeName = "DOUBLE PRECISION")]
        public double CommisionValue { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CommisionTypeId { get; set; }

        [Column(TypeName = "text")]
        public string? ProductDescription { get; set; }

        [Column(TypeName = "INTEGER")]
        public int SortOrder { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? ProductTagId { get; set; }

        
        public bool IsDealPackage { get; set; }

        // ?? Navigation
        [ForeignKey(nameof(ProductCategoryId))]
        public virtual ProductCategory? ProductCategory { get; set; }
    }
}
