/*using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdsBanner : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";

    private string _adUnitId;

    IEnumerator Start()
    {
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#else
        Debug.LogWarning("Platform not supported for Unity Ads banners.");
        yield break;
#endif

        Debug.Log($"[AdsBanner] Selected Ad Unit ID: {_adUnitId}");

        // Set banner position early
        Advertisement.Banner.SetPosition(_bannerPosition);

        // Wait until ads are initialized before loading banner
        Debug.Log("[AdsBanner] Waiting for ads initialization...");
        while (!AdsInitializer.adsInitialized)
        {
            yield return new WaitForSeconds(1.25f);
        }
        Debug.Log("[AdsBanner] Ads initialized, proceeding to load banner...");

        // Check if Banner ads are supported on this platform
        if (!Advertisement.isSupported)
        {
            Debug.LogError("[AdsBanner] Unity Ads is not supported on this platform.");
            yield break;
        }

        if (!Advertisement.Banner.isLoaded)
        {
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Debug.Log("[AdsBanner] Loading banner...");
            Advertisement.Banner.Load(_adUnitId, options);
        }
        else
        {
            Debug.Log("[AdsBanner] Banner already loaded. Showing banner...");
            Advertisement.Banner.Show(_adUnitId);
        }
    }

    void OnBannerLoaded()
    {
        Debug.Log("✅ Banner loaded successfully. Showing it now...");
        Advertisement.Banner.Show(_adUnitId);
    }

    void OnBannerError(string message)
    {
        Debug.LogError($"❌ Banner failed to load: {message}");
        StartCoroutine(RetryBannerLoad());
    }

    IEnumerator RetryBannerLoad()
    {
        Debug.Log("[AdsBanner] Retrying banner load in 5 seconds...");
        yield return new WaitForSeconds(5f);

        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Debug.Log("[AdsBanner] Retrying to load banner...");
        Advertisement.Banner.Load(_adUnitId, options);
    }
}*/