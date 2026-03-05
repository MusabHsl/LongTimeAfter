using TMPro;
using UnityEngine;


//Pastane unlock edildikten sonra domateslerin toplanma işlemi ve coin artması
public class UnlockBakeryUnitController : MonoBehaviour
{
    [SerializeField] private TextMeshPro bakerytext;
    [SerializeField] private int maxStoredProductCount;
    [SerializeField] private ProductData.ProductType productType;

    [SerializeField] private int UseProductInSeconds=5;
    [SerializeField] private Transform coinTransform;

    [SerializeField] private GameObject coinGO;
    [SerializeField] private string coinPrefabName = "Coin"; // Resources klasöründeki prefab adı

    [SerializeField] private ParticleSystem smokeParticle;

    private float time;
    private int StoredProductCount;
    private GameObject _coinCache; // coinGO'nun yedeği

    void Awake()
    {
        // Eğer coinGO null veya sahne objesi ise → Resources'dan yükle
        if (coinGO == null || coinGO.scene.IsValid())
        {
            coinGO = Resources.Load<GameObject>(coinPrefabName);
            if (coinGO == null)
                Debug.LogError("Coin prefab '" + coinPrefabName + "' Resources klasöründe bulunamadı!");
            else
                Debug.Log("coinGO Resources'dan yüklendi: " + coinGO.name);
        }
        _coinCache = coinGO;
    }

    void Start()
    {
        DisplayProductCount();
        SmokeControllerEffects();
    }

    void Update()
    {
        if(StoredProductCount>0)
        {
           time += Time.deltaTime;

        }

        if(time>=UseProductInSeconds)
        {
            time=0.0f;
            UseProduct();
        }
    }

    private void DisplayProductCount()
    {
        bakerytext.text = StoredProductCount.ToString() + "/" + maxStoredProductCount.ToString();
        SmokeControllerEffects();
    }

    // Pastane bizden hangi ürünleri bekliyor
    public ProductData.ProductType GetNeededProductType()
    {
        return productType;
    }

    // Pastanede depolayacak yer var mı?
    public bool StoreProduct()
    {
        if (maxStoredProductCount == StoredProductCount)
            return false;

        StoredProductCount++;
        DisplayProductCount();
        return true;
    }

    private void UseProduct()
    {
        StoredProductCount--;
        DisplayProductCount();
        createcoin();
    }

    private void createcoin()
    {
        // coinGO null ise yedek cache'i dene
        GameObject prefabToSpawn = coinGO != null ? coinGO : _coinCache;
        if(prefabToSpawn == null)
        {
            Debug.LogError("coinGO ve cache ikisi de null! Sahne objesi mi atandı?");
            return;
        }
        Vector3 position = UnityEngine.Random.insideUnitSphere * 1f;
        Vector3 InstiatePos = coinTransform.position + position;
        Instantiate(prefabToSpawn, InstiatePos, Quaternion.identity);
    }


    private void SmokeControllerEffects()
    {
        if(StoredProductCount==0)
        {
            if(smokeParticle.isPlaying)
            {
                smokeParticle.Stop();
            }
        }
        else
        {
            if(smokeParticle.isStopped)
            {
                smokeParticle.Play();
            }
        }


    }
}
