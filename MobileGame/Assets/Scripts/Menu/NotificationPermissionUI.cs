using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications;
using TMPro;
using System.Collections;

public class NotificationPermissionUI : MonoBehaviour
{
    public Button askNotificationButton;
    public TMP_Text statusText;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    var status = NotificationCenter.CheckPermissionStatus();
    if (status == NotificationsPermissionStatus.Granted)
    {
        askNotificationButton.gameObject.SetActive(false);
        statusText.text = "Permission already granted.";
    }
    else
    {
        askNotificationButton.gameObject.SetActive(true);
        askNotificationButton.onClick.AddListener(RequestPermission);
    }
#endif
    }

    void RequestPermission()
    {
        StartCoroutine(RequestPermissionCoroutine());
    }

    IEnumerator RequestPermissionCoroutine()
    {
        var request = NotificationCenter.RequestPermission();

        if (request.Status == NotificationsPermissionStatus.RequestPending)
            yield return request;

        Debug.Log("Permission result: " + request.Status);
        statusText.text = "Permission: " + request.Status.ToString();
    }

    public void OpenSystemNotificationSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.APP_NOTIFICATION_SETTINGS");
        intent.Call<AndroidJavaObject>("putExtra", "android.provider.extra.APP_PACKAGE", Application.identifier);
        currentActivity.Call("startActivity", intent);
    }
#endif
    }
}
