using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;

    private const string SFX_VOLUME_PARAM = "SFX";
    private const string MUSIC_VOLUME_PARAM = "Music";

    private bool sfxEnabled = true;
    private bool musicEnabled = true;

    void Start()
    {
        LoadSettings();
        UpdateVisuals();

        if (sfxToggleButton != null)
            sfxToggleButton.onClick.AddListener(ToggleSFX);
        if (musicToggleButton != null)
            musicToggleButton.onClick.AddListener(ToggleMusic);
    }

    private void LoadSettings()
    {
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        Debug.Log($"Loading settings: SFX={sfxEnabled}, Music={musicEnabled}");

        SetSFXVolume(sfxEnabled ? 0f : -80f);
        SetMusicVolume(musicEnabled ? 0f : -80f);
    }

    private void UpdateVisuals()
    {
        if (sfxToggleButton != null && sfxToggleButton.image != null)
            sfxToggleButton.image.sprite = sfxEnabled ? spriteOn : spriteOff;
        if (musicToggleButton != null && musicToggleButton.image != null)
            musicToggleButton.image.sprite = musicEnabled ? spriteOn : spriteOff;
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        Debug.Log($"Toggling SFX: {sfxEnabled}");
        SetSFXVolume(sfxEnabled ? 0f : -80f);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateVisuals();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        Debug.Log($"Toggling Music: {musicEnabled}");
        SetMusicVolume(musicEnabled ? 0f : -80f);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateVisuals();
    }

    private void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat(SFX_VOLUME_PARAM, volume);
            if (!success)
                Debug.LogError($"Failed to set SFX volume. Parameter '{SFX_VOLUME_PARAM}' not found in AudioMixer.");
            else
                Debug.Log($"SFX volume set to: {volume}");
        }
        else
            Debug.LogError("AudioMixer is not assigned.");
    }

    private void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat(MUSIC_VOLUME_PARAM, volume);
            if (!success)
                Debug.LogError($"Failed to set Music volume. Parameter '{MUSIC_VOLUME_PARAM}' not found in AudioMixer.");
            else
                Debug.Log($"Music volume set to: {volume}");
        }
        else
            Debug.LogError("AudioMixer is not assigned.");
    }
}