using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Audio;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = false;
    [SerializeField] private AdsRewarded adsRewarded;
    [SerializeField] private AdsInterstitial adsInterstitial;

    private string _gameId;

    public static bool adsInitialized = false;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
        _gameId = _androidGameId; // Only for testing the functionality in the Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        adsInitialized = true;

        if (adsRewarded != null)
        {
            adsRewarded.LoadAd();
        }
        else
        {
            Debug.LogWarning("AdsRewarded reference is not assigned. Skipping LoadAd.");
        }

        if (adsInterstitial != null)
        {
            adsInterstitial.LoadAd();
        }
        else
        {
            Debug.LogWarning("AdsInterstitial reference is not assigned. Skipping LoadAd.");
        }
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        adsInitialized = false;
    }
}