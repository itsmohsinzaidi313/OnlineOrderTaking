using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointofSaleModels.Entities
{
    public class Restaurants
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("connection_string")]
        public string ConnectionString { get; set; } = string.Empty;
        [Column("domain_name")]
        public string DomainName { get; set; } = string.Empty;
    }
}
