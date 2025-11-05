using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private GameObject fadeObject;
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private string fadeOutTrigger = "FadeOut";
    [SerializeField] private string fadeInTrigger = "FadeIn";
    public float fadeDuration = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private void Start()
    {
        if (fadeObject != null)
        {
            fadeObject.SetActive(true);
            PlayFadeIn();
        }
    }

    public void PlayFadeIn()
    {
        fadeAnimator.ResetTrigger(fadeInTrigger);
        fadeAnimator.SetTrigger(fadeOutTrigger);
        StartCoroutine(DisableAfterDelay());
    }

    public void PlayFadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public void PlayFadeOutAndQuit()
    {
        StartCoroutine(FadeOutAndQuit());
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        PlaySound(buttonClickSound);
        fadeObject.SetActive(true);
        fadeAnimator.ResetTrigger(fadeOutTrigger);
        fadeAnimator.SetTrigger(fadeInTrigger);

        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutAndQuit()
    {
        fadeObject.SetActive(true);
        fadeAnimator.ResetTrigger(fadeInTrigger);
        fadeAnimator.SetTrigger(fadeOutTrigger);

        yield return new WaitForSeconds(fadeDuration);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(fadeDuration);
        fadeObject.SetActive(false);
    }

    public void PlayRawFadeOut()
    {
        if (fadeObject != null)
        {
            fadeObject.SetActive(true);
            fadeAnimator.ResetTrigger(fadeOutTrigger);
            fadeAnimator.SetTrigger(fadeInTrigger);
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