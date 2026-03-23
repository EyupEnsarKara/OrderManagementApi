# 🛒 Order Management API (DEMO) (MySQL & JWT Auth)

Bu proje, temel e-ticaret süreçlerini (ürün yönetimi ve sipariş stok takibi) göstermek amacıyla geliştirilmiş, **Clean Architecture** prensiplerinden ilham alan güvenli bir ASP.NET Core Web API projesidir.

## 🚀 Teknolojiler
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core (MySQL - Pomelo)**
- **JWT Bearer Authentication** (Guvenli Giriş Sistemi)
- **FluentValidation** (DTO Validasyonları için)
- **Swagger / OpenAPI** (API Dokümantasyonu ve Token Desteği)

## 🏗️ Mimari ve Tasarım Kararları
Mimaride **Thin Controller / Fat Service** prensibi benimsenmiştir.
- **Controllers:** Sadece HTTP isteklerini/yanıtlarını yönetir (`try-catch` içermez).
- **Services:** Tüm iş kuralları (stok kontrolü, sipariş onay vb.) burada yer alır.
- **DTOs & FluentValidation:** Verilerin doğrulanması `FluentValidation` ara katmanı ile yapılır.
- **Global Exception Middleware:** Tüm hatalar tek merkezden yakalanıp standart JSON döner.
- **Atomik İşlemler:** Sipariş oluştururken stok düşürme işlemi EF Core Transaction (`SaveChangesAsync`) sayesinde hatasız işler.

## 🔐 Veritabanı ve Şifre Yönetimi (Environment)

Node.js ve PHP'deki `.env` dosyası kurgusunun Microsoft .NET ekosistemindeki en güvenli karşılığı olan **"User Secrets"** mimarisi kullanılmıştır.

Gerçek veritabanı şifreleri `appsettings.json` içerisine yazılmaz (böylece GitHub vb. yerlere sızmaz). Şifreler bilgisayarınızın korumalı `%APPDATA%\Microsoft\UserSecrets\` klasöründe dışarıdan görünmez şekilde tutulur ve uygulama başlarken `appsettings.json` ayarlarının üzerine yazılır.

## ⚙️ Hızlı Kurulum ve Çalıştırma

Projeyi bilgisayarınızda (MySQL kurulu olarak) çalıştırmak için aşağıdaki adımları izleyin:

### 1- Veritabanı Şifrenizi Sisteme Tanıtın
Repoyu indirdikten sonra terminali açıp `.NET User Secrets` ile kendi MySQL şifrenizi projeye entegre edin (Aşağıdaki kodda `Pwd=...` kısmına **kendi MySQL şifrenizi** yazın):
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OrderManagementDb;Uid=root;Pwd=SENIN_MYSQL_SIFREN;"
```

### 2- Tabloları MySQL Sisteminde Oluşturun (Migration)
Aşağıdaki komut, C# objelerimizi analiz edip MySQL sunucunuzda `OrderManagementDb` isimli tüm tabloları sıfırdan oluşturur:
```bash
dotnet ef migrations add InitialMySql -o Data/Migrations
dotnet ef database update
```
*(Not: Bu projede `context.Database.Migrate()` kodlandığı için `dotnet run` deyince gerçi kendi kendine veritabanı oluşturmasını yapacaktır, ancak klasik yapı taşlarının oturması için manuel tetikleme en iyisidir).*

### 3- Projeyi Çalıştırın
```bash
dotnet run
```

Test için tarayıcınızdan Swagger sayfasına gidin:  
👉 `http://localhost:<PORT>/swagger`
*(Api açılınca en üstte sağda çıkan Authorize butonundan JWT Tokeninizi girerek tüm kilitli sayfalarda gezebilirsiniz).*

## 📡 Temel Endpointler

### Auth (JWT Üretici)
- `POST /api/auth/login` : Sisteme girer (`admin` & `123456`) ve JWT Token üretir.

### Ürünler (Products)
- `GET /api/products` : Tüm aktif ürünleri getirir (Herkese Açık).
- `GET /api/products/{id}` : ID ile tek ürün getirir (Herkese Açık).
- `POST /api/products` : Yeni ürün ekler (JWT Zorunlu).
- `PUT /api/products/{id}` : Ürünü günceller (JWT Zorunlu).
- `DELETE /api/products/{id}` : Ürünü siler (Soft-delete: `IsActive=false` - JWT Zorunlu).

### Siparişler (Orders)
- `GET /api/orders` : Tüm siparişleri listeler (JWT Zorunlu).
- `GET /api/orders/{id}` : Tek sipariş getirir (JWT Zorunlu).
- `POST /api/orders` : Sipariş oluşturur (JWT Zorunlu - Stok otomatik düşer).
- `PATCH /api/orders/{id}/status` : Sipariş durumunu günceller (JWT Zorunlu).
- `DELETE /api/orders/{id}` : Siparişi iptal eder (JWT Zorunlu - Stok geri iade edilir).
