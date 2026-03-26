using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExportService.Entities
{
    public class OrderTokens
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("restaurant_id")]
        public int RestaurantId { get; set; }
        [Column("order_token")]
        public string OrderToken { get; set; }
        [Column("created_at", TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }
    }
}
