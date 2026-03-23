namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Sipariş bilgisi dışarıya verilirken kullanılan veri paketi.
/// Status enum'un string karşılığı olarak döner (ör: "Pending").
/// ProductName, frontend'in ek sorgu yapmasını önler.
/// </summary>
public class OrderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string? CustomerNote { get; set; }
}
