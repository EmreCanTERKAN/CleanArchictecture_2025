# Clean Architecture Proje Şablonu

Bu proje, **Clean Architecture** prensipleri temel alınarak geliştirilmiş bir şablondur. 
Aşağıdaki mimari yapı, tasarım desenleri ve kütüphaneler kullanılarak oluşturulmuştur.

---

## 📐 Mimari Yapı

### **Mimari Desen**
- **Clean Architecture**: Uygulama katmanları aşağıdaki şekilde organize edilmiştir:
  - **Domain**: İş mantığı ve temel varlıklar.
  - **Application**: İş kuralları, CQRS ve MediatR işlemleri.
  - **Infrastructure**: Veritabanı, harici servis entegrasyonları.
  - **Presentation**: API endpoint'leri ve kullanıcı arayüzü.

### **Tasarım Desenleri**
- **Result Pattern**: İşlem sonuçlarını standart bir yapıda döndürmek için.
- **Repository Pattern**: Veritabanı erişimini soyutlamak için.
- **CQRS Pattern**: Komut ve sorgu işlemlerini ayrıştırmak için.
- **UnitOfWork Pattern**: İşlemleri atomik olarak yönetmek için.

---

## 📚 Kullanılan Kütüphaneler

### **Temel Kütüphaneler**
- **MediatR**: CQRS ve mesajlaşma işlemleri için.
- **TS.Result**: Standart sonuç modellemeleri ve hata yönetimi için.
- **Mapster**: Nesneler arası kolay eşleme (mapping) için.
- **FluentValidation**: Gelen isteklerin doğrulanması için.

### **Veritabanı ve ORM**
- **EntityFrameworkCore**: ORM (Nesne-İlişkisel Eşleme) için.
- **TS.EntityFrameworkCore.GenericRepository**: Genel repository işlemleri için.

### **API ve Sorgulama**
- **OData**: Esnek veri sorgulama ve filtreleme için.
- **Scrutor**: Dinamik servis kaydı ve Dependency Injection yönetimi için.

---
### **Adımlar**
1. **Depoyu Klonlayın**:
   ```bash
   git clone https://github.com/your-repo-url.git