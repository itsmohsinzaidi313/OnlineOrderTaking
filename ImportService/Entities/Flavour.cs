using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("flavour")]
    public class Flavour
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int FlavourId { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? FlavourName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CompanyId { get; set; }

        
        public bool IsActive { get; set; }
    }
}
