SPOR SALONU YÖNETİM VE RANDEVU SİSTEMİ PROJE DOKÜMANTASYONU


1. PROJE TANIMI Bu proje, Sakarya Üniversitesi Bilgisayar Mühendisliği Bölümü Web Programlama dersi projesi olarak hazırlanmıştır. Geliştirilen "Spor Salonu Yönetim Sistemi", bir spor işletmesindeki üye, eğitmen, hizmet paketi ve randevu süreçlerini dijitalleştirmeyi amaçlar. ASP.NET Core MVC mimarisi üzerine kurulu olan sistem, sadece veri kaydetmekle kalmayıp, yapay zeka entegrasyonu ile üyelere kişisel antrenörlük hizmeti sunmaktadır.



2. SİSTEMİN ÖNE ÇIKAN ÖZELLİKLERİ Proje, kullanıcı deneyimini ve veri bütünlüğünü sağlamak adına aşağıdaki gelişmiş teknik özellikleri barındırır:

Yapay Zeka Destekli Antrenör (AI Integration): Sistemde Google Gemini 2.0 Flash modeli kullanılmıştır. Üyeler; yaş, boy, kilo, cinsiyet ve hedeflerini girdiklerinde, sistem yapay zeka servisine bağlanarak kişiye özel haftalık antrenman ve beslenme programı oluşturur.

Akıllı Randevu Yönetimi ve Çakışma Kontrolü: Sistem, aynı saat dilimine birden fazla randevu alınmasını engelleyen bir "Çakışma Kontrolü" (Conflict Detection) mekanizmasına sahiptir. Bir üye randevu almaya çalışırken, o saat başkası tarafından dolmuşsa sistem uyarı verir ve kaydı engeller.

Otomatik Durum Güncelleme: Uygulama her çalıştırıldığında veya ana sayfaya girildiğinde, arka planda çalışan bir algoritma tarihi geçmiş randevuları tespit eder ve statülerini otomatik olarak "Tamamlandı" olarak günceller.

REST API ve Raporlama: Eğitmenlerin performansını ölçmek ve randevu yoğunluklarını analiz etmek için özel bir REST API geliştirilmiştir. Bu API, veritabanından çekilen verileri LINQ sorguları ile filtreleyerek JSON formatında sunar.

Yetkilendirme: Sistemde Admin ve Üye olmak üzere iki farklı rol mevcuttur. Yönetim panellerine sadece yetkili girişler yapılabilir.



3. TEKNİK ALTYAPI Projede kullanılan temel teknolojiler şunlardır:

Yazılım Dili ve Çatı: C#, ASP.NET Core 8.0 MVC

Veritabanı: SQL Server, Entity Framework Core (Code First)

Arayüz Tasarımı: HTML5, CSS3, JavaScript, Bootstrap 5

Versiyon Kontrol: Git ve GitHub



4. KURULUM VE ÇALIŞTIRMA ADIMLARI Projeyi bilgisayarınıza kurmak ve hatasız çalıştırmak için lütfen aşağıdaki adımları sırasıyla uygulayınız:

Adım 1: Projenin İndirilmesi GitHub deposunda yer alan proje dosyalarını "Code" butonuna basıp "Download ZIP" diyerek indirin veya Git komutu ile bilgisayarınıza klonlayın.

Adım 2: Veritabanının Oluşturulması (Migration) Proje "Code First" yaklaşımı ile yazıldığı için veritabanı tabloları kod üzerinden oluşturulmalıdır.

Projeyi Visual Studio ile açın.

Üst menüden "Tools" > "NuGet Package Manager" > "Package Manager Console" yolunu izleyin.

Açılan konsol ekranına "Update-Database" komutunu yazıp Enter tuşuna basın. Bu işlem, gerekli veritabanını ve tabloları SQL Server üzerinde otomatik olarak oluşturacaktır.

Adım 3: API Anahtarı Kontrolü Yapay zeka modülünün çalışması için Google API anahtarı sisteme tanımlanmıştır. Controllers klasörü altındaki "YapayZekaController.cs" dosyasında API anahtarının tanımlı olduğu görülebilir. Herhangi bir işlem yapılmasına gerek yoktur, ancak kendi anahtarınızı kullanmak isterseniz bu alandan değiştirebilirsiniz.

Adım 4: Projenin Başlatılması Visual Studio üzerindeki "IIS Express" veya yeşil başlat butonuna basarak projeyi tarayıcıda çalıştırabilirsiniz.



5. SİSTEME GİRİŞ BİLGİLERİ Proje ilk kez çalıştırıldığında, veritabanına otomatik olarak bir yönetici (Admin) hesabı tanımlanır. Yönetici paneline erişmek için aşağıdaki bilgileri kullanabilirsiniz:

Admin E-Posta: b221210371@sakarya.edu.tr

Admin Şifre: sau

Not: Normal üye girişi yapmak veya sistemi test etmek için, giriş ekranındaki "Kayıt Ol" butonunu kullanarak yeni bir üyelik oluşturabilirsiniz.



6. GELİŞTİRİCİ BİLGİLERİ Adı Soyadı: Aybüke Gökçe Yavaş 
Öğrenci Numarası: B221210371 Bölüm: 
Bilgisayar Mühendisliği