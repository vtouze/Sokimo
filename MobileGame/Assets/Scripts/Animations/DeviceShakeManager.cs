using UnityEngine;

public enum ShakeType
{
    Light,
    Medium,
    Heavy
}

public class DeviceShakeManager : MonoBehaviour
{
    public static DeviceShakeManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Shake(ShakeType type)
    {
#if UNITY_ANDROID || UNITY_IOS
        switch (type)
        {
            case ShakeType.Light:
                Vibrate(100);
                break;
            case ShakeType.Medium:
                Vibrate(300);
                break;
            case ShakeType.Heavy:
                Vibrate(600);
                break;
        }
#endif
    }

    void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
        {
            if (vibrator != null)
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate(); // iOS only supports this
#endif
    }
}