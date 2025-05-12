#if !DISABLESTEAMWORKS
using Steamworks;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JetBrains.Annotations;


[System.Serializable]
public enum StoryChapters { TUTORIAL = 0, CHALLENGE1 = 1, MURAL1 = 2, ADVANCEDTUTORIAL = 3, CHALLENGE2 = 4, MURAL2 = 5, EXTRATUTORIAL = 6, CHALLENGE3 = 7, MURAL3 = 8, FINALCHALLENGE = 9, BONUSCHALLENGE = 10, BONUSCHALLENGE2 = 11, CRASH1 = 12, CRASH2 = 13, CRASH3 = 14}


public class GameMasterManager : MonoBehaviour {
    

    public static string GetStartSceneString() {
        return "0-PictoPull";
    }



    public bool isAndroidEditor = true;
    public bool isSingleFileSaveMode = false;
    public const string KEYBOARDMOUSE = "KeyboardMouse", LEVELSELECTPLAYERPREF = "CurrentLevelSelectIndex", NINTENDOSWITCHPROCONTROLLER = "Switch Pro Controller", NINTENDOSWITCH = "Nintendo Switch";
    public static bool isDemo;
    public static GameMasterManager instance;
    public static GameMode currentGameMode;
    
    public Canvas canvas;
    public RectTransform scaleParent;
    public WinScreen winScreen;
    public BlockLevelGenerator generator;
    public BlockLevelCreator creator;
    public MainMenu mainMenu;
    public StoryController storyController;
    public GameObject optionsMenu;
    public LevelSelector levelSelector;
    public AddLevelPackScreen addLevelPackScreen;
    public LevelPackManager levelPackManager;
    public PauseScreen pauseScreen;
    public GameObject MainMenuButton;
    public GameObject gameplayUI;
    public Fader fader;
    public EnvironmentController environmentController;
    public ThreeDLevelCreator threeDLevelCreator;
    
    
    public CameraManager cameraManager;
    public AchievementTracker tracker;
    public static GameSaveFile gameSaveFile;
    public ButtonPromptManager promptManager;
    public NavigationManager navManager;
    public InputManager inputManger;
    public PlayerController playerController;
    public CustomLevelSelector customLevelSelect;
    public ViewAchievementScreen viewAchievementScreen;
    public WorkshopManager workShopManager; 
    public SteamManager steamManager;
    public Colors colors;
    public CutSceneManager cutsceneManager;
    public CharacterSelector characterSelector;
    public CharacterObject defaultCharObject;
    public ConfirmScreen confirmScreen;
    public OnScreenKeyBoardSceen keyboardScreen;
    [Header("NavigationButtons")]
    public NavigationObject firstMainMenuButton;
    public NavigationObject firstMainMenuButtonSwitch;
    


    internal static bool IsCurrentlyGamePad() {
        
        #if UNITY_SWITCH
        return true;
        #endif
        
        if (ButtonPromptManager.input == null) {
            //  Debug.Log("Null Return");
            return false;
        }
        return ButtonPromptManager.input.currentControlScheme != GameMasterManager.KEYBOARDMOUSE;
    }

    public static LevelPack currentLevelPack;
    public static int currentLevelPackIndex = 0;
    public static List<LevelInfo> currentLevelPackLevels = new List<LevelInfo>();
    public static LevelPackProgressFile currentLevelPackProgressFile;



    public static LevelGroup currentLevelGroup;
    public static LevelGroup envLevelGroup;
    public static LevelInfo currentLevel;
    public static LevelInfo nextLevel;
    public static int currentIndex;

    public List<LevelGroup> levels = new List<LevelGroup>();
    public List<LevelGroup> bonusLevels = new List<LevelGroup>();
    public List<LevelGroup> demoLevels = new List<LevelGroup>();
    public List<LevelGroup> availableLevels = new List<LevelGroup>();
    public List<LevelGroup> levelsStartUnlocked = new List<LevelGroup>();
    public CursorObject cursor;


    public LevelGroup overSizeLevelArea;

    public ButtonImagePrompt hintButton;



    public Toggle skipToggle, hideUIToggle;

    private void Awake() {
        SetFrameRate();
        instance = this;

        //Makes sure that the PC builds are never single save file mode
        if (!IsEditor()) {
            isSingleFileSaveMode = false;
        }

#if UNITY_SWITCH
        // We've got to set this up earlier than the coroutine for switch as things such as the audio manager
        // need to read from the save file (PlayerPrefs don't work on Switch) and the coroutine is too late
        gameSaveFile = SaveManager.LoadGameSaveFile();
        
        // Setting the switch build to always read custom levels from a single file
        isSingleFileSaveMode = true;
#endif
        StartCoroutine(StartUp());
        FixResolution();

    }

    public void SetFrameRate() {

        if (IsEditor()) {
            Application.targetFrameRate = -1;
        } else {
            QualitySettings.vSyncCount = 1;
            // Application.targetFrameRate = 60;
        }


    }


    public static bool isAndroid() {
        if (Application.platform == RuntimePlatform.Android) {
            return true;
        }
        if (IsEditor() && instance.isAndroidEditor) {
            return true;
        }
        return false;
    }


    public void FixResolution() {

        float sixteenten = 16f / 10f;
        float sixteennine = 16f / 9f;


        float height = Screen.height;
        float width = Screen.width;

        float aspectRatio = width / height;
        // print(aspectRatio + " aspect ratio");
        // print(sixteennine + " sixteennine");
        //  print(sixteenten + "sixteenten");

        if (aspectRatio == sixteennine) {
            scaleParent.localScale = Vector3.one;
        } else if (aspectRatio >= 2) {
            scaleParent.localScale = Vector3.one;
        } else {
            float amt = (1 / sixteennine) / (1 / aspectRatio);
            scaleParent.transform.localScale = Vector3.one * amt;
        }



    }


    public static List<T> GetComponentsInHoveredObjects<T>() where T : MonoBehaviour {
        List<T> res = new List<T>();


        return res;
    }

    public const string STEAMSCENE = "STEAM", DEMOSCENE = "DEMO";


    IEnumerator StartUp() {
#if !DISABLESTEAMWORKS
        SteamManager.isSteamBuild = false;
#endif
        isDemo = false;
        if (!IsEditor()) {
            playerController.debugText.enabled = false;
            playerController.debugText.gameObject.SetActive(false);

        }
        //Debug.Log(SceneManager.sceneCountInBuildSettings);
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            // Debug.Log(SceneUtility.GetScenePathByBuildIndex(i));
            if (SceneUtility.GetScenePathByBuildIndex(i).Contains(STEAMSCENE)) {
#if !DISABLESTEAMWORKS
                SteamManager.isSteamBuild = true;
#endif
            }
            if (SceneUtility.GetScenePathByBuildIndex(i).Contains(DEMOSCENE)) {
                isDemo = true;
            }
        }


        mainMenu.languageMenu.Setup();
        HideAll();
#if !DISABLESTEAMWORKS
        try {
            steamManager.Init();
        } catch {

        }
        while (SteamManager.isSteamBuild && !SteamManager.Initialized) {
            //  Debug.Log("in loop " + Time.time.ToString());
            yield return null;
        }
#endif
        yield return null;

        mainCanvas = canvas;
        colors.SetupColors();

        instance = this;
        promptManager.Initialize();
        cursor.Setup();

        if (isDemo) {
            // levels = demoLevels;
            availableLevels = demoLevels;
        } else {
            availableLevels = levels;
            foreach (LevelGroup g in bonusLevels) {
                availableLevels.Add(g);
            }
        }

        playerController.Setup();
        DontDestroyOnLoad(gameObject);
        
        
        #if !UNITY_SWITCH
        gameSaveFile = SaveManager.LoadGameSaveFile();
        #endif
        creator.Setup();
        machine = new StateMachine<GameMasterManager>(new MainMenuState(), this);
        yield return levelSelector.Setup();
        generator.Setup();

        string level = gameSaveFile.lastEnv;
        int idx = 0;
        if (level == null) {

        } else {
            foreach (LevelGroup g in levels) {
                if (g.envGroup.envKey.Equals(level)) {
                    idx = levels.IndexOf(g);
                    break;
                }
            }
        }
        GameMasterManager.currentLevelGroup = levels[idx];
        environmentController.SetEnvironment(levels[idx].envGroup);
        yield return null;
        GoToMainMenu();
        tracker.Setup();
        viewAchievementScreen.SetAchievements(tracker.achievements);
        mainMenu.Setup();
#if !DISABLESTEAMWORKS
        try {
            if (SteamManager.isSteamBuild && !GameMasterManager.isDemo) {
                tracker.ForceCheckAllSteamAchievements();
                SaveManager.EnsureSteamWorkshopItemsAreDownloaded();
            } else {

            }
        } catch (Exception e) {
            Debug.LogError(e.StackTrace);
        }
#endif


        demoWindow.SetActive(isDemo);
#if !DISABLESTEAMWORKS
        if (!SteamManager.isSteamBuild && isDemo) {
            //buyOnItchButton.SetActive(true);

        } else {
            buyOnItchButton.SetActive(false);


        }
#endif

        if (isDemo) {
            // soundButton.SetActive(false);
        } else {
            //soundButton.SetActive(false);
        }

        if (GameMasterManager.IsEditor()) {
            navManager.debugText.enabled = true;
        } else {
            navManager.debugText.enabled = false;
        }

        cutsceneManager.HideCutsceneManager();

        CharacterObject c = characterSelector.GetCharacterFromKey(gameSaveFile.avatarID);
        if (c == null) c = defaultCharObject;

        SelectCharacter(c, false);
        SaveManager.LoadCustomLevels();
        levelPackManager.UpdateLevelPacks();

        skipToggle.SetIsOnWithoutNotify(gameSaveFile.skipCutscenes);
        hideUIToggle.SetIsOnWithoutNotify(gameSaveFile.hidegameplayUI);
        storyController.SetMainLevels();

        //Debug.LogError("End Start Up " + Time.time.ToString());
        GoToMainMenu();
        

    }

    public void SelectLevelToAddToPack(LevelInfo levelInfo) {
        HideAll();
        addLevelPackScreen.gameObject.SetActive(true);
        addLevelPackScreen.AddLevelToCurrentPack(levelInfo);
    }

    public static bool IsEditor() {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor) return true;

        return false;
    }

    public static void SelectCharacter(CharacterObject o, bool saveAfter) {
        instance.playerController.SetCharacterObject(o);
        gameSaveFile.avatarID = o.GetKey();
        if (saveAfter) instance.SaveGameFile();
    }


    IEnumerator SetStoryLevel(LevelGroup group) {

        HideAll();


        if (Environment.instance.associatedLevelGroups.Contains(group)) {

            string cID = null;
            if (Environment.instance.startCutscene != null) {
                Debug.LogError("Here1");
                cID = Environment.instance.startCutscene.cutsceneid;
            }
            Debug.LogError("Here2");
            StoryCutScene s = Environment.instance.startCutscene;

            if (GameMasterManager.gameSaveFile.viewedStoryCutscenes == null) GameMasterManager.gameSaveFile.viewedStoryCutscenes = new HashSet<string>();

            Debug.LogError(cID + " " + currentGameMode.ToString());

            if (cID != null && !GameMasterManager.gameSaveFile.viewedStoryCutscenes.Contains(cID) && s !=null && currentGameMode == GameMode.STORYLEVEL) {
                Debug.LogError("Here3");
                yield return StoryCutsceneNum(s, true);
            }
        }

        




        playerController.HideInteractionText();
        currentGameMode = GameMode.STORYLEVEL;
        //  Debug.LogError("Here");
        //  if (environmentController.currentEnvironmentGroup != group.envGroup) {
        //     fader.InstantlySetClear();
        //  Debug.LogError("Here2");
        //    GameMasterManager.currentLevelGroupIndex = GameMasterManager.instance.availableLevels.IndexOf(group);
        //    GameMasterManager.currentLevelGroup = group;
        //   environmentController.SetEnvironment(group.envGroup);
        //   while (environmentController.working) {
        //     yield return null;
        //    }
        //    } else {

        //  }

        //  Debug.LogError("Here3");


        
        //levelSelector.SetLastLevelSelectIndex();
        GameMasterManager.currentLevelGroup = group;

        Debug.Log("2");


        GoToLevelSelect();
        yield return null;

    }

    public void DeselectButton() {
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void SetStoryLevelGroup(LevelGroup levelGroup) {

        Debug.Log("1");
        StartCoroutine(SetStoryLevel(levelGroup));

    }

    public void ShowOptionsMenu() {
        ShowGamePadCursor();
        optionsMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }

    public void HideGamePadCursor() {
        GameMasterManager.showCursorWithGamePad = false;
        CursorObject.ShowCursorIfNeeded();
    }
    
    public void ShowGamePadCursor() {
        // NavigationManager.instance.currentNavigationObject = null;
        NavigationManager.StopNavigation();
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.cursorShouldBeShowing = true;
        CursorObject.ShowCursorIfNeeded();
    }

    public void HideOptionsMenu() {

        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        HideGamePadCursor();
        if (GameMasterManager.IsCurrentlyGamePad()) NavigationManager.SetSelectedObject(mainMenu.optionsButton);
    }

    private void Update() {
        if (machine != null && machine.currentState != null) machine.Update();
    }

    public void SaveGameFile() {
        SaveManager.SaveGameFile(gameSaveFile);
    }

    public StateMachine<GameMasterManager> machine;

    public static LevelGroup nextLevelGroup;

    public static bool skipLevel = false;

    internal void StartLevel(BlockSaveFile b, LevelInfo levelInfo, int index, bool isCustom) {
        skipLevel = false;

        currentIndex = index;
        currentLevel = levelInfo;
        nextLevelGroup = null;
        if (isCustom) {
            nextLevel = null;
        } else {

            

            if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL && index < (GameMasterManager.currentLevelGroup.levels.Count - 1)) {
                GameMasterManager.nextLevel = GameMasterManager.currentLevelGroup.levels[index + 1];
            } else if (GameMasterManager.currentGameMode == GameMode.LEVELPACK && (GameMasterManager.currentLevelPackIndex + 1) < GameMasterManager.currentLevelPackLevels.Count) {
                nextLevel = GameMasterManager.currentLevelPackLevels[GameMasterManager.currentLevelPackIndex + 1];
            } else GameMasterManager.nextLevel = null;

            if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL && GameMasterManager.nextLevel == null) {
                if (GameMasterManager.instance.availableLevels.Contains(currentLevelGroup)) {
                    int idx = GameMasterManager.instance.availableLevels.IndexOf(currentLevelGroup);
                    idx++;
                    if (idx < GameMasterManager.instance.availableLevels.Count) {
                        nextLevelGroup = GameMasterManager.instance.availableLevels[idx];
                        nextLevel = nextLevelGroup.levels[0];
                        skipLevel = true;
                    }
                }
            }

        }
        machine.ChangeState(new PlayGameState(b, levelInfo, index));
        generator.SetPlayerAtStartPostion();
        //gameplayUI.SetActive(true);
        // generator.player.SetIdleUI();
        //  GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
        //  generator.player.StartMoving();
    }

    public void HideAll() {
        //Debug.Log("HIDEALL");
        //showCursorWithGamePad = false;
        winScreen.ImmediatelyHideWinScreen();
        // playerController.gameObject.SetActive(false);
        //generator.gameObject.SetActive(false);
        creator.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        levelSelector.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);
        storyController.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        gameplayUI.SetActive(false);
        customLevelSelect.gameObject.SetActive(false);
        viewAchievementScreen.gameObject.SetActive(false);
#if !DISABLESTEAMWORKS
        workShopManager.gameObject.SetActive(false);
#endif
        cutsceneManager.HideCutsceneManager();
        characterSelector.Hide();
        addLevelPackScreen.gameObject.SetActive(false);
        confirmScreen.gameObject.SetActive(false);
        threeDLevelCreator.gameObject.SetActive(false);
        threeDLevelCreator.SetUI(false);
#if !UNITY_SWITCH
        keyboardScreen.gameObject.SetActive(false);
        cutsceneManager.HideCutsceneManager();
#endif
    }





    public void ShowWorkShop() {
        HideAll();

#if !DISABLESTEAMWORKS
        
        workShopManager.gameObject.SetActive(true);
        workShopManager.MainWorkshopMenu();
#endif
    }

    internal static void ModeDebugLog() {
        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
            Debug.Log("Story");
        } else if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT) {
            Debug.Log("Level Select");
        } else {
            Debug.Log("Editor");
        }
    }

    public void GoToMainMenu() {
        // Debug.Log("Call " + Time.time);
        // generator.CleanUp();
        machine.ChangeState(new MainMenuState());
        cameraManager.SetEnvCamera(false);
        PlayMenuTrack();
    }

    public void ShowAchievementScreen() {
        HideAll();
        ShowGamePadCursor();
        GameMasterManager.instance.SaveGameFile();
        viewAchievementScreen.gameObject.SetActive(true);
        viewAchievementScreen.SetAchievements(tracker.achievements);
        MainMenuButton.gameObject.SetActive(true);
    }

  


    public void ShowCharacterSelector() {
        HideAll();
        characterSelector.Activate();
        MainMenuButton.gameObject.SetActive(true);
        cameraManager.ShowCharacterCustomizationCamera();
        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.ShowCursorIfNeeded();



    }

    public void CustomLevelSelect() {
        HideAll();
        customLevelSelect.gameObject.SetActive(true);
        CursorObject.cursorShouldBeShowing = true;
        CursorObject.ShowCursorIfNeeded();
        customLevelSelect.Show();
        MainMenuButton.SetActive(true);
    }


    public void MakePullLevel() {
        BlockLevelCreator.playTestSaveFile = null;
        GoToCustomLevelCreator(LevelType.PULL, false);
    }

    public void MakeCrashLevel() {
        BlockLevelCreator.playTestSaveFile = null;
        GoToCustomLevelCreator(LevelType.CRASH, false);
    }

    public void Make3DCrashLevel() {
        BlockLevelCreator.playTestSaveFile = null;
        GoToCustomLevelCreator(LevelType.CRASH, true);
    }

    public void MakeStretchLevel() {
        BlockLevelCreator.playTestSaveFile = null;
        GoToCustomLevelCreator(LevelType.STRETCH, false);
    }



    

    public void GoToCustomLevelCreator(LevelType type, bool is3D) {
        currentGameMode = GameMode.EDITOR;
        // generator.CleanUp();
        
        machine.ChangeState(new LevelCreatorState(type, is3D));
    }

    public void GoToLevelSelectButton() {

        currentGameMode = GameMode.LEVELSELECT;
        GoToLevelSelect();
    }


    public void GoToLevelSelect() {
        //  generator.CleanUp();
        machine.ChangeState(new LevelSelectState());

        LevelGroupButton b;

        int index = 0;

        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
            Debug.Log("3");
            index = levels.IndexOf(currentLevelGroup) + 2;
        }

        if (StoryLevelButton.currentLevelButton != null) {
           // b = StoryLevelButton.currentLevelButton;
        }

        if (index == 0) b = levelSelector.customLevelButton;
        else if (index == 1) b = levelSelector.levePackButton;
        else {
            index -= 2;
        if (index < 0) index = 0;

            b = levelSelector.levelGroupButtons[index];
        }

        Debug.Log("4");


        levelSelector.SetLevelGroup(b);


    }

    public void QuitGame() {
        SaveGameFile();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }

    public void PlayCustomLevelTest(BlockSaveFile saveFile) {
        GameMasterManager.currentGameMode = GameMode.EDITOR;
        BlockLevelCreator.playTestSaveFile = saveFile;
        HideAll();
        generator.gameObject.SetActive(true);
        StartCoroutine(generator.GenerateLevel(BlockLevelCreator.playTestSaveFile, true, true));
        generator.SetPlayerAtStartPostion();
        GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
        generator.player.SetIdleUI();
        generator.player.StartMoving();
    }


    public void PlayCustomLevelTest() {
        PlayCustomLevelTest(creator.GetSaveFile());

    }
    public static bool paused = false;
    public bool isCurrentlyCustomLevel = false;



    public bool LevelGroupUnlocked(LevelGroup g) {
        if (!gameSaveFile.unlockedStoryChapters.Contains(g.StoryChapter)) {
            return false;
        }
        return true;
    }
    public LevelGroup GetLevelGroupFromString(string s) {
        if (s == null) return null;
        foreach (LevelGroup l in levels) {
            if (l.envGroup.envKey == s) {
                return l;
            }
        }
        return null;
    }


    public void ShowPauseScreen() {
        CursorObject.cursorShouldBeShowing = true;
        CursorObject.ShowCursorIfNeeded();
        paused = true;
        pauseScreen.gameObject.SetActive(true);
        pauseScreen.ShowScreen();
    }
    public void HidePauseScreen() {
        CursorObject.cursorShouldBeShowing = false;
        CursorObject.ShowCursorIfNeeded();
        paused = false;
        pauseScreen.HideScreen();
        GameMasterManager.instance.generator.timeController.recording = true;
    }


    public void ShowStoryMenu() {
        HideAll();
        machine.ChangeState(new StoryMenuState());
        storyController.gameObject.SetActive(true);
        storyController.SetUp();
        MainMenuButton.SetActive(true);
    }

    public void PlayStoryMenuButton() {
        HideAll();
        currentGameMode = GameMode.STORYLEVEL;
        storyController.gameObject.SetActive(true);
        storyController.SetUp();
        MainMenuButton.SetActive(true);
        machine.ChangeState(new StoryMenuState());
        // levelSelector.LoadLevelInfo(0, false);
    }

    public void Filler() {

    }

    public void UnlockAllLevelButton() {
        confirmScreen.SetConfirmScreen("Unlock All Levels?", new List<VoidDelegateString>() { new VoidDelegateString(UnlockAllLevels, "Yes"), new VoidDelegateString(Filler, "No") });
    }

    public void ClearSaveDataButton() {
        confirmScreen.SetConfirmScreen("Clear All Save Data?", new List<VoidDelegateString>() { new VoidDelegateString(ClearSaveData, "Yes"), new VoidDelegateString(Filler, "No") });
    }

    public void ClearAchievementsButton() {
        confirmScreen.SetConfirmScreen("Clear All Achievements?", new List<VoidDelegateString>() { new VoidDelegateString(ClearAchievementData, "Yes"), new VoidDelegateString(Filler, "No") });
    }



    public void ClearSaveData() {
        AchievementTrackerSaveFile s = gameSaveFile.trackerSaveFile;
        
        #if !UNITY_SWITCH
        gameSaveFile = SaveManager.CreateNewEmptyGameSaveFileWithCheckingDemo();
        #else
        gameSaveFile = SaveManager.CreateNewEmptyGameSaveFile();
        #endif

        gameSaveFile.trackerSaveFile = s;
        SaveGameFile();

        currentLevelGroup = storyController.currentButtons.First().levelGroup;
    }


    public void ClearAchievementData() {
        gameSaveFile.trackerSaveFile = new AchievementTrackerSaveFile();
        SaveGameFile();
#if !UNITY_WEBGL && !DISABLESTEAMWORKS
        if (!GameMasterManager.isDemo && SteamManager.isSteamBuild) Steamworks.SteamUserStats.ResetAllStats(true);
#endif
    }

    public void UnlockAllLevels() {
        foreach (LevelGroup g in levels) {
            if (!gameSaveFile.unlockedStoryChapters.Contains(g.StoryChapter)) {
                gameSaveFile.unlockedStoryChapters.Add(g.StoryChapter);
            }
            foreach (LevelInfo l in g.levels) {
                if (!gameSaveFile.completedStoryLevels.Contains(l.saveFile.GetSaveKey()) && !gameSaveFile.skippedLevels.Contains(l.saveFile.GetSaveKey())) gameSaveFile.skippedLevels.Add(l.saveFile.GetSaveKey());
            }
        }
        SaveGameFile();
    }


    public static bool showCursorWithGamePad = false;


    public void ControlsChanged() {

        CursorObject.ShowCursorIfNeeded();

       // Debug.Log(ButtonPromptManager.input.currentControlScheme + " = Current Control Scheme" + Time.time.ToString());

     //   foreach (InputDevice p in ButtonPromptManager.input.devices) {
         //   Debug.Log(p.displayName + " attached " + Time.time.ToString());
        //    if (p.displayName.Contains(NINTENDOSWITCH) && ButtonPromptManager.input.currentControlScheme == "Gamepad") {
         //   //    ButtonPromptManager.input.SwitchCurrentControlScheme(p);
         //   }

     //   }



        if (machine != null && machine.currentState != null) machine.currentState.AdaptToChangedControlScheme(promptManager, machine);
        if (customLevelSelect.gameObject.activeInHierarchy) customLevelSelect.InputCheck();

    }

    public AudioPlayer clickSound, moveSound;

    public void UIClickSound() {
        clickSound.Play();
    }

    public void UIMoveSound() {
        moveSound.Play();
    }


    internal static void AddUnlockedStoryLevel(LevelGroup level) {
        gameSaveFile.unlockedStoryChapters.Add(level.StoryChapter);
        instance.SaveGameFile();
    }

    public void ShowPlayerUI() {
        //GameMasterManager.instance.gameplayUI.SetActive(true);
        //  playerController.rewindUI.SetActive(false);
    }

    public void SelectLevelForEditor() {
        MainMenuButton.SetActive(false);
        levelSelector.gameObject.SetActive(true);
        GameMasterManager.currentGameMode = GameMode.EDITOR;
        GoToLevelSelect();
    }

    public void SelectLevelForLevelPack() {
        MainMenuButton.SetActive(false);
        levelSelector.gameObject.SetActive(true);
        GameMasterManager.currentGameMode = GameMode.ADDLEVELPACKBUTTON;
        GoToLevelSelect();
    }

    public static bool inGameArea = false;
    public static Canvas mainCanvas;
    public List<ControlVariable> controlsToCheck;


    public static string ReplacePrompts(string t) {
        string s = new string(t);
        foreach (ControlVariable c in GameMasterManager.instance.controlsToCheck) {
            if (s.Contains(ButtonPromptManager.ControlVariableToString(c))) {
                s = s.Replace(ButtonPromptManager.ControlVariableToString(c), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(c)));
            }
        }
        return s;
    }

    public void InGameEnter() {
        inGameArea = true;
        InGameCheck();
    }

    public void InGameExit() {
        inGameArea = false;
        InGameCheck();
    }


    public void InGameCheck() {
        if (playerController.InGame()) {
            if (inGameArea) {
                gameplayUI.SetActive(true);
            } else {
                gameplayUI.SetActive(false);
            }
        }
    }

    public void ShowLevelPackCreator(LevelPack levelPack) {
        HideAll();
        addLevelPackScreen.gameObject.SetActive(true);
        addLevelPackScreen.LoadLevelPack(levelPack);
    }

    public void GoBackToLevelPackCreator() {
        HideAll();
        addLevelPackScreen.gameObject.SetActive(true);

    }



    public void OpenURL(string s) {
#if !UNITY_SWITCH
        NavigationManager.StopNavigation();
        //NavigationManager.SetSelectedObject(null);
        Application.OpenURL(s);
        EventSystem.current.SetSelectedGameObject(null);
        #else
        SwitchWebApplet.Show(s);
#endif
    }

    public void SetSkyColor(Color skyColor) {
        Camera.main.backgroundColor = skyColor;
    }

    public void SetDefaultSkyColor() {
        Camera.main.backgroundColor = Colors.DEFAULTSKYCOLOR;
    }


    public void PlayMenuTrack() {
        AudioManager.PlayTrack(menuClip);
    }

    public void PlayEnvTrack() {
        if (Environment.instance != null) AudioManager.PlayTrack(Environment.instance.audioTrack);
    }

    public AudioClip menuClip;


    [Header("Demo Logic")]
    public GameObject demoWindow;
    public GameObject soundButton;
    public GameObject buyOnItchButton;


    public static int minWorkshopImageWidth = 300, minWorkshopImageheight = 300;
   

    public static Texture2D MakeWorkshopSprite(Sprite sprite) {


        int multamount = (int)Math.Min(minWorkshopImageWidth / sprite.rect.width, minWorkshopImageheight / sprite.rect.height) + 1;
        Debug.Log(multamount);

        int size = (int)MathF.Max(sprite.rect.width * multamount, sprite.rect.height * multamount);
        int xoffset = (size - ((int)sprite.rect.width) * multamount) / 2;
        int yoffset = (size - ((int)sprite.rect.height) * multamount) / 2;



       Texture2D t = new Texture2D(size, size);

        for (int i = 0; i < sprite.rect.width; i++) {
            for (int j = 0; j < sprite.rect.height; j++) {

                Color c = sprite.texture.GetPixels(i, j, 1, 1)[0];
                c.a = 1;
              //  print(i.ToString() + " " + j.ToString() + " " + c.ToString());
                for (int x = 0; x < multamount; x++) {
                    for (int y = 0; y < multamount; y++) {
                        t.SetPixel((i * multamount) + x + xoffset, (j * multamount) + y + yoffset, c);
                    }
                }
            }
        }
        t.Apply();
        return t;
    }


    public void PlayStoryCutscene(StoryCutScene s) {


        if (s == null) {
            return;
        }
        
        StartCoroutine(StoryCutsceneNum(s, true));



    }

    public IEnumerator StoryCutsceneNum(StoryCutScene s, bool levelSelectAfter) {
        HideAll();
        yield return s.CutScene();
        if (levelSelectAfter)levelSelector.gameObject.SetActive(true);
        GameMasterManager.TrackWatchedCutScene(s.cutsceneid);
        s.HideCutSceneProps();


    }

    public static void TrackWatchedCutScene(string cutsceneid) {
        if (gameSaveFile.viewedStoryCutscenes == null) gameSaveFile.viewedStoryCutscenes = new HashSet<string>();
        if (!gameSaveFile.viewedStoryCutscenes.Contains(cutsceneid)) gameSaveFile.viewedStoryCutscenes.Add(cutsceneid);
        instance.SaveGameFile();
    }

    public void SetSkipCutscenes(bool b) {
        gameSaveFile.skipCutscenes = b;
        SaveGameFile();
    }

    public void SetHideGameplayUI(bool b) {
        gameSaveFile.hidegameplayUI = b;
        SaveGameFile();
    }
    


}

public class MainMenuState : State<GameMasterManager> {
    public override void Enter(StateMachine<GameMasterManager> obj) {
        GameMasterManager.showCursorWithGamePad = false;
        GameMasterManager.currentGameMode = GameMode.MAINMENU;
        CursorObject.SetDefaultCursorSprite();
        obj.target.HideAll();
        obj.target.mainMenu.gameObject.SetActive(true);
        if (GameMasterManager.IsCurrentlyGamePad()) {
            NavigationManager.SetSelectedObject(GetFirstMainMenuButton(obj));
        }
        obj.target.playerController.DoNothingMenuState();
        CursorObject.ShowCursorIfNeeded();
        if (!GameMasterManager.IsCurrentlyGamePad()) {
            CursorObject.ShowCursor();
        }
    }


    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<GameMasterManager> obj) {
       // Debug.Log("fortnite");
        if (!ButtonPromptManager.input.currentControlScheme.Equals(GameMasterManager.KEYBOARDMOUSE) && !GameMasterManager.showCursorWithGamePad) {
           // Debug.Log("HERE FUCKER");
            NavigationManager.SetSelectedObject(GetFirstMainMenuButton(obj));
        } else {
            NavigationManager.StopNavigation();
        }
    }

    public NavigationObject GetFirstMainMenuButton(StateMachine<GameMasterManager> obj)
    {
#if !UNITY_SWITCH
        return obj.target.firstMainMenuButton;
#else
        return obj.target.firstMainMenuButtonSwitch;
#endif
    }
}



public class PlayGameState : State<GameMasterManager> {

    private float index;
    private BlockSaveFile saveFile;
    private LevelInfo levelInfo;

    public PlayGameState(BlockSaveFile b, LevelInfo l, int i) {
        saveFile = b;
        levelInfo = l;
        index = i;
    }

    public override void Enter(StateMachine<GameMasterManager> obj) {
        obj.target.HideAll();
        
       
        obj.target.generator.gameObject.SetActive(true);
        obj.target.StartCoroutine(obj.target.generator.GenerateLevel(saveFile, true, true));
        obj.target.generator.SetPlayerAtStartPostion();
       // obj.target.cameraManager.ShowPlayerCamera(true);

        

       
    }

    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<GameMasterManager> obj) {
        if (obj.target.pauseScreen.isActiveAndEnabled) {
            obj.target.pauseScreen.NavigationCheck();
        }
    }
}


public class WinScreenState : State<GameMasterManager> {
    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<GameMasterManager> obj) {
        if (obj.target.winScreen.isActiveAndEnabled) {
            obj.target.winScreen.InputCheck();
        }
    }
}


public class LevelSelectState : State<GameMasterManager> {
    public override void Enter(StateMachine<GameMasterManager> obj) {
        if (GameMasterManager.currentGameMode != GameMode.EDITOR)obj.target.HideAll();
        obj.target.levelSelector.gameObject.SetActive(true);
        obj.target.cameraManager.SetEnvCamera(true); 
        obj.target.playerController.DoNothingMenuState();
        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
            obj.target.MainMenuButton.SetActive(false);
            obj.target.levelSelector.SetStoryGridSize();
            obj.target.levelSelector.levelGroupButtonSortingObject.gameObject.SetActive(false);
            obj.target.levelSelector.storyMenuButton.SetActive(true);
            obj.target.levelSelector.storybar.SetActive(true);
        } else {
            obj.target.levelSelector.SetCustomGridSize();
            if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) GameMasterManager.currentGameMode = GameMode.LEVELSELECT;
            if (GameMasterManager.currentGameMode != GameMode.EDITOR) obj.target.MainMenuButton.SetActive(true);
            obj.target.levelSelector.levelGroupButtonSortingObject.gameObject.SetActive(true);
            obj.target.levelSelector.storyMenuButton.SetActive(false);
            obj.target.levelSelector.storybar.SetActive(false);
        }


        if (ButtonPromptManager.input.currentControlScheme != GameMasterManager.KEYBOARDMOUSE) {
            obj.target.levelSelector.SelectLasttButton();
        } else {
            NavigationManager.StopNavigation();
        }

    }

    float Timer = 0;

    public override void Update(StateMachine<GameMasterManager> obj) {
        if (  (obj.target.inputManger.cameraButtonDown || obj.target.inputManger.timeRewindButtonDown) && (GameMasterManager.currentGameMode == GameMode.LEVELSELECT || GameMasterManager.currentGameMode == GameMode.EDITOR) && Timer < Time.time && !obj.target.inputManger.jumpDown ) {
            if (obj.target.inputManger.cameraButtonDown) {
                
                obj.target.levelSelector.LevelSelectRight();
             //   Timer = Time.time + 0.5f;
            } else {
                obj.target.levelSelector.LevelSelectLeft();
              //  Timer = Time.time + 0.5f;
            }
        }
    }

    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<GameMasterManager> obj) {
        if (obj.target.levelSelector.gameObject.activeInHierarchy) {
            if (ButtonPromptManager.input.currentControlScheme != GameMasterManager.KEYBOARDMOUSE) {
                obj.target.levelSelector.SelectLasttButton();
            } else {
                NavigationManager.StopNavigation();
            }
        }
    }
}



public class LevelCreatorState : State<GameMasterManager> {
    private LevelType type;
    private bool is3D;

    public LevelCreatorState(LevelType type, bool is3D) {
        this.type = type;
        this.is3D = is3D;
    }

    public override void Enter(StateMachine<GameMasterManager> obj) {
        obj.target.HideAll();
        obj.target.creator.gameObject.SetActive(!is3D);
        obj.target.ShowGamePadCursor();
        //Debug.Log("HALLOO");
        obj.target.threeDLevelCreator.SetUI(is3D);

        


        if (!is3D) {
            obj.target.creator.CreateImage(type, is3D);
        } else {
            obj.target.threeDLevelCreator.SetDefaultLevel();
          //  obj.target.generator.gameObject.SetActive(false);
        }


        //obj.target.MainMenuButton.SetActive(true);
        obj.target.playerController.DoNothingMenuState();
        



    }
    public override void Exit(StateMachine<GameMasterManager> obj) {
        obj.target.HideGamePadCursor();
    }
}



public class StoryMenuState : State<GameMasterManager> {

    public override void Enter(StateMachine<GameMasterManager> obj) {
       // Debug.Log("ehrere sad a");
        obj.target.storyController.SelectCurrentfButton();
        obj.target.levelSelector.storyMenuButton.gameObject.SetActive(false);
        obj.target.storyController.SelectCurrentfButton();
        if (GameMasterManager.IsCurrentlyGamePad()) {
           
        }
    }


  

    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<GameMasterManager> obj) {
        if (GameMasterManager.IsCurrentlyGamePad()) {
            Debug.Log("HEREEEEE");
            obj.target.storyController.SelectCurrentfButton();
        } else {
            NavigationManager.StopNavigation();
            CursorObject.ShowCursorIfNeeded();
        }
    }

    public override void Update(StateMachine<GameMasterManager> obj) {
        if (obj.target.inputManger.jumpDown && GameMasterManager.IsCurrentlyGamePad()) {
            obj.target.storyController.SelectCurrentfButton();
        }
    }

}