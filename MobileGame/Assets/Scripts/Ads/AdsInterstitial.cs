/*using UnityEngine;
using UnityEngine.Advertisements;
 

public class AdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOSAdUnitId = "Interstitial_iOS";
    private string _adUnitId;
    private bool _adIsLoaded = false;

    private System.Action onAdCompleteCallback;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
        ? _iOSAdUnitId
        : _androidAdUnitId;
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
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

    // ----- LISTENERS -----

    public void OnClickShowAd()
    {
        ShowAd();
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        if (adUnitId == _adUnitId)
        {
            Debug.Log("Ad successfully loaded: " + adUnitId);
            _adIsLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        Debug.LogError($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
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
        Debug.Log($"Ad finished: {adUnitId} - Completion State: {showCompletionState}");

        if (adUnitId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            onAdCompleteCallback?.Invoke();
        }
        else
        {
            onAdCompleteCallback?.Invoke();
        }

        onAdCompleteCallback = null;
        LoadAd();
    }

    public void ShowAdWithCallback(System.Action callback)
    {
        if (_adIsLoaded)
        {
            onAdCompleteCallback = callback;
            Advertisement.Show(_adUnitId, this);
            _adIsLoaded = false;
        }
        else
        {
            Debug.Log("Ad not ready, skipping...");
            callback?.Invoke(); // Continue immediately if no ad
            LoadAd();           // Try again for next time
        }
    }
}*/