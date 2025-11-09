using UnityEngine;
using UnityEngine.UI;

public class OpenGooglePlayStore : MonoBehaviour
{
    public string appPackageName = "com.company.app";
    public void OpenStorePage()
    {
        string appUrl = $"market://details?id={appPackageName}";
        string webUrl = $"https://play.google.com/store/apps/details?id={appPackageName}";

        Application.OpenURL(appUrl);
    }
}