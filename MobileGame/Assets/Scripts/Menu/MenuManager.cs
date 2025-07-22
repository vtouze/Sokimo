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
        if (currentMenu == null || currentMenu == mainMenu) return;

        Vector2 exitDir = Vector2.zero;

        if (currentMenu == shopMenu) exitDir = Vector2.left;
        else if (currentMenu == settingsMenu) exitDir = Vector2.right;
        else if (currentMenu == levelSelectionMenu) exitDir = Vector2.down;

        RectTransform exiting = currentMenu;
        currentMenu = mainMenu;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 exitPos = (Vector3)(exitDir * screenSize);

        LeanTween.move(exiting, exiting.anchoredPosition3D + exitPos, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            exiting.gameObject.SetActive(false);
            mainMenu.anchoredPosition3D = -exitPos;
            mainMenu.gameObject.SetActive(true);
            LeanTween.move(mainMenu, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad);
        });
    }

    private void ShowMenuFromDirection(RectTransform targetMenu, Vector2 fromDirection)
    {
        if (currentMenu == targetMenu) return;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 enterPos = (Vector3)(fromDirection * screenSize);

        if (currentMenu != null)
        {
            currentMenu.gameObject.SetActive(false);
        }

        targetMenu.anchoredPosition3D = enterPos;
        targetMenu.gameObject.SetActive(true);
        LeanTween.move(targetMenu, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad);

        currentMenu = targetMenu;
    }
}