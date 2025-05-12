using System.IO;
//using LapinerTools.Steam;
//using LapinerTools.Steam.Data;
using UnityEngine;
using TMPro;
//using LapinerTools.Steam.UI;


public class WorkshopManager : MonoBehaviour
{
    public GameObject workShopMainMenu;
    public GameObject uploadSelectPuzzleOrPackMenu;
    public GameObject workshopMenuButton;
    public GameObject downloadScreen, uploadScreen;
  //  public SteamWorkshopUIUpload uploadUI;
    public GameObject backPanel;
    
    #if !DISABLESTEAMWORKS
    public void UploadButton() {
        HideAll();
        workshopMenuButton.SetActive(true);
        uploadSelectPuzzleOrPackMenu.SetActive(true);
    }

    public void UploadPuzzleButton() {
        HideAll();
        workshopMenuButton.SetActive(true);
        GameMasterManager.instance.levelSelector.gameObject.SetActive(true);
        LevelSelector.selectingForWorkshop = true;
        GameMasterManager.currentGameMode = GameMode.WORKSHOPCOMPLETENESSCHECK;
        GameMasterManager.instance.levelSelector.ShowCustomLevels();
        backPanel.gameObject.SetActive(false);
        GameMasterManager.instance.cameraManager.ShowPlayerCamera(false);

    }


    public void UploadPuzzlePackButton() {
        HideAll();
        workshopMenuButton.SetActive(true);
        GameMasterManager.instance.levelSelector.gameObject.SetActive(true);
        LevelSelector.selectingForWorkshop = true;
        GameMasterManager.currentGameMode = GameMode.WORKSHOPCOMPLETENESSCHECK;
        GameMasterManager.instance.levelSelector.ShowCustomLevelPacks();
        backPanel.gameObject.SetActive(false);
        GameMasterManager.instance.cameraManager.ShowPlayerCamera(false);


    }

    public void PickPuzzleForUpload(LevelInfo levelInfo) {

        GameMasterManager.instance.levelSelector.gameObject.SetActive(false);
        GameMasterManager.instance.StartCoroutine(GameMasterManager.instance.generator.GenerateLevel(levelInfo.saveFile, true, false));
        levelInfo.saveFile.isSteam = true;
        ShowUploadPuzzleContext(levelInfo);
       // GameMasterManager.instance.cameraManager.ShowLevelViewerCamera();

    }


    public void PickPuzzlePackForUpload(LevelPack levelPack) {
        GameMasterManager.instance.levelSelector.gameObject.SetActive(false);
        ShowUploadPuzzlePackContext(levelPack);
    }

    public void ShowUploadPuzzlePackContext(LevelPack levelPack) {
        HideAll();
        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.ShowCursorIfNeeded();
        workshopMenuButton.SetActive(true);
        uploadScreen.SetActive(true);
    //    SteamWorkshopMain.Instance.IsDebugLogEnabled = true;

        string datapath = levelPack.levelPath;


     //   WorkshopItemUpdate createNewItemUsingGivenFolder = new WorkshopItemUpdate();
     //   createNewItemUsingGivenFolder.ContentPath = datapath;
     //   uploadUI.SetItemData(createNewItemUsingGivenFolder);
     //   uploadUI.SetPuzzlePackInfo(levelPack);

    }



    public void ShowUploadPuzzleContext(LevelInfo levelinfo) {
        HideAll();

        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.ShowCursorIfNeeded();

        workshopMenuButton.SetActive(true);
        uploadScreen.SetActive(true);


        // SteamWorkshopMain.Instance.IsDebugLogEnabled = true;

        // everything inside this folder will be uploaded with your item


        
        string datapath = SaveManager.GetCustomLevelPath(levelinfo.saveFile.levelName);

        // string datapath = Application.persistentDataPath;
        //if (Application.platform == RuntimePlatform.WindowsEditor) {
        //  datapath = Application.dataPath;
        //}

        
        // create dummy content to upload

        BlockSaveFile b = new BlockSaveFile(levelinfo.saveFile);
        b.associatedSteamID = "12345";

        string dummyItemContentStr = SaveManager.SerializeLevelFile(b);
        File.WriteAllText(datapath, dummyItemContentStr);




        // tell which folder you want to upload
        //WorkshopItemUpdate createNewItemUsingGivenFolder = new WorkshopItemUpdate();
        //createNewItemUsingGivenFolder.ContentPath = SaveManager.GetCustomLevelFolderPath() + "/" + levelinfo.saveFile.levelName;
        
        //uploadUI.SetItemData(createNewItemUsingGivenFolder);
        //uploadUI.SetPuzzleInfo(levelinfo);
    }


    public void HideAll() {
        GameMasterManager.instance.MainMenuButton.SetActive(false);
        workshopMenuButton.SetActive(false);
        workShopMainMenu.SetActive(false);
        uploadSelectPuzzleOrPackMenu.SetActive(false);
        downloadScreen.SetActive(false);
        uploadScreen.SetActive(false);
        backPanel.gameObject.SetActive(false);
        LevelSelector.selectingForWorkshop = false;
        GameMasterManager.instance.levelSelector.gameObject.SetActive(false);
    }


    public void DownloadButton() {
        HideAll();
        workshopMenuButton.SetActive(true);
        downloadScreen.SetActive(true);
    }

    

    public void MainWorkshopMenu() {
        HideAll();
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.cursorShouldBeShowing = true;
        CursorObject.ShowCursorIfNeeded();
        GameMasterManager.currentGameMode = GameMode.LEVELSELECT;
        GameMasterManager.instance.levelSelector.SetCustomGridSize();
        GameMasterManager.instance.MainMenuButton.SetActive(true);
        workShopMainMenu.SetActive(true);
        //if (SteamWorkshopUIUpload.tocheck != null) {
        //    uploadUI.RevertToCheckSteamStatus();
        //}
    }

    public StateMachine<WorkshopManager> manager;

    public void SelectPuzzleForUpload() {

        HideAll();

    }


    private void Update() {
        if (manager != null && manager.currentState != null)manager.Update();
    }

  
        public void SearchKeyboardinput(TMP_InputField input) {
            GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Search");
        }


    public void ItemNameinput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Item Name");
    }

    public void ItemDescriptioninput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Item Description");
    }

#endif
}