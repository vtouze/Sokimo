using UnityEngine;
using Unity.Notifications.Android;
using Unity.Notifications;
using System;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

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
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator RequestNotificationPermission()
    {
        var request = NotificationCenter.RequestPermission();
        if (request.Status == NotificationsPermissionStatus.RequestPending)
            yield return request;

        Debug.Log("Permission result: " + request.Status);
    }

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

    public void ScheduleDailyRewardNotification(DateTime triggerTime, int delayInSeconds = 2)
    {
        if (!NotificationsEnabled)
        {
            Debug.Log("Notification not scheduled: disabled by user.");
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
    }

    public void SendTestNotification()
    {
        if (!NotificationsEnabled)
        {
            Debug.Log("Test notification not sent: notifications disabled.");
            return;
        }

        DateTime fireTime = DateTime.UtcNow.AddSeconds(5); // 5 seconds from now

        var notification = new AndroidNotification()
        {
            Title = "🧪 Test Notification",
            Text = "If you see this, notifications are working!",
            FireTime = fireTime
        };

        AndroidNotificationCenter.SendNotification(notification, "daily_reward_channel");

        Debug.Log("Test notification scheduled for: " + fireTime);
    }
}