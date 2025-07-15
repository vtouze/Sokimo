using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyRewardButton : MonoBehaviour
{
    public Button rewardButton;
    public Image buttonImage;
    public Sprite spriteAvailable;
    public Sprite spriteCooldown;
    public TMP_Text timerText;

    public int cooldownDuration = 10; // e.g., 86400 for 24h

    private DateTime nextRewardTime;
    private bool isReady;

    void Start()
    {
        LoadNextRewardTime();
        rewardButton.onClick.AddListener(ClaimReward);
        UpdateUI();
    }

    void Update()
    {
        if (!isReady)
        {
            TimeSpan remaining = nextRewardTime - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
            {
                isReady = true;
                UpdateUI();
            }
            else
            {
                timerText.text = $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
        }
    }

    void ClaimReward()
    {
        if (!isReady) return;

        Debug.Log("1 coin added!");
        CoinManager.Instance?.AddCoin();

        nextRewardTime = DateTime.UtcNow.AddSeconds(cooldownDuration);
        SaveNextRewardTime();

        // 🔔 Schedule notification when reward goes on cooldown
        NotificationManager.Instance?.ScheduleDailyRewardNotification(nextRewardTime);

        isReady = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        rewardButton.interactable = isReady;
        buttonImage.sprite = isReady ? spriteAvailable : spriteCooldown;
        timerText.text = isReady ? "Ready!" : timerText.text;
    }

    void LoadNextRewardTime()
    {
        if (PlayerPrefs.HasKey("NextRewardTime"))
        {
            long binary = Convert.ToInt64(PlayerPrefs.GetString("NextRewardTime"));
            nextRewardTime = DateTime.FromBinary(binary);
            isReady = DateTime.UtcNow >= nextRewardTime;
        }
        else
        {
            isReady = true;
        }
    }

    void SaveNextRewardTime()
    {
        PlayerPrefs.SetString("NextRewardTime", nextRewardTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }
}