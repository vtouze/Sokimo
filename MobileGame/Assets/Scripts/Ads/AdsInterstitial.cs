using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    private string _adUnitId;
    private bool _adIsLoaded = false;

    void Awake()
    {
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;
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
            _adIsLoaded = false; // R�initialiser car il faudra reload apr�s la pub
        }
        else
        {
            Debug.LogWarning("Ad not ready yet. Reloading...");
            LoadAd(); // Tente de charger � nouveau
        }
    }

    // ----- LISTENERS -----

    public void OnClickShowAd()
    {
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
        }
        else
        {
            Debug.Log("Ad was skipped or failed. No reward.");
        }

        LoadAd(); // Preload for next time
    }
}
