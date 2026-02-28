using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BagController : MonoBehaviour
{
    [SerializeField] private Transform bag;
    public List<ProductData> productDataList = new List<ProductData>();
    private Vector3 productSize = Vector3.zero;
    
    [Header("Capacity & UI")]
    [SerializeField] private TextMeshPro maxText; 
    [SerializeField] private int maxBagCapacity = 5;

    void Start()
    {
        SetMaxTextOff(); 
    }



  private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("ShopPoint"))
    {
        // 1. Önce paraları veriyoruz (Liste hala doluyken)
        for (int i = 0; i < productDataList.Count; i++)
        {
           SellProductToShop(productDataList[i]); 
        }
        // 2. Paraları aldık, şimdi listeyi silebiliriz
        productDataList.Clear();
        // 3. Görselleri yok et (Senin mevcut döngün)
        int childCount = bag.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(bag.GetChild(i).gameObject);
        }
        
        SetMaxTextOff();
        Debug.Log("Çanta boşaltıldı ve paralar alındı.");
    }
}
// 4. Çağırdığın o fonksiyonu buraya tanımlıyoruz ki CashManager ile konuşsun
private void SellProductToShop(ProductData data)
{
    if (CashManager.instance != null)
    {
        CashManager.instance.ExChangeProduct(data);
    }


}


    public void AddProductToBag(ProductData productData)  
    {

         //Eğer datalist Maxbagden büyük veya eşitse aşağıdaki kodları çalıştırma dön
        if (productDataList.Count >= maxBagCapacity) return; 


        //bura ise üsstteki şart sağlanırsa assetler ınstantie edilip kasalara spawnlanır.
        GameObject boxProduct = Instantiate(productData.productPrefab, Vector3.zero, Quaternion.identity);
        
        CalculateObjectSize(boxProduct);
        float yPosition = CalculateNewPositionOfBox();
        
        boxProduct.transform.SetParent(bag, false);
        boxProduct.transform.localRotation = Quaternion.identity;
        boxProduct.transform.localPosition = new Vector3(0, yPosition, 0);

        Collider[] colliders = boxProduct.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        productDataList.Add(productData);
        ControlBagCapacity();
    }

   


    private float CalculateNewPositionOfBox()
    {
        return productSize.y * productDataList.Count;
    }

    private void CalculateObjectSize(GameObject targetObj)
    {
        if (productSize == Vector3.zero)
        {
            MeshRenderer renderer = targetObj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                productSize = renderer.bounds.size;
            }
        }
    }


    public void ControlBagCapacity() //kapas,te kontrolü
    {
        if (productDataList.Count >= maxBagCapacity)
        {
            SetMaxTextOn();
        }
        else
        {
            SetMaxTextOff();
        }
    }


    private void SetMaxTextOn() //maxtextin görünme kodu
    {
        if (maxText != null)
        {
            maxText.gameObject.SetActive(true);
        }
    }

    private void SetMaxTextOff()//maxtextin görünmeme kodu
    {
        if (maxText != null)
        {
            maxText.gameObject.SetActive(false);
        }
    }
      public bool IsEmptySpace()  //çantada boş yer varsa dataliste git ve ürün al
    {
        return productDataList.Count < maxBagCapacity;
    }
}

