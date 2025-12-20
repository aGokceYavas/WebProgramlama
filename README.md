#  Spor Salonu Yönetim ve Randevu Sistemi

Bu proje, **Sakarya Üniversitesi Bilgisayar Mühendisliği Bölümü – Web Programlama** dersi kapsamında geliştirilmiştir.  
ASP.NET Core MVC mimarisi kullanılarak geliştirilen sistem; spor salonlarındaki üye, eğitmen, hizmet paketi ve randevu süreçlerini dijital ortama taşımayı amaçlar.

Sistem, klasik yönetim fonksiyonlarının yanı sıra **yapay zeka destekli kişisel antrenör** özelliğiyle öne çıkmaktadır.

---

##  Projenin Öne Çıkan Özellikleri

-  **Yapay Zeka Destekli Antrenör**
  - Google **Gemini 2.0 Flash** modeli kullanılmıştır.
  - Üyeler yaş, boy, kilo, cinsiyet ve hedef bilgilerini girerek
    kişiye özel **haftalık antrenman ve beslenme programı** alabilir.

-  **Akıllı Randevu Yönetimi (Çakışma Kontrolü)**
  - Aynı saat dilimine birden fazla randevu alınması engellenir.
  - Dolu saatler için kullanıcı uyarılır ve kayıt yapılmaz.

-  **Otomatik Randevu Durum Güncelleme**
  - Süresi geçen randevular otomatik olarak **“Tamamlandı”** durumuna alınır.

-  **REST API & Raporlama**
  - Eğitmen performansları ve randevu yoğunlukları
    özel bir REST API üzerinden JSON formatında sunulur.

-  **Rol Bazlı Yetkilendirme**
  - **Admin** ve **Üye** olmak üzere iki farklı kullanıcı rolü bulunmaktadır.
  - Yönetim panelleri sadece yetkili kullanıcılar tarafından erişilebilir.

---

##  Kullanılan Teknolojiler

| Katman | Teknoloji |
|------|---------|
| Backend | C#, ASP.NET Core 8.0 MVC |
| Veritabanı | SQL Server, Entity Framework Core (Code First) |
| Frontend | HTML5, CSS3, JavaScript, Bootstrap 5 |
| Versiyon Kontrol | Git, GitHub |
| IDE | Visual Studio |

---

##  Kurulum ve Çalıştırma

Aşağıdaki adımları sırasıyla uygulayarak projeyi bilgisayarınızda çalıştırabilirsiniz.

### 1️ Projenin İndirilmesi

GitHub deposunu bilgisayarınıza klonlayın:

```bash
git clone https://github.com/aGokceYavas/WebProgramlama.git
```

veya  
GitHub sayfasındaki **Code → Download ZIP** seçeneğini kullanabilirsiniz.

---

### 2️ Veritabanının Oluşturulması (Migration)

Proje **Entity Framework Core – Code First** yaklaşımıyla geliştirilmiştir.

1. Projeyi **Visual Studio** ile açın  
2. Üst menüden aşağıdaki yolu izleyin:

```
Tools → NuGet Package Manager → Package Manager Console
```

3. Açılan konsolda aşağıdaki komutu çalıştırın:

```powershell
Update-Database
```

Bu işlem SQL Server üzerinde gerekli veritabanı ve tabloları otomatik olarak oluşturacaktır.

---

### 3️ Yapay Zeka API Anahtarı

- Google Gemini API anahtarı projeye tanımlıdır.
- API anahtarı şu dosyada yer almaktadır:

```
Controllers/YapayZekaController.cs
```

> Kendi API anahtarınızı kullanmak isterseniz bu dosya üzerinden değiştirebilirsiniz.

---

### 4️ Projenin Çalıştırılması

Visual Studio üzerinden:

- **IIS Express**
- veya **yeşil Başlat (Run) butonu**

kullanılarak proje tarayıcıda çalıştırılabilir.

---

##  Varsayılan Giriş Bilgileri

Proje ilk çalıştırıldığında sisteme otomatik olarak bir **Admin** hesabı eklenir.

**Admin Girişi**
-  E-posta: `b221210371@sakarya.edu.tr`
- Şifre: `sau`

>  Normal kullanıcı olarak sistemi test etmek için giriş ekranındaki **Kayıt Ol** butonunu kullanabilirsiniz.

---

##  Geliştirici Bilgileri

- **Ad Soyad:** Aybüke Gökçe Yavaş  
- **Öğrenci Numarası:** B221210371  
- **Bölüm:** Bilgisayar Mühendisliği  
- **Geliştirme Ortamı:** Visual Studio  

---

##  Notlar

- Proje akademik amaçlı geliştirilmiştir.
- Kod yapısı ASP.NET Core MVC standartlarına uygundur.
- Genişletilebilir ve modüler mimariye sahiptir.
