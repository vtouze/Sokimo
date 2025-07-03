using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] private TMP_Text coinText;
    private int currentCoins = 0;

    private void Awake()
    {
        Instance = this;
        UpdateCoinText();
    }

    public void AddCoin()
    {
        currentCoins++;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
        else
            Debug.LogWarning("Coin Text is not assigned in the inspector.");
    }
}