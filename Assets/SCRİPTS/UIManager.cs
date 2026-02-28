using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI coinCountText;

    private void Awake()
    {
        // Singleton Yapısı
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCoinCountOnScreen(int coins)
    {
        if (coinCountText != null)
        {
            coinCountText.text = coins.ToString();
        }
    }
}
