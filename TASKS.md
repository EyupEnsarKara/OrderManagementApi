# 🛒 OrderManagementApi – Görev Listesi

> **Proje:** E-Ticaret Demo API  
> **Framework:** ASP.NET Core 8 Web API  
> **ORM:** Entity Framework Core (InMemory → ileride SQL Server)  
> **Mimari:** Thin Controller / Fat Service  
> **Tarih:** 2026-03-23

---

## 📁 Hedef Proje Hiyerarşisi

Aşağıdaki yapıya **birebir** uyulmalıdır. Tüm dosya ve klasör isimleri, namespace'ler ve konumlar bu hiyerarşiyi takip etmelidir.

```
OrderManagementApi/
│
├── Controllers/
│   ├── ProductsController.cs
│   └── OrdersController.cs
│
├── Models/
│   ├── Entities/
│   │   ├── Product.cs
│   │   └── Order.cs
│   │
│   ├── DTOs/
│   │   ├── ProductDto.cs
│   │   ├── CreateProductDto.cs
│   │   ├── OrderDto.cs
│   │   └── CreateOrderDto.cs
│   │
│   └── Enums/
│       └── OrderStatus.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/              (EF Core tarafından otomatik üretilecek)
│
├── Services/
│   ├── Interfaces/
│   │   ├── IProductService.cs
│   │   └── IOrderService.cs
│   │
│   └── Implementations/
│       ├── ProductService.cs
│       └── OrderService.cs
│
├── appsettings.json
├── Program.cs
├── OrderManagementApi.csproj
└── TASKS.md                     (Bu dosya)
```

> ⚠️ **ÖNEMLİ:** Her `.cs` dosyasının namespace'i, dosyanın bulunduğu klasör yapısına uygun olmalıdır.  
> Örnek: `Models/Entities/Product.cs` → `namespace OrderManagementApi.Models.Entities;`

---

## 📊 Mevcut Durum

| Bileşen | Durum | Açıklama |
|---|---|---|
| .NET 8 Web API şablonu | ✅ Mevcut | Proje oluşturulmuş |
| Swagger / OpenAPI | ✅ Mevcut | Swashbuckle yüklü |
| Klasör yapısı | ⚠️ Kısmen | Klasörler var ama hepsi boş |
| WeatherForecast kodu | ❌ Silinmeli | Program.cs'de hâlâ varsayılan şablon kodu var |
| AddControllers / MapControllers | ❌ Eksik | Henüz eklenmemiş |
| EF Core paketi | ❌ Eksik | NuGet'ten yüklenmeli |
| Connection String | ❌ Eksik | appsettings.json'da tanımlı değil |
| Entity / DTO / Enum dosyaları | ❌ Eksik | Hiçbiri yazılmamış |
| Service katmanı | ❌ Eksik | Hiçbiri yazılmamış |
| Controller katmanı | ❌ Eksik | Hiçbiri yazılmamış |

---

## ✅ FASE 1 – Proje Altyapısı & NuGet Paketleri

Bu adımlar diğer tüm adımlardan **önce** tamamlanmalıdır.

---

### Görev 1.1 — NuGet Paketlerini Yükle

**Dosya:** `OrderManagementApi.csproj` (otomatik güncellenir)

Aşağıdaki paketler terminalde çalıştırılarak projeye eklenmeli:

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.*
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.*
```

**Neden InMemory?**  
Demo projesi olduğu için harici bir veritabanı kurulumuna gerek kalmaması adına InMemory kullanıyoruz. İleride SQL Server veya SQLite'a geçiş kolaylıkla yapılabilir.

**Beklenen sonuç:** `.csproj` dosyasında 3 yeni `<PackageReference>` satırı oluşmalı.

---

### Görev 1.2 — `Program.cs` Temizliği ve Yapılandırması

**Dosya:** `Program.cs`

Yapılacaklar:

1. **Sil:** `WeatherForecast` record sınıfını ve `/weatherforecast` MapGet bloğunu tamamen kaldır.
2. **Ekle:** `builder.Services.AddControllers();` — Controller desteğini etkinleştir.
3. **Ekle:** `app.MapControllers();` — Controller route'larını tanıt.
4. **Ekle:** EF Core InMemory veritabanı kaydı:
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseInMemoryDatabase("OrderManagementDb"));
   ```
5. **Ekle:** Service katmanı DI (Dependency Injection) kayıtları:
   ```csharp
   builder.Services.AddScoped<IProductService, ProductService>();
   builder.Services.AddScoped<IOrderService, OrderService>();
   ```
6. **Koru:** Swagger/OpenAPI yapılandırmasını olduğu gibi bırak.

**Program.cs son hali şuna benzemelidir:**

```csharp
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Services.Interfaces;
using OrderManagementApi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// EF Core - InMemory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

// Dependency Injection - Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

> ⚠️ Not: Bu görev Fase 4 ve Fase 5 tamamlanmadan derlenmeyecektir çünkü referans edilen sınıflar henüz mevcut değildir. Önce modelleri ve servisleri yaz, en son Program.cs'yi güncelle.

---

### Görev 1.3 — `appsettings.json` Güncelle

**Dosya:** `appsettings.json`

InMemory kullandığımız için zorunlu değil, ancak ileride SQL Server'a geçiş için hazır bir yorum bırakılmalı:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrderManagementDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

> Not: InMemory DB kullanırken ConnectionStrings okunmaz. Bu alanı SQL Server'a geçiş için hazır tutuyoruz.

---

## ✅ FASE 2 – Model Katmanı (Entities, DTOs, Enums)

Bu katman tüm iş mantığının ve veri akışının temelidir. Tüm dosyalar `Models/` klasörü altındaki ilgili alt klasörlere yerleştirilmelidir.

---

### Görev 2.1 — `Models/Enums/OrderStatus.cs`

**Konum:** `Models/Enums/OrderStatus.cs`  
**Namespace:** `OrderManagementApi.Models.Enums`

Siparişin yaşam döngüsündeki durumlarını temsil eden enum:

```csharp
namespace OrderManagementApi.Models.Enums;

public enum OrderStatus
{
    Pending = 0,       // Bekliyor - Sipariş yeni oluşturuldu
    Confirmed = 1,     // Onaylandı - Sipariş onaylandı
    Shipped = 2,       // Kargoya Verildi
    Delivered = 3,     // Teslim Edildi
    Cancelled = 4      // İptal Edildi - Stok iadesi yapılır
}
```

**Kullanım amacı:** `Order` entity'sindeki `Status` alanı bu enum tipinde olacak. API dışarıya string olarak döndürecek (ör: `"Pending"`).

---

### Görev 2.2 — `Models/Entities/Product.cs`

**Konum:** `Models/Entities/Product.cs`  
**Namespace:** `OrderManagementApi.Models.Entities`

Veritabanındaki ürün tablosunu temsil eden entity sınıfı:

| Alan | Tip | Açıklama | Kısıtlama |
|---|---|---|---|
| `Id` | `int` | Primary Key, otomatik artan | PK |
| `Name` | `string` | Ürün adı | Zorunlu, maks. 200 karakter |
| `Description` | `string?` | Ürün açıklaması | Opsiyonel |
| `Price` | `decimal` | Birim fiyat | > 0 olmalı |
| `Stock` | `int` | Stok miktarı | >= 0 olmalı |
| `CreatedAt` | `DateTime` | Oluşturulma tarihi | Otomatik atanır |
| `IsActive` | `bool` | Ürün aktif mi? | Varsayılan: true (soft delete için) |
| `Orders` | `ICollection<Order>` | Navigation property | Bir ürünün birden fazla siparişi olabilir |

**Soft Delete mantığı:** Ürün silindiğinde veritabanından kaldırılmaz, `IsActive = false` yapılır. Böylece eski siparişlerle ilişki bozulmaz.

---

### Görev 2.3 — `Models/Entities/Order.cs`

**Konum:** `Models/Entities/Order.cs`  
**Namespace:** `OrderManagementApi.Models.Entities`

Veritabanındaki sipariş tablosunu temsil eden entity sınıfı:

| Alan | Tip | Açıklama | Kısıtlama |
|---|---|---|---|
| `Id` | `int` | Primary Key, otomatik artan | PK |
| `ProductId` | `int` | Hangi ürüne ait | FK → Product tablosu |
| `Product` | `Product` | Navigation property | EF Core ilişki |
| `Quantity` | `int` | Sipariş adedi | > 0 olmalı |
| `UnitPrice` | `decimal` | Sipariş anındaki birim fiyat | Değişmez, tarihsel kayıt |
| `TotalPrice` | `decimal` | `Quantity × UnitPrice` | Hesaplanarak atanır |
| `Status` | `OrderStatus` | Sipariş durumu | Varsayılan: `Pending` |
| `OrderDate` | `DateTime` | Sipariş tarihi | Otomatik atanır |
| `CustomerNote` | `string?` | Müşteri notu | Opsiyonel |

**Neden `UnitPrice` ayrıca kaydediliyor?**  
Ürün fiyatı ileride değişebilir. Sipariş anındaki fiyatı kaydetmezsek, eski siparişlerin tutarı yanlış görünür. Bu yüzden sipariş oluşturulurken `Product.Price` değeri `Order.UnitPrice`'a kopyalanır.

---

### Görev 2.4 — `Models/DTOs/CreateProductDto.cs`

**Konum:** `Models/DTOs/CreateProductDto.cs`  
**Namespace:** `OrderManagementApi.Models.DTOs`

Yeni ürün eklenirken dışarıdan alınan veri paketi:

| Alan | Tip | Açıklama | Validasyon |
|---|---|---|---|
| `Name` | `string` | Ürün adı | Zorunlu, boş olamaz |
| `Description` | `string?` | Ürün açıklaması | Opsiyonel |
| `Price` | `decimal` | Birim fiyat | Zorunlu, > 0 |
| `Stock` | `int` | Başlangıç stok miktarı | Zorunlu, >= 0 |

**Neden ayrı bir DTO?**  
- Kullanıcıdan `Id`, `CreatedAt`, `IsActive` gibi alanlar alınmaz, bunlar sunucu tarafında otomatik atanır.
- Güvenlik: Entity'yi doğrudan API'ye açmak tehlikelidir (over-posting saldırısı).

---

### Görev 2.5 — `Models/DTOs/ProductDto.cs`

**Konum:** `Models/DTOs/ProductDto.cs`  
**Namespace:** `OrderManagementApi.Models.DTOs`

Ürün listelenirken dışarıya verilen veri paketi:

| Alan | Tip | Açıklama |
|---|---|---|
| `Id` | `int` | Ürün ID'si |
| `Name` | `string` | Ürün adı |
| `Description` | `string?` | Ürün açıklaması |
| `Price` | `decimal` | Birim fiyat |
| `Stock` | `int` | Mevcut stok miktarı |
| `IsActive` | `bool` | Ürün aktif mi? |
| `CreatedAt` | `DateTime` | Oluşturulma tarihi |

---

### Görev 2.6 — `Models/DTOs/CreateOrderDto.cs`

**Konum:** `Models/DTOs/CreateOrderDto.cs`  
**Namespace:** `OrderManagementApi.Models.DTOs`

Müşterinin sipariş oluştururken gönderdiği veri paketi:

| Alan | Tip | Açıklama | Validasyon |
|---|---|---|---|
| `ProductId` | `int` | Sipariş edilecek ürünün ID'si | Zorunlu |
| `Quantity` | `int` | Kaç adet isteniyor | Zorunlu, > 0 |
| `CustomerNote` | `string?` | Müşteri notu | Opsiyonel |

**Kullanıcıdan alınmayan bilgiler:** `UnitPrice`, `TotalPrice`, `OrderDate`, `Status` — bunların hepsi sunucu tarafında hesaplanır ve atanır.

---

### Görev 2.7 — `Models/DTOs/OrderDto.cs`

**Konum:** `Models/DTOs/OrderDto.cs`  
**Namespace:** `OrderManagementApi.Models.DTOs`

Sipariş bilgisi dışarıya verilirken kullanılan veri paketi:

| Alan | Tip | Açıklama |
|---|---|---|
| `Id` | `int` | Sipariş ID'si |
| `ProductId` | `int` | Ürün ID'si |
| `ProductName` | `string` | Ürün adı (kolaylık için) |
| `Quantity` | `int` | Sipariş adedi |
| `UnitPrice` | `decimal` | Sipariş anındaki birim fiyat |
| `TotalPrice` | `decimal` | Toplam tutar |
| `Status` | `string` | Sipariş durumu (enum'un string karşılığı, ör: `"Pending"`) |
| `OrderDate` | `DateTime` | Sipariş tarihi |
| `CustomerNote` | `string?` | Müşteri notu |

**Neden `ProductName` var?**  
Frontend'in sadece sipariş listesi alırken ürün adını da görebilmesi için. Böylece ayrıca ürün bilgisi sorgulamaya gerek kalmaz.

**Neden `Status` string?**  
Enum integer olarak saklanır ama API yanıtında `0` yerine `"Pending"` yazmak çok daha okunaklı ve anlaşılırdır.

---

## ✅ FASE 3 – Veri Erişim Katmanı (Data Layer)

---

### Görev 3.1 — `Data/AppDbContext.cs`

**Konum:** `Data/AppDbContext.cs`  
**Namespace:** `OrderManagementApi.Data`

EF Core DbContext sınıfı. C# sınıflarımızı veritabanı tablolarına bağlayan köprü.

**İçermesi gerekenler:**

1. **DbSet tanımları:**
   ```csharp
   public DbSet<Product> Products { get; set; }
   public DbSet<Order> Orders { get; set; }
   ```

2. **`OnModelCreating` konfigürasyonları:**

   **Product tablosu:**
   - `Name` → zorunlu, maks. 200 karakter
   - `Price` → `decimal(18,2)` hassasiyetle (para birimi için)
   - `IsActive` → varsayılan değer `true`
   - `CreatedAt` → varsayılan değer `DateTime.UtcNow`

   **Order tablosu:**
   - `ProductId` → Foreign Key, `Product` ile ilişki
   - `UnitPrice` → `decimal(18,2)`
   - `TotalPrice` → `decimal(18,2)`
   - `Status` → enum olarak integer saklanır
   - `OnDelete` → `Restrict` (Ürün silinirse siparişler silinmemeli)

3. **Seed Data (Demo Verileri):**
   
   `OnModelCreating` içinde 3-5 örnek ürün ekle:

   | Id | Name | Description | Price | Stock |
   |---|---|---|---|---|
   | 1 | Kablosuz Mouse | Ergonomik kablosuz mouse | 299.99 | 50 |
   | 2 | Mekanik Klavye | RGB aydınlatmalı mekanik klavye | 899.50 | 30 |
   | 3 | USB-C Hub | 7-in-1 USB-C çoklayıcı | 449.00 | 100 |
   | 4 | Monitor Standı | Ayarlanabilir alüminyum stand | 599.90 | 25 |
   | 5 | Webcam HD | 1080p HD webcam, mikrofon dahil | 349.00 | 40 |

   Bu veriler uygulama her başlatıldığında InMemory DB'ye otomatik yüklenecektir.

---

## ✅ FASE 4 – Servis Katmanı (İş Mantığı)

Tüm iş kuralları ve hesaplamalar bu katmanda uygulanır. Controller'lar yalnızca HTTP isteklerini karşılar ve Service'e yönlendirir. Bu yaklaşıma **Thin Controller / Fat Service** denir.

---

### Görev 4.1 — `Services/Interfaces/IProductService.cs`

**Konum:** `Services/Interfaces/IProductService.cs`  
**Namespace:** `OrderManagementApi.Services.Interfaces`

Ürün işlemleri için sözleşme (interface):

```csharp
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
}
```

| Metod | Açıklama |
|---|---|
| `GetAllProductsAsync` | Tüm aktif ürünleri döndürür (`IsActive == true`) |
| `GetProductByIdAsync` | ID ile tek ürün getirir, bulunamazsa `null` |
| `CreateProductAsync` | Yeni ürün oluşturur, validasyonları uygular |
| `UpdateProductAsync` | Mevcut ürünü günceller, bulunamazsa `null` |
| `DeleteProductAsync` | Soft delete yapar (`IsActive = false`), başarı durumunu döndürür |

---

### Görev 4.2 — `Services/Interfaces/IOrderService.cs`

**Konum:** `Services/Interfaces/IOrderService.cs`  
**Namespace:** `OrderManagementApi.Services.Interfaces`

Sipariş işlemleri için sözleşme (interface):

```csharp
public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, OrderStatus newStatus);
    Task<bool> CancelOrderAsync(int id);
}
```

| Metod | Açıklama |
|---|---|
| `GetAllOrdersAsync` | Tüm siparişleri ürün bilgisiyle birlikte döndürür |
| `GetOrderByIdAsync` | ID ile tek sipariş getirir |
| `CreateOrderAsync` | Sipariş oluşturur, stok kontrolü ve düşüşü yapar |
| `UpdateOrderStatusAsync` | Sipariş durumunu günceller |
| `CancelOrderAsync` | Siparişi iptal eder ve stoğu geri yükler |

---

### Görev 4.3 — `Services/Implementations/ProductService.cs`

**Konum:** `Services/Implementations/ProductService.cs`  
**Namespace:** `OrderManagementApi.Services.Implementations`

`IProductService` interface'ini implement eden sınıf. Constructor'da `AppDbContext` alır (DI ile enjekte edilir).

**Her metodun detaylı davranışı:**

#### `GetAllProductsAsync()`
1. `_context.Products` üzerinden `Where(p => p.IsActive)` filtresi uygula
2. Her entity'yi `ProductDto`'ya dönüştür
3. Listeyi döndür

#### `GetProductByIdAsync(int id)`
1. `_context.Products.FindAsync(id)` ile ürünü bul
2. Bulunamazsa → `null` döndür
3. Bulunursa → `ProductDto`'ya dönüştür ve döndür

#### `CreateProductAsync(CreateProductDto dto)`
1. **Validasyonlar:**
   - `dto.Name` boş veya null ise → `ArgumentException` fırlat
   - `dto.Price <= 0` ise → `ArgumentException` fırlat ("Fiyat 0'dan büyük olmalıdır")
   - `dto.Stock < 0` ise → `ArgumentException` fırlat ("Stok negatif olamaz")
2. Yeni `Product` entity'si oluştur:
   - `CreatedAt = DateTime.UtcNow`
   - `IsActive = true`
3. `_context.Products.Add(product)` ile ekle
4. `_context.SaveChangesAsync()` ile kaydet
5. `ProductDto`'ya dönüştür ve döndür

#### `UpdateProductAsync(int id, CreateProductDto dto)`
1. Ürünü ID ile bul
2. Bulunamazsa → `null` döndür
3. Aynı validasyonları uygula (fiyat, stok, ad)
4. Alanları güncelle
5. `SaveChangesAsync()` ile kaydet
6. Güncel `ProductDto`'yu döndür

#### `DeleteProductAsync(int id)` — Soft Delete
1. Ürünü ID ile bul
2. Bulunamazsa → `false` döndür
3. `product.IsActive = false` yap
4. `SaveChangesAsync()` ile kaydet
5. `true` döndür

---

### Görev 4.4 — `Services/Implementations/OrderService.cs`

**Konum:** `Services/Implementations/OrderService.cs`  
**Namespace:** `OrderManagementApi.Services.Implementations`

`IOrderService` interface'ini implement eden sınıf. Constructor'da `AppDbContext` alır.

**Her metodun detaylı davranışı:**

#### `GetAllOrdersAsync()`
1. `_context.Orders.Include(o => o.Product)` ile siparişleri ürün bilgisiyle birlikte getir
2. Her entity'yi `OrderDto`'ya dönüştür:
   - `Status` → `order.Status.ToString()` (enum → string)
   - `ProductName` → `order.Product.Name`
3. Listeyi döndür

#### `GetOrderByIdAsync(int id)`
1. `Include(o => o.Product)` ile siparişi bul
2. Bulunamazsa → `null`
3. `OrderDto`'ya dönüştür ve döndür

#### `CreateOrderAsync(CreateOrderDto dto)` — ⚠️ KRİTİK: STOK YÖNETİMİ

Bu metod projenin en kritik iş mantığını içerir. Adım adım:

```
Adım 1: Ürünü bul
  └─ Ürün yoksa → Exception: "Ürün bulunamadı"
  └─ Ürün IsActive == false ise → Exception: "Bu ürün artık satışta değil"

Adım 2: Stok kontrolü
  └─ Product.Stock < dto.Quantity ise → Exception: "Yetersiz stok. Mevcut: {stok}, İstenen: {adet}"

Adım 3: Fiyat yakalama
  └─ UnitPrice = Product.Price (sipariş anındaki fiyatı kaydet)

Adım 4: Toplam hesaplama
  └─ TotalPrice = dto.Quantity × UnitPrice

Adım 5: Stoktan düşme
  └─ Product.Stock -= dto.Quantity

Adım 6: Sipariş oluşturma
  └─ Status = OrderStatus.Pending
  └─ OrderDate = DateTime.UtcNow

Adım 7: Kaydetme (ATOMİK)
  └─ Product güncellemesi + Order eklenmesi TEK SaveChangesAsync() çağrısıyla yapılmalı
  └─ Böylece ya ikisi birden başarılı olur ya da ikisi birden geri alınır
```

#### `UpdateOrderStatusAsync(int id, OrderStatus newStatus)`
1. Siparişi bul (Include Product)
2. Bulunamazsa → `null`
3. `order.Status = newStatus`
4. `SaveChangesAsync()`
5. Güncel `OrderDto` döndür

#### `CancelOrderAsync(int id)` — ⚠️ KRİTİK: STOK İADESİ

```
Adım 1: Siparişi bul (Include Product)
  └─ Bulunamazsa → false

Adım 2: Durum kontrolü
  └─ Zaten Cancelled ise → Exception: "Bu sipariş zaten iptal edilmiş"
  └─ Delivered ise → Exception: "Teslim edilmiş sipariş iptal edilemez"

Adım 3: Stok iadesi
  └─ Product.Stock += Order.Quantity

Adım 4: Durum güncelleme
  └─ Order.Status = OrderStatus.Cancelled

Adım 5: Kaydetme (ATOMİK)
  └─ Product stok güncellemesi + Order durum güncellemesi TEK SaveChangesAsync()
```

---

## ✅ FASE 5 – Controller Katmanı (API Endpoints)

Controller'lar yalnızca HTTP isteklerini karşılar ve Service katmanına yönlendirir. **İş mantığı Controller'da OLMAMALIDIR.**

Her controller:
- `[ApiController]` attribute'u ile işaretlenmeli
- `[Route("api/[controller]")]` ile route tanımlanmalı
- Constructor'da ilgili Service interface'ini almalı (DI)

---

### Görev 5.1 — `Controllers/ProductsController.cs`

**Konum:** `Controllers/ProductsController.cs`  
**Namespace:** `OrderManagementApi.Controllers`  
**Route:** `api/products`

| HTTP | Endpoint | Metod | Açıklama | Başarılı | Hatalı |
|---|---|---|---|---|---|
| `GET` | `/api/products` | `GetAll()` | Tüm aktif ürünleri listele | `200 OK` + `List<ProductDto>` | — |
| `GET` | `/api/products/{id}` | `GetById(int id)` | Tek ürün getir | `200 OK` + `ProductDto` | `404 Not Found` |
| `POST` | `/api/products` | `Create(CreateProductDto)` | Yeni ürün ekle | `201 Created` + `ProductDto` | `400 Bad Request` |
| `PUT` | `/api/products/{id}` | `Update(int id, CreateProductDto)` | Ürünü güncelle | `200 OK` + `ProductDto` | `404 Not Found` |
| `DELETE` | `/api/products/{id}` | `Delete(int id)` | Ürünü soft-delete | `204 No Content` | `404 Not Found` |

**Detaylar:**
- `POST` başarılı olduğunda `CreatedAtAction` ile `201` döndür (Location header'ı ile birlikte)
- `DELETE` başarılı olduğunda `204 No Content` döndür (body boş)
- Validasyon hatalarında Service'ten gelen exception'ı yakala ve `400 Bad Request` döndür

---

### Görev 5.2 — `Controllers/OrdersController.cs`

**Konum:** `Controllers/OrdersController.cs`  
**Namespace:** `OrderManagementApi.Controllers`  
**Route:** `api/orders`

| HTTP | Endpoint | Metod | Açıklama | Başarılı | Hatalı |
|---|---|---|---|---|---|
| `GET` | `/api/orders` | `GetAll()` | Tüm siparişleri listele | `200 OK` + `List<OrderDto>` | — |
| `GET` | `/api/orders/{id}` | `GetById(int id)` | Tek sipariş getir | `200 OK` + `OrderDto` | `404 Not Found` |
| `POST` | `/api/orders` | `Create(CreateOrderDto)` | Yeni sipariş oluştur | `201 Created` + `OrderDto` | `400 Bad Request` |
| `PATCH` | `/api/orders/{id}/status` | `UpdateStatus(int id, ...)` | Durumu güncelle | `200 OK` + `OrderDto` | `404 Not Found` |
| `DELETE` | `/api/orders/{id}` | `Cancel(int id)` | İptal et + stok iadesi | `204 No Content` | `400 Bad Request` |

**Detaylar:**
- `POST` — stok yetersizse veya ürün bulunamazsa `400 Bad Request` döndür, hata mesajını body'de açıkla
- `PATCH` — sadece `Status` alanını günceller; body'de yeni status gönderilir
- `DELETE` — iptal işlemi. Zaten iptal edilmişse veya teslim edilmişse `400 Bad Request`

---

## ✅ FASE 6 – Program.cs DI Kaydı (Son Hal)

---

### Görev 6.1 — `Program.cs` Final Güncelleme

**Dosya:** `Program.cs`

Bu görev, Fase 1.2'de yazılan taslağın son halidir. Tüm sınıflar artık mevcut olduğu için derleme hatası vermeyecektir.

Kontrol listesi:
- [ ] `using` satırları doğru mu?
- [ ] `AddDbContext<AppDbContext>` eklendi mi?
- [ ] `AddScoped<IProductService, ProductService>` eklendi mi?
- [ ] `AddScoped<IOrderService, OrderService>` eklendi mi?
- [ ] `AddControllers()` eklendi mi?
- [ ] `MapControllers()` eklendi mi?
- [ ] WeatherForecast kodu tamamen silindi mi?
- [ ] Swagger korunuyor mu?

---

## ✅ FASE 7 – Derleme, Çalıştırma ve Test

---

### Görev 7.1 — Projeyi Derle

```bash
dotnet build
```

Beklenti: **0 hata, 0 uyarı** (veya kabul edilebilir uyarılar).

---

### Görev 7.2 — Projeyi Çalıştır

```bash
dotnet run
```

Beklenti: Uygulama başarıyla başlamalı. Konsolda `Now listening on: https://localhost:xxxx` mesajı görünmeli.

---

### Görev 7.3 — Swagger UI Üzerinden Manuel Test

Tarayıcıda `https://localhost:xxxx/swagger` adresine git ve aşağıdaki senaryoları test et:

| # | Senaryo | Beklenen Sonuç |
|---|---|---|
| 1 | `GET /api/products` | Seed data'daki 5 ürün listelenmeli |
| 2 | `POST /api/products` — geçerli veri | `201 Created`, yeni ürün dönmeli |
| 3 | `POST /api/products` — fiyat: -10 | `400 Bad Request`, hata mesajı |
| 4 | `POST /api/orders` — ProductId: 1, Quantity: 2 | `201 Created`, stok 50→48 düşmeli |
| 5 | `GET /api/products/1` | Stok 48 olarak görünmeli |
| 6 | `POST /api/orders` — ProductId: 1, Quantity: 999 | `400 Bad Request`, "Yetersiz stok" |
| 7 | `DELETE /api/orders/{id}` | `204`, stok geri artmalı |
| 8 | `DELETE /api/products/1` | `204`, ürün artık listede görünmemeli |
| 9 | `POST /api/orders` — ProductId: 1 (silinen ürün) | `400`, "Ürün satışta değil" |

---

## ✅ FASE 8 – İyileştirmeler (Opsiyonel)

Bu görevler temel API çalıştıktan sonra eklenebilir. Öncelik sırasına göre listelenmiştir.

---

### Görev 8.1 — Global Exception Handling Middleware

Tüm beklenmeyen hataları yakala ve tutarlı bir JSON formatında döndür:

```json
{
  "statusCode": 400,
  "message": "Yetersiz stok. Mevcut: 48, İstenen: 999",
  "timestamp": "2026-03-23T13:45:00Z"
}
```

---

### Görev 8.2 — DTO Validasyonu (Data Annotations veya FluentValidation)

DTO sınıflarına validasyon kuralları ekle:

```csharp
public class CreateProductDto
{
    [Required(ErrorMessage = "Ürün adı zorunludur")]
    [MaxLength(200)]
    public string Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz")]
    public int Stock { get; set; }
}
```

---

### Görev 8.3 — AutoMapper Entegrasyonu

Entity ↔ DTO dönüşümlerini elle yapmak yerine AutoMapper ile otomatikleştir:

```bash
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

---

### Görev 8.4 — Sayfalama (Pagination) Desteği

`GET` endpoint'lerine sayfalama parametreleri ekle:

```
GET /api/products?page=1&pageSize=10
```

Response header'larında:
- `X-Total-Count`: Toplam kayıt sayısı
- `X-Total-Pages`: Toplam sayfa sayısı

---

### Görev 8.5 — Loglama (Serilog)

Yapılandırılmış loglama sistemi ekle:

```bash
dotnet add package Serilog.AspNetCore
```

---

### Görev 8.6 — SQL Server / SQLite'a Geçiş + Migration

InMemory DB'den gerçek veritabanına geçiş:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### Görev 8.7 — Unit Test Projesi

```bash
dotnet new xunit -n OrderManagementApi.Tests
dotnet add OrderManagementApi.Tests reference OrderManagementApi
```

ProductService ve OrderService için birim testler yaz. Özellikle stok yönetimi senaryolarını kapsamlı test et.

---

## 📌 Özet: Uygulama Sırası

```
1. NuGet paketlerini yükle
2. Enums → OrderStatus.cs
3. Entities → Product.cs, Order.cs
4. DTOs → CreateProductDto, ProductDto, CreateOrderDto, OrderDto
5. Data → AppDbContext.cs (Seed Data dahil)
6. Services/Interfaces → IProductService, IOrderService
7. Services/Implementations → ProductService, OrderService
8. Controllers → ProductsController, OrdersController
9. Program.cs → DI kaydı, DB kaydı, Controller kaydı
10. dotnet build → Hata kontrolü
11. dotnet run → Swagger testi
```

> **Her adımda dosya konumunun ve namespace'in hiyerarşiye uyduğundan emin ol!**
