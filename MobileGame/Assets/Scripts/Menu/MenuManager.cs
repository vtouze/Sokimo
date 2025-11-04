using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform settingsMenu;
    [SerializeField] private RectTransform shopMenu;
    [SerializeField] private RectTransform levelSelectionMenu;
    [Header("Background Video")]
    [SerializeField] private VideoPlayer backgroundVideo;
    private RectTransform currentMenu;

    private void Start()
    {
        if (backgroundVideo != null)
        {
            backgroundVideo.isLooping = true;
            backgroundVideo.Play();
        }
        HideAllMenus();
        mainMenu.gameObject.SetActive(true);
        currentMenu = mainMenu;
    }

    private void HideAllMenus()
    {
        mainMenu.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
        shopMenu.gameObject.SetActive(false);
        levelSelectionMenu.gameObject.SetActive(false);
    }

    public void ShowShopMenu()
    {
        ShowMenuFromDirection(shopMenu, Vector2.left);
    }

    public void ShowSettingsMenu()
    {
        ShowMenuFromDirection(settingsMenu, Vector2.right);
    }

    public void ShowLevelSelectionMenu()
    {
        ShowMenuFromDirection(levelSelectionMenu, Vector2.down);
    }

    public void BackToMainMenu()
    {
        Vector2 exitDir = Vector2.zero;
        if (currentMenu == shopMenu) exitDir = Vector2.left;
        else if (currentMenu == settingsMenu) exitDir = Vector2.right;
        else if (currentMenu == levelSelectionMenu) exitDir = Vector2.down;

        RectTransform exiting = currentMenu;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 exitPos = (Vector3)(exitDir * screenSize);

        CanvasGroup exitingCanvasGroup = exiting.GetComponent<CanvasGroup>();
        if (exitingCanvasGroup == null)
        {
            exitingCanvasGroup = exiting.gameObject.AddComponent<CanvasGroup>();
        }
        exitingCanvasGroup.alpha = 1f;
        LeanTween.alphaCanvas(exitingCanvasGroup, 0f, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                exiting.gameObject.SetActive(false);
            });

        LeanTween.move(exiting, exiting.anchoredPosition3D + exitPos, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
            });

        mainMenu.anchoredPosition3D = -exitPos;
        mainMenu.gameObject.SetActive(true);

        CanvasGroup mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();
        if (mainMenuCanvasGroup == null)
        {
            mainMenuCanvasGroup = mainMenu.gameObject.AddComponent<CanvasGroup>();
        }
        mainMenuCanvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(mainMenuCanvasGroup, 1f, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
            });

        LeanTween.move(mainMenu, Vector3.zero, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
            });

        currentMenu = mainMenu;
    }

    private void ShowMenuFromDirection(RectTransform targetMenu, Vector2 fromDirection)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 enterPos = (Vector3)(fromDirection * screenSize);

        if (currentMenu != null)
        {
            CanvasGroup currentMenuCanvasGroup = currentMenu.GetComponent<CanvasGroup>();
            if (currentMenuCanvasGroup == null)
            {
                currentMenuCanvasGroup = currentMenu.gameObject.AddComponent<CanvasGroup>();
            }
            currentMenuCanvasGroup.alpha = 1f;
            LeanTween.alphaCanvas(currentMenuCanvasGroup, 0f, 0.3f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    currentMenu.gameObject.SetActive(false);
                    currentMenu = targetMenu;
                });
        }

        targetMenu.anchoredPosition3D = enterPos;
        targetMenu.gameObject.SetActive(true);

        CanvasGroup targetMenuCanvasGroup = targetMenu.GetComponent<CanvasGroup>();
        if (targetMenuCanvasGroup == null)
        {
            targetMenuCanvasGroup = targetMenu.gameObject.AddComponent<CanvasGroup>();
        }
        targetMenuCanvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(targetMenuCanvasGroup, 1f, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
            });

        LeanTween.move(targetMenu, Vector3.zero, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
            });
    }
}