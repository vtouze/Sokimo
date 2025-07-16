using UnityEngine;
using UnityEngine.UI;

public class UIAnimationsManager : MonoBehaviour
{
    public static UIAnimationsManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Rotation sur l'axe X
    public void RotateX(GameObject target, float angle = 360f, float duration = 1f)
    {
        LeanTween.rotateX(target, angle, duration).setLoopClamp();
    }

    // Pulse scale en boucle
    public void PulseScale(GameObject target, float pulseScale = 1.2f, float duration = 0.5f)
    {
        LeanTween.scale(target, Vector3.one * pulseScale, duration)
            .setLoopPingPong();
    }

    // Shake position
    public void Shake(GameObject target, float strength = 15f, float duration = 0.5f)
    {
        LeanTween.moveLocalX(target, target.transform.localPosition.x + strength, duration / 4)
            .setLoopPingPong(3);
    }

    // Bounce up
    public void Bounce(GameObject target, float bounceHeight = 30f, float duration = 0.5f)
    {
        Vector3 startPos = target.transform.localPosition;
        LeanTween.moveLocalY(target, startPos.y + bounceHeight, duration)
            .setLoopPingPong();
    }

    // Fade In (requires CanvasGroup on target)
    public void FadeIn(GameObject target, float duration = 0.5f)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1f, duration);
    }

    // Fade Out (requires CanvasGroup on target)
    public void FadeOut(GameObject target, float duration = 0.5f)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        LeanTween.alphaCanvas(cg, 0f, duration);
    }

    // Slide In from right
    public void SlideInFromRight(GameObject target, float distance = 500f, float duration = 0.5f)
    {
        Vector3 startPos = target.transform.localPosition;
        target.transform.localPosition = startPos + new Vector3(distance, 0, 0);
        LeanTween.moveLocalX(target, startPos.x, duration).setEaseOutExpo();
    }

    // Flip horizontal (rotation Y 0->180)
    public void FlipHorizontal(GameObject target, float duration = 0.5f)
    {
        LeanTween.rotateY(target, 180f, duration).setLoopPingPong(1);
    }
}