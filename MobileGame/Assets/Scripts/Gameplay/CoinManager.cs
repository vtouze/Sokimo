using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] private TMP_Text coinText;

    private int currentCoins;
    private int sessionCoins = 0;

    private HashSet<string> sessionCollectedCoinIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentCoins = PlayerPrefs.GetInt("Coins", 0);
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
            coinText.text = (currentCoins + sessionCoins).ToString();
    }

    public void RegisterCoinText(TMP_Text text)
    {
        coinText = text;
        UpdateCoinText();
    }

    public void ResetCoinPrefs()
    {
        PlayerPrefs.DeleteKey("Coins");

        foreach (Coin coin in FindObjectsOfType<Coin>())
        {
            if (!string.IsNullOrEmpty(coin.CoinID))
                PlayerPrefs.DeleteKey("CoinCollected_" + coin.CoinID);
        }

        PlayerPrefs.Save();
    }
    public void AddSessionCoin(int amount = 1)
    {
        sessionCoins += amount;
        UpdateCoinText();
    }

    public void CommitSessionCoins()
    {
        currentCoins += sessionCoins;
        SaveCoins();

        foreach (var id in sessionCollectedCoinIDs)
        {
            PlayerPrefs.SetInt("CoinCollected_" + id, 1);
        }

        PlayerPrefs.Save();

        sessionCoins = 0;
        sessionCollectedCoinIDs.Clear();

        UpdateCoinText();
    }

    public void ClearSessionCoins()
    {
        sessionCoins = 0;
        sessionCollectedCoinIDs.Clear();
        UpdateCoinText();
    }

    public void RegisterSessionCoin(string coinID)
    {
        if (!string.IsNullOrEmpty(coinID))
        {
            sessionCollectedCoinIDs.Add(coinID);
        }
    }

    public void ResetCoinPrefsFromScene()
    {
        foreach (Coin coin in FindObjectsOfType<Coin>())
        {
            if (!string.IsNullOrEmpty(coin.CoinID))
            {
                PlayerPrefs.DeleteKey("CoinCollected_" + coin.CoinID);
            }
        }

        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.Save();
        Debug.Log("All CoinCollected keys from this scene have been reset.");
    }


}