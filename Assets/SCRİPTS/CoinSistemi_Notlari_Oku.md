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

## 🥐 Pastane (Bakery) Sistemi

### Özet: Ne Yaptık?
Oyuna bir pastane binası (3D asset) ve baca ekledik. Pastanenin önüne gelen oyuncu, sırtındaki ürünleri (domates gibi) otomatik olarak teslim eder. Bina üstünde bir 3D TextMeshPro ile "3/5 domates teslim edildi" gibi bir gösterge çalışır.

Bunun için iki yeni yapı kuruldu:
1. **`UnlockBakeryUnitController`** → Pastaneyi yönetir (ne istiyor, kaç depoladı)
2. **`BagController`'da yeni bir trigger bloğu** → Oyuncu pastaneye girince çantayı işler

---

### Kod: Nasıl Çalışıyor?

**Adım 1 — Oyuncu pastaneye girer:**
`BagController.OnTriggerEnter` tetiklenir. Tag kontrolü: `"UnlockBakeryUnit"`

**Adım 2 — Pastane ne istiyor?**
```csharp
ProductData.ProductType neededType = bakeryunit.GetNeededProductType();
// → Örneğin: ProductData.ProductType.tomato
```

**Adım 3 — Çantayı tara (tersine döner, silme sırası bozulmasın):**
```csharp
for (int i = productDataList.Count - 1; i >= 0; i--)
{
    if (productDataList[i].productType == neededType)
    {
        if (bakeryunit.StoreProduct()) // pastanede yer var mı?
        {
            Destroy(bag.transform.GetChild(i).gameObject); // görseli sil
            productDataList.RemoveAt(i);                   // veriyi sil
        }
    }
}
```
*(Liste **tersine** dönülür çünkü RemoveAt ile eleman silinince indeksler kayar, tersine gidince bu sorun olmaz.)*

**Adım 4 — StoreProduct() ne yapar?**
```csharp
public bool StoreProduct()
{
    if (maxStoredProductCount == StoredProductCount) return false; // dolu
    StoredProductCount++;    // sayacı artır
    DisplayProductCount();   // TMP'yi güncelle → "3/5"
    return true;
}
```

### Unity Inspector Ayarları
1. Pastane objesine `"UnlockBakeryUnit"` tag'i verilmeli
2. `UnlockBakeryUnitController` script pastane objesine atılmalı
3. `bakerytext` alanına 3D TMP sürüklenmeli
4. `maxStoredProductCount` → kaç ürün alacak (örn. 5)
5. `productType` → Inspector'dan seçilmeli (tomato vb.)

---

## 🪙 Coin Spawn Sorunu ve Çözümü (coinGO Null Hatası)

### Sorun Neydi?
`UnlockBakeryUnitController`'da `coinGO` alanına Inspector'dan **Hierarchy'deki sahne coin objesini** atamıştık.
Bu obje mavi görünüyordu ama aslında bir **sahne instance**'ıydı (gerçek prefab değil).

**Hata zinciri:**
1. Pastane coin üretiyor → `Instantiate(coinGO)` ile klon oluşturuyor ✅
2. Oyuncu sahne coin'ini topluyor → `Destroy(gameObject)` çalışıyor ❌
3. `coinGO` referansı yok oluyor → ikinci turda `coinGO` null oluyor
4. `MissingReferenceException` hatası

### Fark Nedir? (Sahne Objesi vs Prefab)
| | Sahne Objesi (❌) | Project Prefab (✅) |
|---|---|---|
| Konumu | Hierarchy'de görünür | Sadece Project panelinde |
| Silinebilir mi? | Evet, `Destroy()` ile | Hayır, oyun sırasında silinemez |
| `Instantiate` sonrası | Klon + orijinal sahada | Sadece klon sahada |

### Çözüm: `Resources.Load` Kullanımı
1. Coin prefabı `Assets/Resources/Coin.prefab` olarak konumlandırıldı
2. `Awake()`'te `coinGO.scene.IsValid()` ile sahne objesi mi diye kontrol edildi
3. Sahne objesi atanmışsa → `Resources.Load<GameObject>("Coin")` ile gerçek prefab yüklendi

```csharp
void Awake()
{
    // Sahne objesi mi? → Resources'dan yükle
    if (coinGO == null || coinGO.scene.IsValid())
    {
        coinGO = Resources.Load<GameObject>(coinPrefabName);
    }
    _coinCache = coinGO; // Yedek referans
}
```

> **Not:** `coinPrefabName` Inspector'dan değiştirilebilir (varsayılan: `"Coin"`).
> Prefab mutlaka `Assets/Resources/` klasöründe olmalı!

### 3D TextMeshPro Sönük/Işıktan Etkilenme Sorunu
**Sorun:** 3D TMP ekranda soluk/sönük beyaz görünüyor, ışıklandırmadan etkileniyor.
**Çözüm:** TMP objesindeki Material'in Shader'ını değiştir:
`TextMeshPro/Distance Field` → `TextMeshPro/Distance Field Overlay`
Overlay shader ışıktan etkilenmez, metin her zaman parlak ve net görünür.

### CharacterController Havada Durma Sorunu
**Sorun:** Play'e basınca karakter hafifçe yerden yüksekte duruyor.
**Çözüm:** CharacterController → `Center Y` değerini `Height / 2`'ye eşitle.
Örn: Height = 1.8 → Center Y = 0.9 ~ 1.01 arası dene.
**Not:** Center X ve Z değerleri modelin pivot offset'ine göre ayarlıdır, sıfırlamaya çalışma!

### Step Offset / Yokuş Hissi
CharacterController düz zeminde bile bazen "tümsek" hissi verir. Bu `Step Offset` değerinden kaynaklanır (0.3 altı tutulması önerilir). Rahatsız ediyorsa düşür.
