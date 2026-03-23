using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Yeni ürün eklenirken dışarıdan alınan veri paketi.
/// Id, CreatedAt, IsActive gibi alanlar sunucu tarafında otomatik atanır.
/// </summary>
public class CreateProductDto
{
    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stok miktarı zorunludur.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
    public int Stock { get; set; }
}
