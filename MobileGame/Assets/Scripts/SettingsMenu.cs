using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Button toggleButton;
    public Sprite spriteOn;
    public Sprite spriteOff;

    void Start()
    {
        if (toggleButton == null)
            toggleButton = GetComponent<Button>();

        UpdateVisual();

        toggleButton.onClick.AddListener(() =>
        {
            // Toggle the setting
            bool newValue = !NotificationManager.Instance.NotificationsEnabled;
            NotificationManager.Instance.NotificationsEnabled = newValue;

            UpdateVisual();
        });
    }

    void UpdateVisual()
    {
        if (toggleButton.image != null)
        {
            toggleButton.image.sprite = NotificationManager.Instance.NotificationsEnabled ? spriteOn : spriteOff;
        }
    }
}