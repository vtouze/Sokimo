using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    [Header("Scrolling")]
    [SerializeField] private RectTransform creditsContent;
    [SerializeField] private float scrollSpeed = 50f;

    [Header("Logo Fade")]
    [SerializeField] private RectTransform logoRect;

    [Header("Fade Manager")]
    [SerializeField] private FadeManager fadeManager;
    private string nextSceneName = "MainMenu";

    [SerializeField] private PlayerController playerController;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool fadeTriggered = false;

    private void Start()
    {
        startPos = creditsContent.anchoredPosition;
        endPos = new Vector2(startPos.x, startPos.y + (creditsContent.rect.height + Screen.height));

        if (playerController != null)
            playerController.enabled = false;
    }

    private void Update()
    {
        ScrollCredits();
        CheckLogoFade();
    }

    private void ScrollCredits()
    {
        creditsContent.anchoredPosition = Vector2.MoveTowards(
            creditsContent.anchoredPosition,
            endPos,
            scrollSpeed * Time.deltaTime
        );
    }

    private void CheckLogoFade()
    {
        if (fadeTriggered || logoRect == null) return;

        Vector3 logoCenterScreen;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            creditsContent,
            RectTransformUtility.WorldToScreenPoint(Camera.main, logoRect.position),
            Camera.main,
            out logoCenterScreen
        );

        Vector3[] corners = new Vector3[4];
        logoRect.GetWorldCorners(corners);
        float logoCenterY = (corners[0].y + corners[2].y) / 2f;

        float screenCenterY = Screen.height / 2f;

        if (Mathf.Abs(logoCenterY - screenCenterY) < 5f)
        {
            fadeTriggered = true;
            if (fadeManager != null)
                fadeManager.PlayFadeOutAndLoadScene(nextSceneName);
        }
    }
}