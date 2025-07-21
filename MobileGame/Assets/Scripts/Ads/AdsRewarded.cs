using System;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;
using UnityEngine.UI;


public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    private string _adUnitId;
    private bool _adIsLoaded = false;

    private DateTime nextAdRewardTime;
    private const string AdCooldownKey = "NextAdRewardTime";
    public int adCooldownDuration = 10; // for testing — set to 86400 for 24h

    [SerializeField] private TMP_Text adCooldownText;
    [SerializeField] private Button adButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;


    private bool IsAdRewardAvailable => DateTime.UtcNow >= nextAdRewardTime;


    void Awake()
    {
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;

        LoadNextAdRewardTime();
    }

    void Update()
    {
        TimeSpan remaining = nextAdRewardTime - DateTime.UtcNow;

        if (IsAdRewardAvailable)
        {
            adCooldownText.text = "Ready!";
            if (adButton != null) adButton.interactable = true;
            if (buttonImage != null && activeSprite != null)
                buttonImage.sprite = activeSprite;
        }
        else
        {
            adCooldownText.text = $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            if (adButton != null) adButton.interactable = false;
            if (buttonImage != null && inactiveSprite != null)
                buttonImage.sprite = inactiveSprite;
        }
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (_adIsLoaded)
        {
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
            _adIsLoaded = false;

        }
        else
        {
            Debug.LogWarning("Ad not ready yet. Reloading...");
            LoadAd();
        }
    }

    private void LoadNextAdRewardTime()
    {
        if (PlayerPrefs.HasKey(AdCooldownKey))
        {
            long binary = Convert.ToInt64(PlayerPrefs.GetString(AdCooldownKey));
            nextAdRewardTime = DateTime.FromBinary(binary);
        }
        else
        {
            nextAdRewardTime = DateTime.MinValue;
        }
    }

    private void SaveNextAdRewardTime()
    {
        PlayerPrefs.SetString(AdCooldownKey, nextAdRewardTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    public void OnClickShowAd()
    {
        if (!IsAdRewardAvailable)
        {
            Debug.Log("Ad reward is on cooldown. Try again later.");
            return;
        }

        ShowAd();
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId == _adUnitId)
        {
            Debug.Log("Ad successfully loaded: " + adUnitId);
            _adIsLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Ad started: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Ad clicked: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("User watched the full ad. Grant reward.");
            CoinManager.Instance?.AddCoin();

            nextAdRewardTime = DateTime.UtcNow.AddSeconds(adCooldownDuration);
            SaveNextAdRewardTime();

            NotificationManager.Instance?.ScheduleAdRewardNotification(nextAdRewardTime);
        }
        else
        {
            Debug.Log("Ad was skipped or failed. No reward.");
        }

        LoadAd();
    }
}
