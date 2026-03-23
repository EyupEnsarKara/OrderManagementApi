using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Services.Interfaces;

namespace OrderManagementApi.Controllers;

/// <summary>
/// Ürün ekleme, listeleme, güncelleme ve silme (soft delete) isteklerini karşılar.
/// İş mantığı IProductService'e devredilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Tüm aktif ürünleri listeler.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// ID ile tek ürün getirir.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = $"ID {id} ile ürün bulunamadı." });

        return Ok(product);
    }

    /// <summary>
    /// Yeni ürün ekler.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mevcut ürünü günceller.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, dto);

            if (product == null)
                return NotFound(new { message = $"ID {id} ile ürün bulunamadı." });

            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Ürünü soft-delete yapar (IsActive = false).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
            return NotFound(new { message = $"ID {id} ile ürün bulunamadı." });

        return NoContent();
    }
}
