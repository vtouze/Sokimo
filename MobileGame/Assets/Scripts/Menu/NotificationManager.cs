using UnityEngine;
using Unity.Notifications.Android;
using Unity.Notifications;
using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI Elements")]
    [SerializeField] private Button notificationToggleButton;
    [SerializeField] private Sprite notificationSpriteOn;
    [SerializeField] private Sprite notificationSpriteOff;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    public bool NotificationsEnabled
    {
        get => PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("NotificationsEnabled", value ? 1 : 0);
            PlayerPrefs.Save();
            if (!value)
            {
                AndroidNotificationCenter.CancelAllScheduledNotifications();
                AndroidNotificationCenter.CancelAllDisplayedNotifications();
            }
            UpdateNotificationVisuals();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNotificationChannel();

#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(RequestNotificationPermission());
#elif UNITY_IOS && !UNITY_EDITOR
            StartCoroutine(RequestiOSNotificationPermission());
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateNotificationVisuals();
        if (notificationToggleButton != null)
        {
            notificationToggleButton.onClick.AddListener(ToggleNotifications);
        }
    }

    IEnumerator RequestNotificationPermission()
    {
        var request = NotificationCenter.RequestPermission();
        if (request.Status == NotificationsPermissionStatus.RequestPending)
            yield return request;

        Debug.Log("Permission result: " + request.Status);

        if (request.Status == NotificationsPermissionStatus.Denied)
        {
            Debug.Log("Notification permission denied. Opening app notification settings...");
            OpenAppNotificationSettings();
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
    IEnumerator RequestiOSNotificationPermission()
    {
        NotificationServices.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, true);
        yield return new WaitForEndOfFrame();
    }
#endif

    void InitializeNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "daily_reward_channel",
            Name = "Daily Reward Notifications",
            Importance = Importance.Default,
            Description = "Sends a notification when your daily reward is ready."
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public bool AreNotificationsEnabled()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var status = NotificationCenter.CheckPermission();
        return status == NotificationsPermissionStatus.Granted;
#elif UNITY_IOS && !UNITY_EDITOR
        return NotificationServices.authorizationStatus == AuthorizationStatus.Authorized;
#else
        return true;
#endif
    }

    public void ToggleNotifications()
    {
        PlaySound(buttonClickSound);
        NotificationsEnabled = !NotificationsEnabled;
        UpdateNotificationVisuals();
    }

    private void UpdateNotificationVisuals()
    {
        if (notificationToggleButton != null && notificationToggleButton.image != null)
        {
            notificationToggleButton.image.sprite = NotificationsEnabled ? notificationSpriteOn : notificationSpriteOff;
        }
    }

    public void ScheduleDailyRewardNotification(DateTime triggerTime, int delayInSeconds = 2)
    {
        if (!NotificationsEnabled || !AreNotificationsEnabled())
        {
            Debug.Log("Notification not scheduled: disabled by user or no permission.");
            return;
        }

        AndroidNotificationCenter.CancelAllScheduledNotifications();
        var notification = new AndroidNotification()
        {
            Title = "🎁 Daily Reward Ready!",
            Text = "Come back to claim your reward!",
            FireTime = triggerTime.AddSeconds(delayInSeconds)
        };
        AndroidNotificationCenter.SendNotification(notification, "daily_reward_channel");
    }

    public void ScheduleAdRewardNotification(DateTime triggerTime, int delayInSeconds = 2)
    {
        if (!NotificationsEnabled || !AreNotificationsEnabled())
        {
            Debug.Log("Ad Reward notification not scheduled: disabled by user or no permission.");
            return;
        }

        var notification = new AndroidNotification()
        {
            Title = "💰 Watch an Ad, Get a Coin!",
            Text = "Your next rewarded ad is ready to watch!",
            FireTime = triggerTime.AddSeconds(delayInSeconds)
        };
        AndroidNotificationCenter.SendNotification(notification, "daily_reward_channel");
    }

    public void OpenAppNotificationSettings()
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

    [ContextMenu("Reset Notification Toggle")]
    void ResetNotificationPrefs()
    {
        PlayerPrefs.DeleteKey("NotificationsEnabled");
        PlayerPrefs.Save();
        UpdateNotificationVisuals();
        Debug.Log("Notification preferences reset.");
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}