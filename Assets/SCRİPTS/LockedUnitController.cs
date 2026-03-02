using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LockedUnitController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int price;
    [Header("Objects")]
    [SerializeField] private TextMeshPro pricetext;
    [SerializeField] private GameObject lockedunit;
    [SerializeField] private GameObject unlockedunit;

    private bool isPurchased;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pricetext.text=price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !isPurchased)
        {
            //ürünü paran yeterse al
            UnlockUnit();
        }
    }

    private void UnlockUnit()
    {
        if(CashManager.instance.TryBuyThisUnit(price))
        {
            Unlock();
        }
        //parası varsa kontrol et
        //varsa aç
    }

    private void Unlock()
    {
        isPurchased = true;
        Debug.Log("Unlock çağrıldı! unlockedunit: " + unlockedunit.name);

        // UnlockedObjects'i aç
        unlockedunit.SetActive(true);

        // Child'lar direkt kapatılmış olabilir, hepsini aç
        foreach (Transform child in unlockedunit.transform)
        {
            child.gameObject.SetActive(true);
            Debug.Log("Child açıldı: " + child.name);
        }

        // En son LockedObjects'i kapat
        lockedunit.SetActive(false);
    }
}
