namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Yeni ürün eklenirken dışarıdan alınan veri paketi.
/// (Validasyon işlemleri FluentValidation ile ayrı bir sınıfta yapılır).
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
}
