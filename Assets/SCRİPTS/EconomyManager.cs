using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Economy Settings")]
    [SerializeField] private int totalCoins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
        Debug.Log("Yeni Coin Miktarı: " + totalCoins);
    }

    private void UpdateCoinUI()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowCoinCountOnScreen(totalCoins);
        }
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
}
