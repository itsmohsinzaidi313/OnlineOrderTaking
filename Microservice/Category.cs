using System.ComponentModel.DataAnnotations;

namespace Microservice;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = default!;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Product> Products { get; set; } = [];

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }
}