using TMPro;
using UnityEngine;

//Pastane unlock edildikten sonra domateslerin toplanma işlemi ve coin artması
public class UnlockBakeryUnitController : MonoBehaviour
{
    [SerializeField] private TextMeshPro bakerytext;
    [SerializeField] private int maxStoredProductCount;
    [SerializeField] private ProductData.ProductType productType;

    private int StoredProductCount;

    void Start()
    {
        DisplayProductCount();
    }

    void Update()
    {
        
    }

    private void DisplayProductCount()
    {
        bakerytext.text = StoredProductCount.ToString() + "/" + maxStoredProductCount.ToString();
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
}
