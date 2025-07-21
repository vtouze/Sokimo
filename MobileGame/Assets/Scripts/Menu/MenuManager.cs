using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject levelSelectionMenu;

    [Header("Background Video")]
    [SerializeField] private VideoPlayer backgroundVideo;

    private GameObject currentMenu;

    private void Start()
    {
        if (backgroundVideo != null)
        {
            backgroundVideo.isLooping = true;
            backgroundVideo.Play();
        }

        ShowMenu(mainMenu);
    }

    public void ShowMenu(GameObject menuToShow)
    {
        if (currentMenu != null)
            currentMenu.SetActive(false);

        menuToShow.SetActive(true);
        currentMenu = menuToShow;
    }

    public void ShowMainMenu() => ShowMenu(mainMenu);
    public void ShowSettingsMenu() => ShowMenu(settingsMenu);
    public void ShowShopMenu() => ShowMenu(shopMenu);
    public void ShowLevelSelectionMenu() => ShowMenu(levelSelectionMenu);
}