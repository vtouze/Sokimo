using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] private TMP_Text coinText;

    private int currentCoins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes

        currentCoins = PlayerPrefs.GetInt("Coins", 0); // Load saved coins
        UpdateCoinText();
    }

    public int CurrentCoins => currentCoins;

    public void AddCoin(int amount = 1)
    {
        currentCoins += amount;
        SaveCoins();
        UpdateCoinText();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
            return false;

        currentCoins -= amount;
        SaveCoins();
        UpdateCoinText();
        return true;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }

    public void RegisterCoinText(TMP_Text text) // Optional for HUD/shop UIs
    {
        coinText = text;
        UpdateCoinText();
    }

    void ResetCoinPrefs()
    {
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.Save();
    }
}