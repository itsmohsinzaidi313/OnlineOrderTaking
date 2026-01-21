using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("city")]
    public class City
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        public int CityId { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string CityName { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? CountryId { get; set; }

        [Column(TypeName = "INTEGER")]
        public int? ProvinceId { get; set; }
    }
}
