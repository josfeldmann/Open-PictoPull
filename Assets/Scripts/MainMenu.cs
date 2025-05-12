using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject SteamWorkshopButton;
    public GameObject achievementButton;
    public GameObject creditMenu, onlineSources, mainCredits;
    public LanguageMenu languageMenu;
    public InputManager inputManager;
    public NavigationObject StoryButton, optionsButton;
    public GameObject webGLWarning;
    public void Setup() {

        webGLWarning.SetActive(false);
#if PLATFORM_WEBGL
        webGLWarning.SetActive(true);
#endif

        languageMenu.gameObject.SetActive(false);
        creditMenu.gameObject.SetActive(false);

    #if !UNITY_SWITCH
        if (GameMasterManager.isDemo) {
            achievementButton.SetActive(false);
        } else {
            achievementButton.SetActive(true);
        }
    #endif
        SteamWorkshopButton.SetActive(false);
    #if !DISABLESTEAMWORKS
        if (SteamManager.isSteamBuild) {
            SteamWorkshopButton.SetActive(true);
        } else {
            SteamWorkshopButton.SetActive(false);
        }
    #endif

        ShowMainCredits();


    }

    private void Update() {
        if (inputManager.cancelButtonDown && creditMenu.activeInHierarchy) {
            CloseCreditMenu();
        }
    }


    public void ShowCreditMenu() {

        GameMasterManager.instance.ShowGamePadCursor();
        creditMenu.SetActive(true);
        NavigationManager.StopNavigation();
    }

    public void ShowMainCredits() {
        onlineSources.SetActive(false);
        mainCredits.SetActive(true);
    }
    public void ShowOnlineSources() {
        #if !UNITY_SWITCH
        onlineSources.SetActive(true);
        mainCredits.SetActive(false);
        #else
        GameMasterManager.instance.OpenURL("https://www.beardedants.com/pictopull-nintendo-switch-licenses/");
        #endif
    }


    public void CloseCreditMenu() {
        GameMasterManager.instance.HideGamePadCursor();
        #if !UNITY_SWITCH
        NavigationManager.SetSelectedObject(GameMasterManager.instance.firstMainMenuButton);
        #else
        NavigationManager.SetSelectedObject(GameMasterManager.instance.firstMainMenuButtonSwitch);
        #endif
        creditMenu.SetActive(false);
    }

    public void OpenLanguageMenu() {
        languageMenu.gameObject.SetActive(true);
    }

    public void CloseLanguageMenu() {
        languageMenu.gameObject.SetActive(false);
    }
}
