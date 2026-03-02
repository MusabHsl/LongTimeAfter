# 🪙 Coin Sistemi ve Mimari Notları

Bu not defteri, oyuna eklediğimiz coin sisteminin nasıl çalıştığını, hangi scriptin ne işe yaradığını ve kullandığımız teknik terimlerin ne anlama geldiğini özetler.

---

## 🏗️ Sistemin Üç Atlısı (Mimarimiz)

Oyunumuzda paranın akışını denetleyen 3 temel parça var:

### 1. UIManager (Görsel Sorumlusu)
*   **Görevi:** Sadece ekrandaki yazıyı (TextMeshPro) günceller. Matematiksel hesap yapmaz, sadece kendisine gelen sayıyı ekrana yansıtır.
*   **Neden UI TMP?** Çünkü bu yazı telefon camına yapışmış bir etiket gibi, oyuncu nereye bakarsa baksın hep sağ üstte sabit durur.

### 2. CashManager (Kasa Sorumlusu)
*   **Görevi:** Toplam parayı hafızasında tutar. `AddCoin` fonksiyonu çağrıldığında parayı artırır ve `UIManager`'a "Hey, yeni parayı ekrana yaz!" der.
*   **Özelliği (Singleton):** Bu scriptte `public static CashManager instance` yapısını kullandık. Bu sayede diğer scriptlerden (örneğin BagController) kasaya ulaşmak için "sürükle-bırak" yapmaya gerek kalmaz. Doğrudan `CashManager.instance` yazarak ulaşabiliriz.

### 3. BagController (İşçi Sorumlusu)
*   **Görevi:** Ürünleri toplar ve satış noktasına (`ShopPoint`) gidince satışı başlatır.
*   **Kritik Satış Mantığı:** 
    1.  Önce listeyi döner ve `CashManager`'a her ürün için para ekletir.
    2.  Hemen ardından listeyi (`Clear`) temizler.
    3.  En son görselleri siler (`Destroy`).
    *(Not: Önce silseydik, ne kadar para ekleyeceğimizi bilemezdik!)*

---

## 📚 Terimler Sözlüğü

*   **Singleton:** "Tek bir merkezden yönetim." Bir scriptten sahne boyunca sadece bir tane olmasını ve ona her yerden (örneğin `CashManager.instance` diyerek) kolayca ulaşılmasını sağlar.
*   **UI TextMeshPro (Normal):** Canvas içinde olan, ekrana yapışık duran yazılar. (Coin sayısı vb.)
*   **3D TextMeshPro:** Oyun dünyasında bir nesnenin üstünde duran, uzaklaşınca küçülen yazılar. (Karakterin kafasındaki MAX yazısı vb.)
*   **OnTriggerEnter:** Bir objenin (Player) başka bir alana (ShopPoint) girdiği anı yakalayan fonksiyondur.

---

## 🛠️ Unity İçinde Unutmaman Gerekenler
1.  Sahnede `UIManager` ve `CashManager` isminde boş objeler olmalı ve scriptler üstlerinde olmalı.
2.  `UIManager`'ın içindeki kutucuğa **UI Text (TMP)** nesnesi sürüklenmiş olmalı.
3.  `ProductData` dosyalarında `productPrice` değeri sıfırdan büyük olmalı.

Bu sistem sayesinde oyunun ekonomisi tıkır tıkır işleyecektir! 🚀

---

## 🗓️ 02.03.2026 - Bugün Ne Yaptık?

### 📋 Genel Sıralama (Ne Yaptık?)

1. **Sahnede kilitli satın alma alanları oluşturduk** — Lahana ve domates tarlaları için `CabbageLockedUnit` ve `TomatoLockedUnit` objeleri oluşturuldu. Her birinin altına `LockedObjects` (çerçeve, coin ikonu, fiyat yazısı) ve `TriggerZone` (collider + script) eklendi.

2. **CashManager'a para harcama sistemi ekledik** — `SpendCoin` ve `TryBuyThisUnit` metodları yazıldı. Player trigger'a girince coin yeterliyse harcama yapılacak şekilde `LockedUnitController` scripti oluşturuldu.

3. **CashManager'daki 3 hatayı düzelttik** — Yeni yazılan metodlarda syntax ve mantık hataları vardı, tek tek giderildi.

4. **TriggerZone'u sahnede konumlandırdık** — Boş bir GameObject, Box Collider (Is Trigger ✅) eklendi. Dükkânın önüne yerleştirildi. OnTriggerEnter'ın yalnızca Collider'ın üzerinde olduğu objede çalıştığını öğrendik, script TriggerZone'a taşındı.

5. **Satın alma çalıştı — coin azaldı, LockedObjects kapandı** ✅

6. **UnlockedObjects içindeki tarlalar görünmedi — hata avcılığı yaptık** — Debug.Log ekleyerek script'in çalıştığını doğruladık. Sorunun Unity SetActive kuralları ve hiyerarşi konumundan kaynaklandığını bulduk.

---

### 🔧 Düzeltilen Hatalar

**CashManager.cs — 3 hata:**

| # | Hata | Çözüm |
|---|------|--------|
| 1 | `SpendCoin()` içinde `price` parametresi yoktu | `SpendCoin(int price)` yapıldı |
| 2 | `GetCoins()` metodu yoktu | `public int GetCoins()` eklendi |
| 3 | `TryBuyThisUnit` bazı durumlarda `bool` dönmüyordu | `if` dışına `return false` eklendi |

Ayrıca `Destroy(instance)` → `Destroy(gameObject)` düzeltildi (Singleton'da yanlış obje yok ediliyordu).

---

### 🔑 Bugün Öğrenilen Kritik Unity Kuralları

**1. Child kapatmak ≠ Parent kapatmak**
- Parent'ı kapatmak → tüm child'ları da kapatır ✅
- Parent'ı açmak → kendi başına kapatılmış child'ları AÇMAZ ❌
- **Kural:** Her zaman child'ı açık bırak, sadece parent'ı kapat/aç!

**2. SetActive sırası önemli!**
- TriggerZone LockedObjects'in içindeyse; LockedObjects kapanınca TriggerZone da kapanır, script yarım kalır.
- **Kural:** Önce UnlockedObjects'i aç (`SetActive(true)`), sonra LockedObjects'i kapat!

**3. OnTriggerEnter nerede tetiklenir?**
- Sadece Collider'ın bulunduğu objede çalışır. Script ve Collider aynı objede olmalı!

---

### ⚠️ Çözülemeyen / Sonraya Bırakılan Problem
- `UnlockedObjects`, `CabbageLockedUnit` içine alınınca tarlalar görünmüyor.
- **Geçici durum:** Şimdilik çalışır halde bırakıldı, prefab konusu sonraya bırakıldı.
- **Yapılacak:** `lockedunit` referansının tam olarak neye bağlı olduğu kontrol edilecek (LockedObjects mı, CabbageLockedUnit mi?).
