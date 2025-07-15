using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewardButton : MonoBehaviour
{
    public Button rewardButton;
    public Image buttonImage;
    public Sprite spriteAvailable;
    public Sprite spriteCooldown;
    public TMP_Text timerText;

    // Duration in seconds (set to 5 for testing, 86400 for 24h)
    public int cooldownDuration = 10;

    private DateTime nextRewardTime;
    private bool isReady = false;

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
        PlayerPrefs.SetString("NextRewardTime", nextRewardTime.ToBinary().ToString());
        PlayerPrefs.Save();

        isReady = false;
        UpdateUI();

        // 🔔 Schedule notification right now
        NotificationManager.Instance?.ScheduleDailyRewardNotification(nextRewardTime);
    }


    void UpdateUI()
    {
        if (isReady)
        {
            rewardButton.interactable = true;
            buttonImage.sprite = spriteAvailable;
            timerText.text = "Ready!";
        }
        else
        {
            rewardButton.interactable = false;
            buttonImage.sprite = spriteCooldown;
        }
    }

    void LoadNextRewardTime()
    {
        if (PlayerPrefs.HasKey("NextRewardTime"))
        {
            long binary = Convert.ToInt64(PlayerPrefs.GetString("NextRewardTime"));
            nextRewardTime = DateTime.FromBinary(binary);
            isReady = DateTime.UtcNow >= nextRewardTime;

            // 🔔 Schedule notification if not ready yet
            if (!isReady)
            {
                NotificationManager.Instance?.ScheduleDailyRewardNotification(nextRewardTime);
            }
        }
        else
        {
            isReady = true;
        }
    }

}