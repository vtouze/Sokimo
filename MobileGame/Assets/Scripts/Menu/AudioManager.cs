using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    private const string SFX_VOLUME_PARAM = "SFX";
    private const string MUSIC_VOLUME_PARAM = "Music";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAudioSettings();
    }

    void ApplyAudioSettings()
    {
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, sfxEnabled ? 0f : -80f);
            audioMixer.SetFloat(MUSIC_VOLUME_PARAM, musicEnabled ? 0f : -80f);
        }
    }
}