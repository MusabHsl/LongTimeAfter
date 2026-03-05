using Unity.VisualScripting;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    private int coins;
    public static CashManager instance;

    private string keyCoins="keyCoins";

    private void   Awake() 
    {
        if(instance==null)
        {
            instance=this;
        }
        else
        {
        Destroy(instance);
        }

    }

    public void ExChangeProduct(ProductData productData)
    {
        AddCoin(productData.productPrice);
    }

    private void SpendCoin(int price)
    {
        coins-=price;
        DisplayCoins();
    }

    public int GetCoins()
    {
        return coins;
    }

    public bool TryBuyThisUnit(int price) //Para harca 
    {
        if(GetCoins()>=price)
        {
           SpendCoin(price);
           return true;
        }
        return false;
    }

    public void AddCoin(int price)
    {
        coins += price;
        Debug.Log("cash" + coins);
        DisplayCoins();

        // AudioManager varsa toplama sesini çal
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayCollectSound();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadCash();
        DisplayCoins();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DisplayCoins()
    {
        UIManager.instance.ShowCoinCountOnScreen(coins);
        SaveCash();
    }

    private void LoadCash()
    {
        coins=PlayerPrefs.GetInt(keyCoins,0);
    }

private void SaveCash()
    {
        PlayerPrefs.SetInt(keyCoins,coins);
    }
}
