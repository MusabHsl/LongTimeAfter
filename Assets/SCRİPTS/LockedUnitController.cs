using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LockedUnitController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string unitID; // Müfettişten her ünite için benzersiz bir isim verin (Örn: "Farm1", "Bakery")
    [SerializeField] private int price;
    [Header("Objects")]
    [SerializeField] private TextMeshPro pricetext;
    [SerializeField] private GameObject lockedunit;
    [SerializeField] private GameObject unlockedunit;

    private bool isPurchased;

    void Start()
    {
        pricetext.text = price.ToString();
        LoadStatus();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !isPurchased)
        {
            UnlockUnit();
        }
    }

    private void UnlockUnit()
    {
        if(CashManager.instance.TryBuyThisUnit(price))
        {
            Unlock();
            SaveStatus();
            
            // AudioManager varsa kilidi açma sesini çal
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayUnlockSound();
            }
        }
    }

    private void Unlock()
    {
        isPurchased = true;
        unlockedunit.SetActive(true);

        foreach (Transform child in unlockedunit.transform)
        {
            child.gameObject.SetActive(true);
        }

        lockedunit.SetActive(false);
    }

    private void SaveStatus()
    {
        PlayerPrefs.SetInt(unitID + "_Purchased", 1);
        PlayerPrefs.Save();
        Debug.Log("Ünite Kaydedildi: " + unitID + " - Kayıt Adı: " + unitID + "_Purchased");
    }

    private void LoadStatus()
    {
        if (string.IsNullOrEmpty(unitID))
        {
            Debug.LogWarning(gameObject.name + " için unitID atanmamış! Kaydetme çalışmayacak.");
            return;
        }

        int status = PlayerPrefs.GetInt(unitID + "_Purchased", 0);
        Debug.Log("Ünite Yükleniyor: " + unitID + " - Durum: " + status);

        if (status == 1)
        {
            Unlock();
            Debug.Log(unitID + " otomatik olarak açıldı.");
        }
    }
}
