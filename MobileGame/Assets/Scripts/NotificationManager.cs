using UnityEngine;
using Unity.Notifications.Android;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNotificationChannel();
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void ScheduleDailyRewardNotification(DateTime triggerTime)
    {
        // Cancel existing notifications (optional)
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        var notification = new AndroidNotification()
        {
            Title = "🎁 Daily Reward Ready!",
            Text = "Come back to claim your reward!",
            FireTime = triggerTime,
        };

        AndroidNotificationCenter.SendNotification(notification, "daily_reward_channel");
    }
}