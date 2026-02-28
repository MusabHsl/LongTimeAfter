using Unity.VisualScripting;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    private int coins;
    public static CashManager instance;

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

    public void AddCoin(int price)
    {
        coins+=price;
        Debug.Log("cash"+coins);
        DisplayCoins();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DisplayCoins()
    {
        UIManager.instance.ShowCoinCountOnScreen(coins);
    }
}
