using System.Net;
using System.Text.Json;

namespace OrderManagementApi.Middlewares;

/// <summary>
/// Tüm uygulamada fırlatılan Exception'ları tek bir merkezden yakalayıp 
/// istemciye okunaklı ve standart bir JSON hata mesajı dönme ara katmanıdır.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // İsteği sonraki middleware veya controller'a ilet
            await _next(context);
        }
        catch (Exception ex)
        {
            // Eğer orada bir hata patlarsa yakala ve Handle et
            _logger.LogError(ex, "Uygulamada beklenmeyen bir hata oluştu!");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Varsayılan HTTP Kodu: 500 (Internal Server Error)
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Beklenmeyen bir sunucu hatası oluştu.";

        // Eğer beklediğimiz / bizim fırlattığımız iş mantığı hatalarıysa 400 döneceğiz
        if (exception is ArgumentException || exception is InvalidOperationException)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
            message = exception.Message; // Bizim yazdığımız mesaj (Örn: "Stok yetersiz")
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // Standart Hata Çıktısı (Frontend / Swagger için kalıp JSON)
        var responseDict = new
        {
            statusCode = statusCode,
            message = message,
            timestamp = DateTime.UtcNow
        };

        var result = JsonSerializer.Serialize(responseDict);
        return context.Response.WriteAsync(result);
    }
}

// Kolay kullanım için bir extension method
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
