# 🚗 Araç Yönetim Sistemi


Bu proje, ASP.NET Core kullanılarak geliştirilmiş bir kimlik yönetimi ve araç takip sistemidir. Kullanıcı yönetimi, araç CRUD işlemleri ve yetkilendirme özellikleri içerir.

<p align="center">
  <a href="#özellikler">Özellikler</a> •
  <a href="#teknolojiler">Teknolojiler</a> •
  <a href="#kurulum">Kurulum</a> •
  <a href="#kullanım">Kullanım</a> •
  <a href="#proje-yapısı">Proje Yapısı</a> •
  <a href="#ekran-görüntüleri">Ekran Görüntüleri</a> •
  <a href="#lisans">Lisans</a>
</p>

## ✨ Özellikler

- 👤 **Kullanıcı Yönetimi**: Kayıt, giriş, profil düzenleme ve rol tabanlı yetkilendirme
- 🚘 **Araç Kataloğu**: Kapsamlı araç veritabanı ve detaylı araç bilgisi
- 🔍 **Araç Arama**: Marka, model, yıl ve daha fazla kritere göre filtreleme
- 💖 **Favori Araçlar**: Kullanıcıların favori araçları kaydetmesi ve listelemesi
- 👑 **Yönetici Paneli**: Kullanıcı ve araçların yönetimi için güçlü admin arayüzü
- 📱 **Duyarlı Tasarım**: Mobil ve masaüstü cihazlarda optimal kullanıcı deneyimi

## 🛠️ Teknolojiler

<p align="center">
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/220px-.NET_Core_Logo.svg.png" alt=".NET Core" width="50" height="50" />
  <img src="https://www.sqlite.org/images/sqlite370_banner.gif" alt="SQLite" width="100" height="50" />
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/b/b2/Bootstrap_logo.svg/220px-Bootstrap_logo.svg.png" alt="Bootstrap" width="50" height="50" />
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/220px-.NET_Core_Logo.svg.png" alt="Entity Framework Core" width="50" height="50" />
</p>

- **ASP.NET Core** - Yüksek performanslı, çapraz platform web framework
- **Entity Framework Core** - Nesne ilişkisel eşleme (ORM) framework'ü
- **SQLite Veritabanı** - Hafif, hızlı ve güvenilir veritabanı sistemi
- **ASP.NET Core Identity** - Kimlik doğrulama ve yetkilendirme kütüphanesi
- **Bootstrap 5** - Duyarlı ve modern UI bileşenleri
- **HTML5/CSS3/JavaScript** - Zengin ve interaktif kullanıcı deneyimi

## 🚀 Kurulum

### Ön Koşullar

- .NET 8.0 SDK veya daha üstü
- Visual Studio 2022 veya VS Code
- Git

### Adımlar

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/[KullaniciAdi]/IdentityApp.git
   ```

2. Proje dizinine gidin:
   ```bash
   cd IdentityApp
   ```

3. Bağımlılıkları yükleyin:
   ```bash
   dotnet restore
   ```

4. Veritabanını oluşturun (Migrationlar zaten hazır):
   ```bash
   dotnet ef database update
   ```

5. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```

6. Tarayıcınızda aşağıdaki URL'yi açın:
   ```
   https://localhost:5001
   ```

## 📘 Kullanım

### Kullanıcı Hesabı

1. Uygulamaya ilk girişte kayıt olun veya demo hesapla giriş yapın:
   - **Admin Demo**: admin@example.com / Admin123!
   - **Kullanıcı Demo**: user@example.com / User123!

2. Kayıt olduktan sonra, giriş yaparak tam işlevlere erişebilirsiniz.

### Temel İşlevler

- **Ana Sayfa**: Tüm araçların listelendiği katalog sayfası
- **Araç Detayları**: Her aracın detaylı bilgilerini görüntüleme
- **Favori Araçlar**: Favori araçlarınızı kaydetme ve yönetme
- **Profil Yönetimi**: Kişisel bilgilerinizi güncelleme

### Yönetici İşlevleri

- **Kullanıcı Yönetimi**: Tüm kullanıcıları görüntüleme, düzenleme ve silme
- **Araç Yönetimi**: Araç ekleme, düzenleme ve silme
- **Rol Yönetimi**: Kullanıcı rollerini atama ve düzenleme

## 📁 Proje Yapısı

```
IdentityApp/
├── Controllers/     # MVC Controller dosyaları
├── Models/          # Veritabanı modelleri ve veri erişim katmanı
├── Views/           # Kullanıcı arayüzü şablonları
├── ViewModels/      # Görünüm modelleri
├── Migrations/      # Veritabanı şema değişiklikleri
└── wwwroot/         # Statik dosyalar (CSS, JavaScript, resimler)
```

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Nasıl katkıda bulunabileceğiniz hakkında daha fazla bilgi için:

1. Bu depoyu fork edin
2. Feature branch'i oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inize push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.
