using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Enums;
using OrderManagementApi.Services.Interfaces;

namespace OrderManagementApi.Controllers;

/// <summary>
/// Sipariş oluşturma, listeleme, durum güncelleme ve iptal isteklerini karşılar.
/// İş mantığı IOrderService'e devredilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Tüm siparişleri listeler.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// ID ile tek sipariş getirir.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
            return NotFound(new { message = $"ID {id} ile sipariş bulunamadı." });

        return Ok(order);
    }

    /// <summary>
    /// Yeni sipariş oluşturur. Stok kontrolü yapar, stoktan düşer.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Sipariş durumunu günceller.
    /// Body'de yeni status değeri gönderilir.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        // String'i enum'a çevir
        if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
            return BadRequest(new { message = $"Geçersiz durum: '{dto.Status}'. Geçerli değerler: Pending, Confirmed, Shipped, Delivered, Cancelled" });

        var order = await _orderService.UpdateOrderStatusAsync(id, newStatus);

        if (order == null)
            return NotFound(new { message = $"ID {id} ile sipariş bulunamadı." });

        return Ok(order);
    }

    /// <summary>
    /// Siparişi iptal eder ve stoğu geri yükler.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var result = await _orderService.CancelOrderAsync(id);

            if (!result)
                return NotFound(new { message = $"ID {id} ile sipariş bulunamadı." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

/// <summary>
/// Sipariş durumu güncellemek için kullanılan DTO.
/// PATCH /api/orders/{id}/status endpoint'inde kullanılır.
/// </summary>
public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
