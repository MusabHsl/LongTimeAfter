using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class BagController : MonoBehaviour
{
    [SerializeField] private Transform bag;
    public List<ProductData>productDataList;
    private Vector3 productSize;
    

    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

   private void OnTriggerEnter(Collider other)
{
    Debug.Log("Trigger çalıştı: " + other.tag); // ← BU SATIRI EKLE
    if(other.CompareTag("ShopPoint"))
    {
        Debug.Log("ShopPoint'e çarpıldı!"); // ← BU SATIRI DA EKLE
        for(int i = productDataList.Count-1; i>=0; i--)
        {
            Destroy(bag.transform.GetChild(i).gameObject);
            productDataList.RemoveAt(i);
        }
    }
}

    public void AddProductToBag(ProductData productData)
    {
        GameObject boxProduct= Instantiate(productData.productPrefab,Vector3.zero,Quaternion.identity);
        boxProduct.transform.SetParent(bag,true);

        CalculateObjectSize(boxProduct);
        float yPosition=CalculatNewPositionOfBox();
        boxProduct.transform.SetParent(bag, true);
        boxProduct.transform.localRotation = Quaternion.identity; // Sırtımızda yamuk durmaması için.
        boxProduct.transform.localPosition = Vector3.zero; // Sırtımızda yamuk durmaması için.
        boxProduct.transform.localPosition=new Vector3(0,yPosition,0);
        productDataList.Add(productData);
    }
    private float CalculatNewPositionOfBox()
    {
    float newYpos=productSize.y * productDataList.Count;
    return newYpos;
    }

    private void CalculateObjectSize(GameObject gameObject)
    {
        if(productSize==Vector3.zero)
        {
            MeshRenderer renderer=gameObject.GetComponent<MeshRenderer>();
            productSize=renderer.bounds.size;
        }
    }

}
