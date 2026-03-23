using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Models.Enums;
using OrderManagementApi.Services.Interfaces;

namespace OrderManagementApi.Services.Implementations;

/// <summary>
/// Sipariş iş mantığını barındıran servis sınıfı.
/// Stok yönetimi (düşme ve iade) bu sınıfta gerçekleşir.
/// </summary>
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tüm siparişleri ürün bilgisiyle birlikte döndürür.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Product)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                Quantity = o.Quantity,
                UnitPrice = o.UnitPrice,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                CustomerNote = o.CustomerNote
            })
            .ToListAsync();
    }

    /// <summary>
    /// ID ile tek sipariş getirir. Bulunamazsa null döner.
    /// </summary>
    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            CustomerNote = order.CustomerNote
        };
    }

    /// <summary>
    /// Yeni sipariş oluşturur.
    /// KRİTİK: Stok kontrolü yapar, stoktan düşer, fiyatı yakalar.
    /// Product + Order tek SaveChanges ile atomik olarak kaydedilir.
    /// </summary>
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Adım 1: Ürünü bul
        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            throw new ArgumentException("Ürün bulunamadı.");

        if (!product.IsActive)
            throw new ArgumentException("Bu ürün artık satışta değil.");

        // Adım 2: Stok kontrolü
        if (product.Stock < dto.Quantity)
            throw new ArgumentException(
                $"Yetersiz stok. Mevcut: {product.Stock}, İstenen: {dto.Quantity}");

        // Adım 3: Fiyat yakalama (sipariş anındaki fiyat)
        var unitPrice = product.Price;

        // Adım 4: Toplam hesaplama
        var totalPrice = dto.Quantity * unitPrice;

        // Adım 5: Stoktan düşme
        product.Stock -= dto.Quantity;

        // Adım 6: Sipariş oluşturma
        var order = new Order
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = unitPrice,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            CustomerNote = dto.CustomerNote?.Trim()
        };

        _context.Orders.Add(order);

        // Adım 7: Atomik kayıt (Product stok güncellemesi + Order eklenmesi birlikte)
        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ProductName = product.Name,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            CustomerNote = order.CustomerNote
        };
    }

    /// <summary>
    /// Sipariş durumunu günceller. Bulunamazsa null döner.
    /// </summary>
    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        order.Status = newStatus;
        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            CustomerNote = order.CustomerNote
        };
    }

    /// <summary>
    /// Siparişi iptal eder ve stoğu geri yükler.
    /// KRİTİK: Stok iadesi atomik olarak yapılır.
    /// </summary>
    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return false;

        // Durum kontrolü
        if (order.Status == OrderStatus.Cancelled)
            throw new ArgumentException("Bu sipariş zaten iptal edilmiş.");

        if (order.Status == OrderStatus.Delivered)
            throw new ArgumentException("Teslim edilmiş sipariş iptal edilemez.");

        // Stok iadesi
        order.Product.Stock += order.Quantity;

        // Durum güncelleme
        order.Status = OrderStatus.Cancelled;

        // Atomik kayıt (Product stok + Order durum birlikte)
        await _context.SaveChangesAsync();

        return true;
    }
}
