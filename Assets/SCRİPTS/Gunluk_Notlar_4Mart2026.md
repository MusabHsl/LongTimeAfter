# 📓 Günlük Notlar — 4 Mart 2026

Bu dosya, 4 Mart 2026 günü LongTimeAfter projesinde yapılan tüm geliştirmeleri,
**nasıl yapıldıklarını** ve **neden yapıldıklarını** adım adım açıklar.
Antigravity (AI asistanı) ile birlikte çalışarak gerçekleştirilen değişikliklerdir.

---

## � Günde Ne Yaptık? (Hızlı Özet)

1. **AudioManager scripti yazıldı** — Oyuna merkezi bir ses sistemi eklendi. Müzik ve efekt sesleri ayrı kanallardan çalıyor.
2. **Ünite açılırken ses eklendi** — Oyuncu bir tarla veya bina satın alınca artık sesli geri bildirim var.
3. **Sahne sınırlandırıldı** — Oyuncu haritanın sonsuzluğuna kaçamıyor; görünmez collider duvarlar sahneye yerleştirildi.
4. **Baca particle efekti eklendi** — Pastanede ürün varken bacadan duman çıkıyor, ürün bitince duruyor; bu durum kodla kontrol ediliyor.
5. **Pastaneden coin sistemi kuruldu** — Oyuncu domates teslim edince pastane belirli sürede bir coin üretiyor; `MissingReferenceException` hatası giderildi.
6. **PlayerPrefs kayıt/yükleme sistemi eklendi** — Satın alınan üniteler oyun kapansa bile açık kalıyor; geliştirici için R tuşu ile sıfırlama aracı eklendi.

---

## 🗂️ Dosya Tablosu

| # | Başlık | İlgili Dosya |
|---|--------|-------------|
| 1 | 🔊 AudioManager — Merkezi Ses Sistemi | `AudioManager.cs` (YENİ) |
| 2 | 🔓 Ünite Açılırken Ses Entegrasyonu | `LockedUnitController.cs` |
| 3 | 🗺️ Sahne Sınırlandırma | Unity Inspector |
| 4 | 💨 Baca Particle Efekti | `UnlockBakeryUnitController.cs` |
| 5 | 🍅 Pastane → Coin Sistemi | `UnlockBakeryUnitController.cs` |
| 6 | 💾 PlayerPrefs: Kayıt / Yükleme | `LockedUnitController.cs`, `GameManager.cs` (YENİ) |

---

## 1. 🔊 AudioManager — Merkezi Ses Sistemi

### Problem Neydi?
Oyunda hiç ses yoktu. Ürün toplarken, ünite açarken hiçbir ses çalmıyordu.
Her objeye ayrı ayrı `AudioSource` eklersek yönetmek zorlaşırdı.

### Ne Yaptık?
Tüm oyun seslerini tek bir yerden yöneten **AudioManager** adında yeni bir script yazdık.

**Tasarım Kararı — Singleton:**
Scriptte `public static AudioManager instance` yapısını kullandık.
Bu sayede sahnede sadece **bir tane** AudioManager olur ve her script `AudioManager.instance.PlayXxx()` diyerek kolayca erişir. Sürükle-bırak bağlantısına gerek kalmaz.

**DontDestroyOnLoad:**
```csharp
DontDestroyOnLoad(gameObject);
```
Sahne değişse bile AudioManager yok olmaz. Müzik kesilmeden devam eder.

**İki Ayrı AudioSource — Neden?**
```
musicSource  → Arka plan müziği (sürekli çalar, loop açık)
sfxSource    → Efekt sesleri (PlayOneShot ile çalar, üst üste binebilir)
```
Ayrı olmasının sebebi: Müziği alçaltırken efekt seslerini etkilememek (ses ayarları için).

**Kullanıma Hazır Kısayol Fonksiyonlar:**
```csharp
public void PlayCollectSound() => PlaySFX(collectSFX);  // Ürün toplarken
public void PlayUnlockSound()  => PlaySFX(unlockSFX);   // Ünite açarken
```

### Unity'de Nasıl Ayarlanır?
1. Sahnede boş bir obje oluştur → Adı: `AudioManager`
2. `AudioManager.cs` scriptini ekle
3. Objeye **iki** `AudioSource` component'i ekle
4. Inspector'da `musicSource` ve `sfxSource` alanlarına bu iki AudioSource'u ata
5. `backgroundMusic`, `collectSFX`, `unlockSFX` alanlarına `.wav` / `.mp3` ses dosyalarını sürükle

---

## 2. 🔓 Ünite Açılırken Ses Entegrasyonu

### Problem Neydi?
Oyuncu bir tarla ya da pastane satın aldığında hiçbir ses çalmıyordu.
Satın alma anı sessiz geçiyordu, geri bildirim yoktu.

### Ne Yaptık?
`LockedUnitController.cs` içindeki `UnlockUnit()` metoduna AudioManager çağrısı ekledik:

```csharp
private void UnlockUnit()
{
    if (CashManager.instance.TryBuyThisUnit(price))
    {
        Unlock();       // Görseli aç
        SaveStatus();   // Kaydet

        // 🔊 Ses çal
        if (AudioManager.instance != null)
            AudioManager.instance.PlayUnlockSound();
    }
}
```

**Neden `null` kontrolü yaptık?**
Eğer AudioManager sahnede yoksa (test sahnesinde vb.) hata alınmaması için.
Bu şekilde ses sistemi olmadan da oyun çalışmaya devam eder.

**`Unlock()` metodu ne yapar?**
```csharp
private void Unlock()
{
    isPurchased = true;
    unlockedunit.SetActive(true);          // Açık üniteyi göster

    foreach (Transform child in unlockedunit.transform)
        child.gameObject.SetActive(true);  // Tüm alt objeleri de aç

    lockedunit.SetActive(false);           // Kilitli üniteyi gizle
}
```
`foreach` ile alt objeleri açmak önemliydi — çünkü bazı binalar birden fazla parçadan oluşuyor
ve parent objeyi açmak child'ları otomatik açmıyordu.

---

## 3. 🗺️ Sahne Sınırlandırma

### Problem Neydi?
Oyuncu haritanın her yerine gidebiliyordu — sınır yoktu.
Boş alanlara, haritanın dışına çıkmak mümkündü.

### Ne Yaptık?
Bu kısmı **sen** yaptın, Unity Inspector üzerinden:

1. Sahneye 4 taraftan görünmez `Collider` duvarları yerleştirildi
2. Bu duvarların `Renderer` bileşeni kapalı tututldu (görünmez)
3. Sadece `Collider` aktif olduğu için oyuncu çarpar ama duvarı görmez
4. Böylece harita sınırı oluşturuldu

**Alternatif Yöntem (Kod ile):**
İlerleyen geliştirmelerde Camera Confiner ya da düşünülen şu yapı eklenebilir:
```csharp
// PlayerMovement.cs içine eklenebilir
transform.position = new Vector3(
    Mathf.Clamp(transform.position.x, minX, maxX),
    transform.position.y,
    Mathf.Clamp(transform.position.z, minZ, maxZ)
);
```

---

## 4. 💨 Baca Particle Efekti

### Problem Neydi?
Pastane binası duruyordu ama hiç "canlı" hissi vermiyordu.
Ürün teslim edildiğinde bacadan duman çıkması gerekiyordu.

### Ne Yaptık?
`UnlockBakeryUnitController.cs`'ye `ParticleSystem` bağlantısı ve kontrol mantığı ekledik:

```csharp
[SerializeField] private ParticleSystem smokeParticle;

private void SmokeControllerEffects()
{
    if (StoredProductCount == 0)
    {
        if (smokeParticle.isPlaying)
            smokeParticle.Stop();   // Ürün yok → Baca durur 💨❌
    }
    else
    {
        if (smokeParticle.isStopped)
            smokeParticle.Play();   // Ürün var → Baca tüter 💨✅
    }
}
```

**Neden `isPlaying` / `isStopped` kontrol ettik?**
Her Update'te Stop() ya da Play() çağırmak particle'ı resetler.
Bu kontrol sayesinde sadece **durum değiştiğinde** müdahale ediyoruz.

**SmokeControllerEffects nerede çağrılıyor?**
```csharp
void Start()              → Başlangıç durumu
DisplayProductCount()     → Her ürün değişiminde (store/use)
```
`DisplayProductCount()`;, `StoreProduct()` ve `UseProduct()` her ikisi tarafından çağrıldığı için
tek bir yerden yönetim yeterli.

**Null Check Neden Önemli?**
Eğer Inspector'da `smokeParticle` atanmamışsa `NullReferenceException` alırsın.
İlerledikçe null güvenliği ekleyebilirsin:
```csharp
if (smokeParticle != null && smokeParticle.isStopped) smokeParticle.Play();
```

### Unity'de Nasıl Ayarlanır?
1. Pastane objesinin bacası olan alt objesine `Particle System` component'i ekle
2. Particle System ayarlarını duman görünümüne getir (renk: gri/beyaz, hız: yavaş, yukarı çıkış)
3. `UnlockBakeryUnitController`'daki `smokeParticle` alanına bu Particle System'i sürükle

---

## 5. 🍅 Pastaneden Coin — Domates Karşılığı Para

### Problem Neydi?
Oyuncu domatesleri pastaneye teslim ediyordu ama para almıyordu.
Pastane teslim alıyordu, tüketiyordu ama karşılığında coin üretmiyordu.

### Ne Yaptık?
Pastane her `UseProductInSeconds` saniyede bir stoktan ürün tüketerek coin üretiyor.

**Ürün Tüketme ve Coin Oluşturma:**
```csharp
void Update()
{
    if (StoredProductCount > 0)
        time += Time.deltaTime;

    if (time >= UseProductInSeconds)
    {
        time = 0.0f;
        UseProduct(); // Ürünü tüket + coin üret
    }
}

private void UseProduct()
{
    StoredProductCount--;
    DisplayProductCount();
    createcoin(); // ← Coin burada oluşuyor
}

private void createcoin()
{
    Vector3 randomOffset  = Random.insideUnitSphere * 1f;
    Vector3 spawnPosition = coinTransform.position + randomOffset;
    Instantiate(coinGO, spawnPosition, Quaternion.identity);
}
```

### coinGO Null Hatası ve Çözümü

**Asıl Sorun:**
Inspector'dan `coinGO`'ya **Hierarchy'deki** bir coin objesi atanmıştı.
Bu obje sahne coin'iydi — oyuncu onu toplayınca `Destroy()` ile yok oluyordu.
Bir sonraki coin üretiminde `coinGO` null olmuştu → `MissingReferenceException`!

**Çözüm — `Resources.Load` + Sahne Kontrolü:**
```csharp
void Awake()
{
    // Sahne objesi mi atandı? → Güvenli prefabı yükle
    if (coinGO == null || coinGO.scene.IsValid())
    {
        coinGO = Resources.Load<GameObject>(coinPrefabName);
        if (coinGO == null)
            Debug.LogError("Coin prefab bulunamadı!");
    }
    _coinCache = coinGO; // Yedek referans
}
```

**`coinGO.scene.IsValid()` ne demek?**
Eğer `coinGO` bir sahne objesi ise `scene.IsValid()` = `true` döner.
Bu sayede "Görünüşe Göre Prefab Ama Aslında Sahne Objesi" durumunu yakalıyoruz.

**`_coinCache` neden var?**
`coinGO` bir şekilde null olursa yedek referanstan oluşturmaya devam etsin diye.

### Unity'de Nasıl Ayarlanır?
1. Coin prefabını `Assets/Resources/Coin.prefab` olarak kaydet
2. `coinPrefabName` → `"Coin"` (zaten varsayılan, değiştirme)
3. `coinTransform` → Coin spawn noktası (pastane baca konumu boş objesi)
4. `UseProductInSeconds` → Kaç saniyede bir coin üretsin (Inspector'dan ayarla)

---

## 6. 💾 PlayerPrefs: Ünite Kaydetme ve Yükleme

### Problem Neydi?
Oyuncu bir tarla ya da pastane satın aldığında oyunu kapatıp açınca
her şey sıfırlanıyor, ünite tekrar kilitli görünüyordu.
Oyun ilerlemesi kalıcı değildi.

### Ne Yaptık?
`LockedUnitController.cs`'ye `PlayerPrefs` tabanlı save/load sistemi ekledik.

**PlayerPrefs Nedir?**
Unity'nin basit bir anahtar-değer (key-value) depolama sistemi.
Oyuncu bilgisayarında küçük bir dosyaya kaydeder.
String, Int, Float saklayabilirsin.

**Kaydetme:**
```csharp
private void SaveStatus()
{
    PlayerPrefs.SetInt(unitID + "_Purchased", 1); // "Tarla1_Purchased" = 1
    PlayerPrefs.Save(); // Diske yaz (güvenlik için)
    Debug.Log("Ünite Kaydedildi: " + unitID);
}
```

**Yükleme:**
```csharp
private void LoadStatus()
{
    if (string.IsNullOrEmpty(unitID))
    {
        Debug.LogWarning(gameObject.name + " için unitID atanmamış!");
        return;
    }

    int status = PlayerPrefs.GetInt(unitID + "_Purchased", 0); // 0 = varsayılan (kilitli)
    if (status == 1)
        Unlock(); // Daha önce satın alınmış → Otomatik aç
}
```

**`unitID` Neden Önemli?**
PlayerPrefs anahtar-değer ile çalışır. Her ünitenin farklı bir anahtar adı olması gerekir.
Eğer hepsi aynı `unitID`'yi kullanırsa birinin açılması hepsini açar!

### ⚠️ Inspector'da Yapman Gereken Tek Şey
Her `LockedUnit` objesine Inspector'dan benzersiz `unitID` ver:
```
Tarla1_Domates  → "Tarla1"
Tarla2_Lahana   → "Tarla2"
Pastane         → "Bakery"
```

### GameManager — Geliştirici Reset Aracı

Test sırasında "Sanki sıfırdan oynasam ne olur?" diye kontrol etmek gerekiyordu.
`GameManager.cs` adlı yeni bir script yazıldı:

```csharp
// R tuşuna basınca:
// 1. Tüm PlayerPrefs silinir
// 2. Sahne yeniden yüklenir
```

**Input System Notları:**
Proje Unity'nin yeni **Input System** paketini kullandığı için
eski `Input.GetKeyDown(KeyCode.R)` çalışmıyordu.
Yeni sistemde:
```csharp
Keyboard.current.rKey.wasPressedThisFrame
```
şeklinde kontrol yapılıyor. `using UnityEngine.InputSystem;` import'u gerekli.

### Unity'de Nasıl Ayarlanır?
1. Sahnede boş bir obje oluştur → Adı: `GameManager`
2. `GameManager.cs` scriptini ekle
3. Oyun çalışırken **R** tuşuna bas → Reset!

---

## ✅ Kontrol Listesi

- [ ] `AudioManager` objesine ses dosyaları bağlandı mı?
- [ ] Her `LockedUnit`'e benzersiz `unitID` atandı mı?
- [ ] Pastanedeki `smokeParticle` alanına Particle System bağlandı mı?
- [ ] `Coin` prefabı `Assets/Resources/` klasöründe mi?
- [ ] `GameManager` sahnede var mı?
- [ ] Sahne duvarları (sınırlar) test edildi mi?

---

*Bu notlar Antigravity AI asistanı ile birlikte yazılmıştır. 🤖*
