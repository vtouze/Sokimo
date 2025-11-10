using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using CandyCoded.HapticFeedback;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button masterToggleButton;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite masterMuteOnSprite;
    [SerializeField] private Sprite masterMuteOffSprite;

    [Header("Audio")]
    public AudioMixer audioMixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private const string SFX_VOLUME_PARAM = "SFX";
    private const string MUSIC_VOLUME_PARAM = "Music";
    private const string MASTER_VOLUME_PARAM = "Master";

    private bool sfxEnabled = true;
    private bool musicEnabled = true;
    private bool masterMuted = false;


    void Start()
    {
        LoadSettings();
        UpdateVisuals();
        UpdateMasterMuteIndicator();

        if (sfxToggleButton != null)
            sfxToggleButton.onClick.AddListener(ToggleSFX);
        if (musicToggleButton != null)
            musicToggleButton.onClick.AddListener(ToggleMusic);
        if (masterToggleButton != null)
            masterToggleButton.onClick.AddListener(ToggleMasterVolume);
    }

    private void LoadSettings()
    {
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        masterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;

        SetVolume(SFX_VOLUME_PARAM, sfxEnabled ? 0f : -80f);
        SetVolume(MUSIC_VOLUME_PARAM, musicEnabled ? 0f : -80f);
        SetVolume(MASTER_VOLUME_PARAM, masterMuted ? -80f : 0f);
    }

    private void UpdateVisuals()
    {
        if (sfxToggleButton != null && sfxToggleButton.image != null)
            sfxToggleButton.image.sprite = (sfxEnabled && !masterMuted) ? spriteOn : spriteOff;
        if (musicToggleButton != null && musicToggleButton.image != null)
            musicToggleButton.image.sprite = (musicEnabled && !masterMuted) ? spriteOn : spriteOff;
        if (masterToggleButton != null && masterToggleButton.image != null)
            masterToggleButton.image.sprite = masterMuted ? masterMuteOnSprite : masterMuteOffSprite;
    }

    private void UpdateMasterMuteIndicator()
    {
        if (masterToggleButton != null)
        {
            masterToggleButton.image.sprite = masterMuted ? masterMuteOnSprite : masterMuteOffSprite;
        }
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        SetVolume(SFX_VOLUME_PARAM, sfxEnabled ? 0f : -80f);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
        CheckAndUpdateMasterVolume();
        UpdateVisuals();
        PlaySound(buttonClickSound);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        SetVolume(MUSIC_VOLUME_PARAM, musicEnabled ? 0f : -80f);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
        CheckAndUpdateMasterVolume();
        UpdateVisuals();
        PlaySound(buttonClickSound);
    }

    public void ToggleMasterVolume()
    {
        masterMuted = !masterMuted;
        SetVolume(MASTER_VOLUME_PARAM, masterMuted ? -80f : 0f);
        PlayerPrefs.SetInt("MasterMuted", masterMuted ? 1 : 0);

        if (!masterMuted)
        {
            sfxEnabled = true;
            musicEnabled = true;
            SetVolume(SFX_VOLUME_PARAM, 0f);
            SetVolume(MUSIC_VOLUME_PARAM, 0f);
            PlayerPrefs.SetInt("SFXEnabled", 1);
            PlayerPrefs.SetInt("MusicEnabled", 1);
        }
        else
        {
            sfxEnabled = false;
            musicEnabled = false;
            SetVolume(SFX_VOLUME_PARAM, -80f);
            SetVolume(MUSIC_VOLUME_PARAM, -80f);
            PlayerPrefs.SetInt("SFXEnabled", 0);
            PlayerPrefs.SetInt("MusicEnabled", 0);
        }

        PlayerPrefs.Save();
        UpdateVisuals();
        UpdateMasterMuteIndicator();
        PlaySound(buttonClickSound);
    }

    private void CheckAndUpdateMasterVolume()
    {
        if (!sfxEnabled && !musicEnabled)
        {
            masterMuted = true;
            SetVolume(MASTER_VOLUME_PARAM, -80f);
            PlayerPrefs.SetInt("MasterMuted", 1);
        }
        else if (masterMuted && (sfxEnabled || musicEnabled))
        {
            masterMuted = false;
            SetVolume(MASTER_VOLUME_PARAM, 0f);
            PlayerPrefs.SetInt("MasterMuted", 0);
        }
        PlayerPrefs.Save();
        UpdateVisuals();
        UpdateMasterMuteIndicator();
    }

    private void SetVolume(string param, float volume)
    {
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat(param, volume);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}