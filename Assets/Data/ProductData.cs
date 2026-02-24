using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [CreateAssetMenu(fileName = "Product Data", menuName = "Scriptable Object/Product Data", order = 0)]
public class ProductData : ScriptableObject
{

    
    public enum ProductType{tomato,cabbage}
    public GameObject productPrefab;
    public ProductType productType;

    public int productPrice;
}
