using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Middlewares;
using OrderManagementApi.Services.Implementations;
using OrderManagementApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Swagger / OpenAPI ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Fluent Validation ──
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ── Controllers ──
builder.Services.AddControllers();

// ── EF Core - InMemory Database ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

// ── Dependency Injection - Services ──
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// ── Standart Exception Yakalama Middleware'i ──
app.UseGlobalExceptionHandler();

// ── Seed Data'yı InMemory DB'ye yükle ──
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// ── HTTP Pipeline ──
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
