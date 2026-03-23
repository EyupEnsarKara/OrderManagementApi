using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.Models.Entities;

/// <summary>
/// Veritabanındaki ürün tablosunu temsil eden entity sınıfı.
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft delete için kullanılır. false ise ürün pasif (silinmiş) kabul edilir.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation property - Bir ürünün birden fazla siparişi olabilir
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
