using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour {

    public LevelGroupButton groupButtonPrefab;
    public Transform levelGroupButtonSortingObject;
    public List<LevelGroupButton> levelGroupButtons = new List<LevelGroupButton>();

    public LevelButton levelButtonPrefab;
    public List<LevelButton> levelButtons = new List<LevelButton>();
    public Transform levelButtonSortingPoint;
    public GameObject levelSelectBar;
    public RectTransform levelCursor;
    public GridLayoutGroup gridGroup;

    [Header("Grid Info")]
    public int customWidth = 150;
    public int customCursorWidth = 165;
    public int storyWidth = 200;
    public int storyCursorWidth = 225;



    [Header("Level Info")]
    public GameObject LevelInfoWindow;
    public float ImageWidth = 550;
    public LevelDisplayImage displayImage;
    public TextMeshProUGUI levelName, levelDescription;

    [Header("Story")]
    public GameObject storyMenuButton;
    public GameObject storybar;
    public Image storyImage;
    public TextMeshProUGUI storyText;

    public List<NavigationObject> navList = new List<NavigationObject>();
    public NavigationManager manager;
    public int lastIndex = 0;

    [Header("Custom Levels")]
    public int customLevelsPerStage = 18;
    public LevelGroupButton customLevelButton;
    public Sprite defaultCustomLevelSprite;
    public Button pageUpButton, pageDownButton;
    public int currentPage;



    public GameObject pageButtonParent;

    [Header("Level Packs")]
    public LevelGroupButton levePackButton;
    public Transform levelPackPickerTransform;
    public LevelPackManager levelPackManager;



    [Header("Level Sprite")]
    public Sprite fourSprite;
    public Sprite fiveSprite, sixSprite, crashSprite, unionIcon, timerIcon;

    [Header("ProtoTypeLevelVariables")]
    public EnvironmentGroup protoTypeEnvironment;
    public int envSceneIndex = 0;

    public GameObject endCutsceneButton, startCutsceneButton;

    public void ShowStartCutscene() {
        GameMasterManager.instance.PlayStoryCutscene(Environment.instance.startCutscene);
    }

    public void ShowEndCutscene() {
        GameMasterManager.instance.PlayStoryCutscene(Environment.instance.endCutscen);
    }

    public void SelectLasttButton() {

        if (GameMasterManager.IsCurrentlyGamePad()) NavigationManager.SetSelectedObject(navList[lastIndex]);
        Hover(levelButtons[lastIndex]);
    }

    public void SetIndexButton(int index) {
        if (GameMasterManager.IsCurrentlyGamePad()) NavigationManager.SetSelectedObject(navList[lastIndex]);
        Hover(levelButtons[lastIndex]);
    }

    public RectTransform cursorRect;

    public int levelSize;
    public IEnumerator Setup() {
        prevGameMode = GameMode.STORYLEVEL;
        int index = 0;
        levelGroupButtons = new List<LevelGroupButton>();
        foreach (LevelGroup g in GameMasterManager.instance.levels) {


            g.MakeTexturesForAllLevels();
            if ( (!GameMasterManager.isDemo || (GameMasterManager.isDemo && GameMasterManager.instance.demoLevels.Contains(g)))) {

                LevelGroupButton b = Instantiate(groupButtonPrefab, levelGroupButtonSortingObject);
                b.Set(g, index);
                b.selector = this;
                levelGroupButtons.Add(b);
                if (g.excludeFromLevelSelect) b.gameObject.SetActive(false);
                index++;
            }

        }
        navList = new List<NavigationObject>();

        customLevelButton.transform.SetAsFirstSibling();
        levePackButton.transform.SetAsFirstSibling();
        for (int i = 0; i < customLevelsPerStage; i++) {
            LevelButton b = Instantiate(levelButtonPrefab, levelButtonSortingPoint);
            levelButtons.Add(b);
            b.SetDefaultWidth(gridGroup.cellSize.x);
            navList.Add(b.navObject);
        }

        yield return null;
        // SetLevelGroup(GameMasterManager.gameSaveFile.currentSelectedLevelGroup);
    }

    public static bool selectingForWorkshop = false;

    public void SelectLevelButton(LevelButton levelButton) {

        if (selectingForWorkshop) {
#if !DISABLESTEAMWORKS
            GameMasterManager.instance.workShopManager.PickPuzzleForUpload(levelButton.levelInfo);
#endif
            selectingForWorkshop = false;
            return;
        }


        if (GameMasterManager.currentGameMode == GameMode.EDITOR || prevGameMode == GameMode.EDITOR) {
            gameObject.SetActive(false);
            BlockSaveFile b = new BlockSaveFile(levelButton.levelInfo.saveFile);
            b.environmentName = null;
            GameMasterManager.instance.creator.LoadBlockSaveFile(b);
            GameMasterManager.instance.creator.ReturnFromDoNothingState();
            GameMasterManager.instance.MainMenuButton.gameObject.SetActive(true);
            GameMasterManager.instance.ShowGamePadCursor();

        } else if (GameMasterManager.currentGameMode == GameMode.ADDLEVELPACKBUTTON || (GameMasterManager.currentGameMode == GameMode.LEVELPACK && prevGameMode == GameMode.ADDLEVELPACKBUTTON)) {
            GameMasterManager.instance.SelectLevelToAddToPack(levelButton.levelInfo);
        } else {
            if (!levelButton.canplay) return;
            if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) GameMasterManager.currentLevelPackIndex = levelButton.index;
            GameMasterManager.instance.isCurrentlyCustomLevel = levelButton.isCustom;
            SetLastButton(levelButton);
            GameMasterManager.currentLevelGroup = levelButton.group;
            LoadLevelInfo(levelButton.index, levelButton.isCustom);
        }
    }

    



    internal void LoadLevelInfo(LevelInfo blockSaveFile) {
        GameMasterManager.instance.StartLevel(blockSaveFile.saveFile, blockSaveFile, 1000000000, true);
    }


    public void LoadLevelInfo(int index, bool isCustom) {

        //LevelInfo l = GameMasterManager.currentLevelGroup.levels[index];
        LevelInfo l = levelButtons[index].levelInfo;
        BlockSaveFile b = l.saveFile;
        GameMasterManager.instance.StartLevel(b, l, index, isCustom);
    }

    internal void SetLastButton(LevelButton levelButton) {
       
        lastIndex = levelButton.index;
    }

    public LevelGroupButton currentLevelGroupButton;

    public void ShowCustomLevels() {
        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = false;
        CursorObject.ShowCursorIfNeeded();

        storyMenuButton.SetActive(false);
        storybar.SetActive(false);
        levelGroupButtonSortingObject.gameObject.SetActive(true);
       // Debug.Log("SHOWC" + Time.time);
        currentLevelGroupButton = customLevelButton;
        levelPackPickerTransform.gameObject.SetActive(false);
        levelButtonSortingPoint.gameObject.SetActive(true);
        LevelInfoWindow.gameObject.SetActive(true);
        SetCursor(customLevelButton, false);
        LoadCustomLevelsOnPage(0, GetCustomLevels());
        SelectFirstLevel();
        SetLastLevelSelectIndex(1);

        HideCutsceneButtons();
    }

    public void HideCutsceneButtons() {
        endCutsceneButton.SetActive(false);
        startCutsceneButton.SetActive(false);
    }

    public void SetCutSceneButtons() {

        HideCutsceneButtons();

        if (GameMasterManager.IsEditor()) {
            if (Environment.instance.startCutscene != null) {
                startCutsceneButton.SetActive(true);
            }
            if (Environment.instance.endCutscen != null) {
                endCutsceneButton.SetActive(true);
            }
        }
        }

        public void StartCutsceneButton() {
        GameMasterManager.instance.PlayStoryCutscene(Environment.instance.startCutscene);
    }

    public void EndCutsceneButton() {
        GameMasterManager.instance.PlayStoryCutscene(Environment.instance.endCutscen);
    }



    public void SetLastLevelSelectIndex(int i) {
       // PlayerPrefs.SetInt(GameMasterManager.LEVELSELECTPLAYERPREF, i);
    }

    GameMode prevGameMode;

    public void ShowSelectedCustomLevelPack(LevelPack levelPack) {


        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = false;
        CursorObject.ShowCursorIfNeeded();


        SetCustomGridSize();
        storyMenuButton.SetActive(false);
        storybar.SetActive(false);
       
        currentLevelGroupButton = levePackButton;
        levelPackPickerTransform.gameObject.SetActive(false);
        levelButtonSortingPoint.gameObject.SetActive(true);
        LevelInfoWindow.gameObject.SetActive(true);
        List<LevelInfo> ls = levelPack.GetSaveFiles();
        prevGameMode = GameMasterManager.currentGameMode;
        GameMasterManager.currentGameMode = GameMode.LEVELPACK;
        GameMasterManager.currentLevelPack = levelPack;
        GameMasterManager.currentLevelPackLevels = ls;
        GameMasterManager.currentLevelPackIndex = 0;
        GameMasterManager.currentLevelPackProgressFile = levelPack.GetProgressFile();
        LoadCustomLevelsOnPage(0, ls);
        SelectFirstLevel();

        HideCutsceneButtons();


        //SetCursor(levePackButton, false);

    }



    public void ShowCustomLevelPacks() {
        SetCustomGridSize();

        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.ShowCursorIfNeeded();

        storyMenuButton.SetActive(false);
        storybar.SetActive(false);
        levelGroupButtonSortingObject.gameObject.SetActive(true);
        currentLevelGroupButton = levePackButton;
        levelPackPickerTransform.gameObject.SetActive(true);
        levelButtonSortingPoint.gameObject.SetActive(false);
        LevelInfoWindow.gameObject.SetActive(false);
        levelPackManager.ShowLevelPacks();
        SetCursor(levePackButton, false);
        pageButtonParent.SetActive(false);
        HoverExit();
        SetLastLevelSelectIndex(0);
        HideCutsceneButtons();

    }
    int maxPage = 0;
    public void CustomPageDown() {
        maxPage = (SaveManager.loadedLevelInfos.Count / customLevelsPerStage) + 1;
        if (currentPage < maxPage - 1) {
            LoadCustomLevelsOnPage(currentPage + 1, levels);
        }
        DeSelectUpDown();
    }

    public void CustomPageUp() {
        if (currentPage > 0) {
            LoadCustomLevelsOnPage(currentPage - 1, levels);
        }
        DeSelectUpDown();
    }

    public void DeSelectUpDown() {
        NavigationManager.DeselectObject();
    }

    public static bool spawnedYet = false;

    public void SetLevelGroupNoNavigationButtons(LevelGroup group) {
        //SetLevelGroup(GameMasterManager.instance.availableLevels.IndexOf(group));
        HoverExit();
    }
  

    public void SetDefaults(float f) {
        foreach (LevelButton b in levelButtons) {
            b.SetDefaultWidth(f);
        }
    }

    public void SelectFirstLevel() {
        int i = 0;
        if (BlockLevelGenerator.lastSelected != null) {
            foreach (LevelButton b in levelButtons) {
                if (b.levelInfo.saveFile.GetSaveKey() == BlockLevelGenerator.lastSelected.GetSaveKey()) {
                    i = levelButtons.IndexOf(b);
                }
            }
            BlockLevelGenerator.lastSelected = null;
        }

        SetLastButton(levelButtons[i]);
        SelectLasttButton();
    }

    public void SetCustomGridSize() {
        
        gridGroup.cellSize = new Vector2(customWidth, customWidth);
        SetDefaults(customWidth);
        cursorRect.sizeDelta = new Vector2(customCursorWidth, customCursorWidth);
        NavigationManager.SetupNavigationGrid(navList, 7);
        gridGroup.childAlignment = TextAnchor.UpperLeft;
        gridGroup.padding.left = 65;
    }

    public void SetStoryGridSize() {
        levelCursor.gameObject.SetActive(false);
        gridGroup.cellSize = new Vector2(storyWidth, storyWidth);
        SetDefaults(storyWidth);
        cursorRect.sizeDelta = new Vector2(storyCursorWidth, storyCursorWidth);
        NavigationManager.SetupNavigationGrid(navList, 5);
        gridGroup.childAlignment = TextAnchor.UpperLeft;
        gridGroup.padding.left = 15;
    }


    public void SetLevelGroup(LevelGroupButton b) {
        levelPackPickerTransform.gameObject.SetActive(false);
        LevelInfoWindow.gameObject.SetActive(true);
        levelButtonSortingPoint.gameObject.SetActive(true);

        HideCutsceneButtons();

        LevelGroup g = null;
        if ( b == customLevelButton ) {
            currentLevelGroupButton = customLevelButton;
            ShowCustomLevels();
            SetCustomGridSize();
            return;
        } else if (b == levePackButton){
            currentLevelGroupButton = levePackButton;
            ShowCustomLevelPacks();
            return;
        } else {
            

            g = b.group;
            currentLevelGroupButton = b;
        }

        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = false;
        CursorObject.ShowCursorIfNeeded();


        PlayerPrefs.SetInt(GameMasterManager.LEVELSELECTPLAYERPREF, levelGroupButtons.IndexOf(b) + 2);
       

        
        
        pageButtonParent.SetActive(false);
        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
           // Debug.Break();
            
            SetStoryGridSize();
            SetCutSceneButtons();
            
        } else {
            SetCursor(currentLevelGroupButton, false);
            SetCustomGridSize();
            //Debug.Break();

        }
        bool added = false;
        foreach (LevelButton bb in levelButtons) {
            bb.gameObject.SetActive(false);
        }
       
        
        int j = 0;

        List<LevelInfo> lvList = new List<LevelInfo>();

      
            foreach (LevelInfo l in g.levels) {
                levelButtons[j].gameObject.SetActive(true);
                levelButtons[j].SetLevelInfo(l, j, this, g, false);
                j++;
            }
        
        currentPage = 0;
        pageUpButton.gameObject.SetActive(false);
        pageDownButton.gameObject.SetActive(false);
        storyImage.sprite = g.iconSprite;
        storyText.text = g.envGroup.GetName() + " - " + g.GetLevelName();
        SelectFirstLevel();
        if (GameMasterManager.IsCurrentlyGamePad()) {
           // SelectFirstLevel();
        } else {
          //  HoverExit();
        }
        
        
    }

    public void Hover(LevelButton button) {
        LevelInfoWindow.gameObject.SetActive(true);
        levelName.text = button.levelInfo.GetName();
        displayImage.SetLevel(button.levelInfo);
        //displayImage.sprite = button.levelInfo.sprite;
        levelDescription.text = button.levelInfo.saveFile.levelDescription;
        StartCoroutine(MoveCursorNum(button));
       

    }

    LevelButton lastMovedButton;

    IEnumerator MoveCursorNum(LevelButton button) {
        cursorRect.transform.localScale = Vector3.zero;
        yield return null;
        cursorRect.transform.localScale = Vector3.one;
        cursorRect.gameObject.SetActive(true);
        cursorRect.transform.position = button.transform.position;
        lastMovedButton = button;
    }

    public void HoverExit() {
        cursorRect.gameObject.SetActive(false);
        LevelInfoWindow.gameObject.SetActive(false);
    }


    public List<LevelInfo> levels = new List<LevelInfo>();

    public void SetCursor(LevelGroupButton button, bool OffByOne) {
        if (!spawnedYet) {
            levelCursor.gameObject.SetActive(false);
            GameMasterManager.instance.StartCoroutine(SpawnOne(button));
        } else {
           
        }
        //spawnedYet = true;
    }

    public IEnumerator SpawnOne(LevelGroupButton button) {
        //Debug.LogError("Here");
        levelCursor.gameObject.SetActive(false);
        yield return null;
        levelCursor.gameObject.SetActive(true);
        levelCursor.transform.position = button.transform.position;
    }


    public List<LevelInfo> GetCustomLevels() {
        return new List<LevelInfo>(SaveManager.loadedLevelInfos.Values);
    }

    public void LoadCustomLevelsOnPage(int amt, List<LevelInfo> customLevels) {
        
        //SetCursor(customLevelButton, false);
        SaveManager.LoadCustomLevels();
       // Debug.Log("Here bitch");
        currentPage = amt;
        foreach (LevelButton b in levelButtons) {
            b.gameObject.SetActive(false);
        }
        pageButtonParent.SetActive(false);
        int startIndex = amt * customLevelsPerStage;
        if (startIndex == 0) {
            pageUpButton.gameObject.SetActive(false);
        } else {
            pageUpButton.gameObject.SetActive(true);
            pageButtonParent.SetActive(true);
        }
        int endIndex = startIndex + customLevelsPerStage;
        levels = new List<LevelInfo>(customLevels);
        if (endIndex >= levels.Count) {
            endIndex = levels.Count;
            pageDownButton.gameObject.SetActive(false);
        } else {
            pageDownButton.gameObject.SetActive(true);
            pageButtonParent.SetActive(true);
        }
        int iter = 0;
        for (int i = startIndex; i < endIndex; i++) {

            levelButtons[iter].gameObject.SetActive(true);
            levelButtons[iter].SetLevelInfo(levels[i], iter, this, null, true);
            iter++;
        }

        if (levelButtons.Count > 0) Hover(levelButtons[0]);


    }

    public int GetLevelIndex() {
        if (currentLevelGroupButton == customLevelButton) {
            return 1;
        }
        if (currentLevelGroupButton == levePackButton) {
            return 0;
        }
        if (currentLevelGroupButton == null) {
            Debug.LogError("NULL FOR SOME REASON" + Time.time.ToString());
            return 1;
        }
        return levelGroupButtons.IndexOf(currentLevelGroupButton) + 2;
        
    }

    internal void LevelSelectLeft() {

        int currentLevelIndex = GetLevelIndex();


        if (currentLevelIndex == 2) {
            ShowCustomLevels();
        } else if (currentLevelIndex == 1) {
            ShowCustomLevelPacks();
        } else if (currentLevelIndex <= 0) {
            SetLevelGroup(levelGroupButtons[levelGroupButtons.Count-1]);
        } else { 
            currentLevelIndex--;
            SetLevelGroup(levelGroupButtons[currentLevelIndex - 2]);
        }
    }

    internal void LevelSelectRight() {
        int currentLevelIndex = GetLevelIndex();

        if (currentLevelGroupButton == levePackButton) {
            ShowCustomLevels();
        } else if (currentLevelIndex >= levelGroupButtons.Count + 1) {
            ShowCustomLevelPacks();
        } else {
            currentLevelIndex++;
            SetLevelGroup(levelGroupButtons[currentLevelIndex - 2]);
        }
    }

    LevelButton toDeleteLevel;

    public void DeleteDialogue(LevelButton lastMovedButton) {
        toDeleteLevel = lastMovedButton;
        GameMasterManager.instance.confirmScreen.SetConfirmScreen("Delete Custom Level: " + lastMovedButton.levelInfo.GetName() + "?", new List<VoidDelegateString>() {
            new VoidDelegateString(DeleteYes, "Yes", true), new VoidDelegateString(DeleteNo, "No", true)

        });
        GameMasterManager.instance.confirmScreen.deleteScreen = true;
    }

    public void DeleteYes() {
        SaveManager.DeleteCustomLevel(toDeleteLevel.levelInfo);
        ShowCustomLevels();
    }

    public void DeleteNo() {

    }

    private void Update() {

        if (GameMasterManager.IsCurrentlyGamePad() && EventSystem.current.currentSelectedGameObject == null) {
            SetIndexButton(0);
        }


        if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT) {
          //  Debug.Break();
            if((GameMasterManager.instance.inputManger.hintDown || (Keyboard.current != null && (Keyboard.current.backspaceKey.wasPressedThisFrame || Keyboard.current.deleteKey.wasPressedThisFrame)) && lastMovedButton != null) ) {
               // Debug.Break();

                if (lastMovedButton != null && lastMovedButton.isCustom && currentLevelGroupButton == customLevelButton) {
                    DeleteDialogue(lastMovedButton);
                }

            }
              
            
        }

        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT) {
                GameMasterManager.instance.GoToMainMenu();
            } else if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
                GameMasterManager.instance.ShowStoryMenu();
            } else if (GameMasterManager.currentGameMode == GameMode.EDITOR) {
                GameMasterManager.instance.ShowGamePadCursor();
                gameObject.SetActive(false);
                GameMasterManager.instance.creator.ReturnFromDoNothingState();
                GameMasterManager.instance.MainMenuButton.SetActive(true);
            } else if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
                GameMasterManager.currentGameMode = prevGameMode;
                ShowCustomLevelPacks();
            } else if (GameMasterManager.currentGameMode == GameMode.ADDLEVELPACKBUTTON) {
                GameMasterManager.instance.GoBackToLevelPackCreator();
            }
        }
    }

    
}

