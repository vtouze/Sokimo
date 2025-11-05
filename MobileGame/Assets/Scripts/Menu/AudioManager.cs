using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    private const string SFX_VOLUME_PARAM = "SFX";
    private const string MUSIC_VOLUME_PARAM = "Music";
    private const string MASTER_VOLUME_PARAM = "Master";

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
        bool masterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, sfxEnabled && !masterMuted ? 0f : -80f);
            audioMixer.SetFloat(MUSIC_VOLUME_PARAM, musicEnabled && !masterMuted ? 0f : -80f);
            audioMixer.SetFloat(MASTER_VOLUME_PARAM, masterMuted ? -80f : 0f);
        }
    }
}