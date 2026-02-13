using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class BagController : MonoBehaviour
{
    [SerializeField] private Transform bag;
    public List<GameObject>productList;
    private Vector3 productSize;
    

    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            Debug.Log("Kube carpildi");
            AddProductToBag(other.gameObject); 
        }
    }

    public void AddProductToBag(GameObject product)
    {
        GameObject boxProduct= Instantiate(product,Vector3.zero,Quaternion.identity);
        boxProduct.transform.SetParent(bag,true);

        CalculateObjectSize(boxProduct);
        float yPosition=CalculatNewPositionOfBox();
        boxProduct.transform.SetParent(bag, true);
        boxProduct.transform.localRotation = Quaternion.identity; // Sırtımızda yamuk durmaması için.
        boxProduct.transform.localPosition = Vector3.zero; // Sırtımızda yamuk durmaması için.
        boxProduct.transform.localPosition=new Vector3(0,yPosition,0);
        productList.Add(boxProduct);
    }
    private float CalculatNewPositionOfBox()
    {
    float newYpos=productSize.y * productList.Count;
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
