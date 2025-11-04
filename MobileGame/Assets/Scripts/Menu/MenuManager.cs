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
        Debug.Log($"[MenuManager] Initialized. Current menu: {currentMenu.name}");
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
        Debug.Log($"[MenuManager] ShowShopMenu called. Current menu: {currentMenu.name}, Target menu: shopMenu");
        ShowMenuFromDirection(shopMenu, Vector2.left);
    }

    public void ShowSettingsMenu()
    {
        Debug.Log($"[MenuManager] ShowSettingsMenu called. Current menu: {currentMenu.name}, Target menu: settingsMenu");
        ShowMenuFromDirection(settingsMenu, Vector2.right);
    }

    public void ShowLevelSelectionMenu()
    {
        Debug.Log($"[MenuManager] ShowLevelSelectionMenu called. Current menu: {currentMenu.name}, Target menu: levelSelectionMenu");
        ShowMenuFromDirection(levelSelectionMenu, Vector2.down);
    }

    public void BackToMainMenu()
    {
        if (currentMenu == null || currentMenu == mainMenu)
        {
            Debug.Log($"[MenuManager] BackToMainMenu called, but current menu is already mainMenu. Skipping.");
            return;
        }

        Vector2 exitDir = Vector2.zero;
        if (currentMenu == shopMenu) exitDir = Vector2.left;
        else if (currentMenu == settingsMenu) exitDir = Vector2.right;
        else if (currentMenu == levelSelectionMenu) exitDir = Vector2.down;

        RectTransform exiting = currentMenu;
        Debug.Log($"[MenuManager] BackToMainMenu called. Current menu: {exiting.name}, Target menu: mainMenu");

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 exitPos = (Vector3)(exitDir * screenSize);

        // Fade out the exiting menu (currentMenu)
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
                Debug.Log($"[MenuManager] {exiting.name} faded out and deactivated.");
            });

        // Slide out the exiting menu
        LeanTween.move(exiting, exiting.anchoredPosition3D + exitPos, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log($"[MenuManager] {exiting.name} slid out.");
            });

        // Prepare the main menu for fade-in
        mainMenu.anchoredPosition3D = -exitPos;
        mainMenu.gameObject.SetActive(true);

        // Fade in the main menu
        CanvasGroup mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();
        if (mainMenuCanvasGroup == null)
        {
            mainMenuCanvasGroup = mainMenu.gameObject.AddComponent<CanvasGroup>();
        }
        mainMenuCanvasGroup.alpha = 0f; // Start invisible
        LeanTween.alphaCanvas(mainMenuCanvasGroup, 1f, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log($"[MenuManager] mainMenu faded in.");
            });

        // Slide in the main menu
        LeanTween.move(mainMenu, Vector3.zero, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log($"[MenuManager] mainMenu slid in.");
            });

        currentMenu = mainMenu;
        Debug.Log($"[MenuManager] Current menu updated to: {currentMenu.name}");
    }

    private void ShowMenuFromDirection(RectTransform targetMenu, Vector2 fromDirection)
    {
        if (currentMenu == targetMenu)
        {
            Debug.Log($"[MenuManager] ShowMenuFromDirection called, but current menu is already {targetMenu.name}. Skipping.");
            return;
        }

        Debug.Log($"[MenuManager] ShowMenuFromDirection called. Current menu: {currentMenu.name}, Target menu: {targetMenu.name}");

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 enterPos = (Vector3)(fromDirection * screenSize);

        if (currentMenu != null)
        {
            // Fade out the current menu
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
                    Debug.Log($"[MenuManager] {currentMenu.name} faded out and deactivated.");
                    currentMenu = targetMenu; // Update currentMenu after fade-out completes
                    Debug.Log($"[MenuManager] Current menu updated to: {currentMenu.name}");
                });
        }

        // Prepare the target menu for fade-in
        targetMenu.anchoredPosition3D = enterPos;
        targetMenu.gameObject.SetActive(true);

        // Fade in the target menu
        CanvasGroup targetMenuCanvasGroup = targetMenu.GetComponent<CanvasGroup>();
        if (targetMenuCanvasGroup == null)
        {
            targetMenuCanvasGroup = targetMenu.gameObject.AddComponent<CanvasGroup>();
        }
        targetMenuCanvasGroup.alpha = 0f; // Start invisible
        LeanTween.alphaCanvas(targetMenuCanvasGroup, 1f, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log($"[MenuManager] {targetMenu.name} faded in.");
            });

        // Slide in the target menu
        LeanTween.move(targetMenu, Vector3.zero, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log($"[MenuManager] {targetMenu.name} slid in.");
            });
    }
}