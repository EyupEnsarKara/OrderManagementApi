# 🛒 Order Management API (Demo)

Bu proje, temel e-ticaret süreçlerini (ürün yönetimi ve sipariş stok takibi) göstermek amacıyla geliştirilmiş, **Clean Architecture** prensiplerinden ilham alan bir ASP.NET Core Web API demosu uygulamasıdır.

## 🚀 Teknolojiler
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core (In-Memory DB)**
- **FluentValidation** (DTO Validasyonları için)
- **Swagger / OpenAPI** (API Dokümantasyonu)

## 🏗️ Mimari ve Tasarım Kararları
Mimaride **Thin Controller / Fat Service** prensibi benimsenmiştir.
- **Controllers:** Sadece HTTP isteklerini/yanıtlarını yönetir (`try-catch` içermez).
- **Services:** Tüm iş kuralları (stok kontrolü, sipariş onay vb.) burada yer alır.
- **DTOs & FluentValidation:** Kullanıcıdan gelen / giden verilerin doğrulanması `FluentValidation` ara katmanı ile yapılır.
- **Global Exception Middleware:** Patlayan tüm hatalar tek bir merkezden yakalanıp istemciye standart (Okunabilir JSON) formatında iletilir.
- **Atomik İşlemler:** Sipariş oluştururken stok düşürme, sipariş iptalinde stoğu geri yükleme senaryoları EF Core Transaction (`SaveChangesAsync`) sayesinde hatasız işler.

## ⚙️ Hızlı Kurulum ve Çalıştırma

Projede In-Memory veritabanı kullanıldığı için herhangi bir SQL kurulumuna gerek yoktur. Örnek **Seed Data** (örnek ürünler) uygulama başladığında belleğe yüklenir.

1. Repoyu klonlayın veya dizine gidin:
   ```bash
   cd OrderManagementApi
   ```
2. Projeyi derleyin ve çalıştırın:
   ```bash
   dotnet run
   ```
3. Test için tarayıcınızdan Swagger sayfasına gidin:  
   👉 `http://localhost:<PORT>/swagger`

## 📡 Temel Endpointler

### Ürünler (Products)
- `GET /api/products` : Tüm aktif ürünleri getirir.
- `GET /api/products/{id}` : ID ile tek ürün getirir.
- `POST /api/products` : Yeni ürün ekler.
- `PUT /api/products/{id}` : Ürünü günceller.
- `DELETE /api/products/{id}` : Ürünü siler (Soft-delete: `IsActive=false`).

### Siparişler (Orders)
- `GET /api/orders` : Tüm siparişleri listeler.
- `GET /api/orders/{id}` : Tek sipariş getirir.
- `POST /api/orders` : Yeni sipariş oluşturur (Stok kontrol edilip otomatik düşürülür).
- `PATCH /api/orders/{id}/status` : Sipariş durumunu günceller.
- `DELETE /api/orders/{id}` : Siparişi iptal eder (Stok hesaba geri iade edilir).
