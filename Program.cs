using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagementApi.Data;
using OrderManagementApi.Middlewares;
using OrderManagementApi.Services.Implementations;
using OrderManagementApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── JWT Authentication Yapılandırması ──
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// ── Swagger / OpenAPI (Authorize Butonu ile birlikte) ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });

    // Swagger'a Jwt Bearer tanımlaması eklendi
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Lütfen Bearer [boşluk] {token} formatında giriniz. Örn: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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

// ── Kimlik ve Yetki Kontrolü ── 
app.UseAuthentication(); // Önce "Sen Kimsin?" (Login olanı tanı)
app.UseAuthorization();  // Sonra "Buna Yetkin Var Mı?"

app.MapControllers();
app.Run();
