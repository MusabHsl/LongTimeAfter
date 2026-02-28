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
