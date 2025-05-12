using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.InputSystem;

using HSVPicker;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class BlockLevelCreator : MonoBehaviour {

    private const string LMB = "<sprite name=LMB>", RMB = "<sprite name=RMB>", MMB = "<sprite name=MMB>";
    public static LevelType levelType;
    public static bool is3D = false;
    public const int maxLevelSize = 100;

    public int defaultWidth = 10;
    public int defaultHeight = 10;

    public static float defaultImageHeight = 750, defaultImageWidth = 750f;

    [HideInInspector] public int width = 20;
    [HideInInspector] public int height = 20;

    public TMPro.TMP_InputField input, sizeX, sizeY, cutsceneid;
    public GameObject cutsceneidmaster;
    [Header("ColorPalettes")]
    public ColorPalette defaultColorPalette;
    public List<ColorPalette> presetColorPalettes = new List<ColorPalette>();
    public Transform colorPaletteSelectGroupingSpot;
    public ColorPaletteSelectButton colorPaletteSelectButtonprefab;
    public List<ColorPaletteSelectButton> colorPaletteSelectButtons = new List<ColorPaletteSelectButton>();
    public GameObject addDefaultPaletteButton;//addColorButton,
    public ColorPicker picker;
    [Header("Sections")]
    public Transform levelEditorUI;
    public BlockLevelGenerator levelGenerator;
    [HideInInspector] public bool middleMouseGoalBlocked;

    public List<Color> colors = new List<Color>();

    public int[,] colorGrid;
    public Image image;
    public RectTransform pixelCursor;
    public Image cursorImage;

    [Header("Goal")]
    public RectTransform goalMarker;
    public Vector2Int goalCoord;
    public Image[] coreCenterImages;
    public Image[] coreBorderImages;

    [Header("Grid")]
    public Image gridImage;
    public Image checkeredBack;

    [Header("Level Button")]
    public Transform colorButtonTransform;
    public ColorButton colorButtonPrefab;
    public List<ColorButton> colorButtons;


    [Header("ObjectImages")]

    public Transform drawImage;
    public ColorIcon pullOutFullImage, pullOutSingleImage;
    public ColorIcon ladderImage;
    public ColorIcon cannonImage, timerImage, unionImage, blockerImage, oppositeImage, cloudImage, subGoalImage, coinImage;
    Dictionary<Vector2Int, SpecialTile> specialTiles;
    Dictionary<Vector2Int, ColorIcon> imageTileIndicator;
    public int gridSquareWidth = 20;
    public static BlockSaveFile playTestSaveFile = null;
    public List<Image> colorIconImages = new List<Image>();

    public RectTransform toolCursor;
    public RectTransform pencilTransform, eraserTransform, goalTransform, pulloutSingleTransform, pulloutFullTransform, ladderTransform, bucketTransform, cannonTransform, blockerTransform, unionTransform, timerTransform, oppositeTransform, cloudTransform;
    public TextMeshProUGUI toolText, toolPromptText;
    public Sprite positiveSprite, negativeSprite;
    public Image oppositeImageToolIcon;

    public void SetToolCursor(RectTransform tr) {
        toolCursor.position = tr.position;
    }




    [Header("Validation Stuff")]
    public GameObject errorTextLabel;
    public TextMeshProUGUI errorText;
    public Button saveButton, playtestButton, exportJsonButton;



    List<SpecialTile> tile;
    public StateMachine<BlockLevelCreator> machine;


    [Header("Direction")]
    public GameObject directionTab;
    public Color defaultDirectionColor = Color.white;
    public Color selectDirectionColor = Color.red;
    public Image upButton, downButton, rightButton, leftButton, forwardButton, backButton;

    public TextMeshProUGUI upText, downText;

    public BlockDirection selectedDirection = BlockDirection.DOWN;
    //public GameObject directioncursor;

    [Header("Level Text Box Manager")]
    public LevelTextBoxManager textBoxManager;
    public int colorIndex = 0;
    public Transform colorIndexSelector;

    [Header("Change Area")]
    public GameObject noneText;
    public Image levelImage;
    public Dictionary<string, EnvironmentGroup> envDict = new Dictionary<string, EnvironmentGroup>();
    public GameObject areaSelectScreen;
    public List<SetAreaButton> areaButtons = new List<SetAreaButton>();
    public LevelGroup levelLevelGroup;

    [Header("Depth Number")]
    public Transform levelDepthCursor;
    public Transform threeNumber, fourNumber, fiveNumber, sixNumber;
    public DepthGrid depthGrid;

    [Header("Full Level Modifiers")]
    public GameObject LevelModifierScreen;
    public GameObject LevelModifierButton;
    public Toggle allTimerToggle, allUnionToggle;

    public OppositePairs currentOppositeInfo;
    public OppositeColorSetter oppositeSetter;

    public TextMeshProUGUI areaMaxSizeLabel;


    [Header("SingleLevelSaveStuff")]
    public List<GameObject> toHideOnSingleLevel = new List<GameObject>();

    public void ShowLevelModifierScreen(bool b) {
        LevelModifierScreen.SetActive(b);
    }

    public void SetAreaMaxSizeLabel() {

        LevelGroup group = levelLevelGroup;

        if (levelLevelGroup == null) group = GameMasterManager.currentLevelGroup;
        if (levelLevelGroup == null) group = Environment.instance.associatedLevelGroups[0];


        areaMaxSizeLabel.text = "Current max area size:\n" + group.envGroup.maxLevelSize.x.ToString() + " X " + group.envGroup.maxLevelSize.y.ToString();
    }

    public void Setup() {
        HideDirectionTab();
        HideAreaSelectScreen();
        ShowLevelModifierScreen(false);
        defaultImageHeight = image.rectTransform.rect.height;
        defaultImageWidth = image.rectTransform.rect.width;
        sizeX.characterValidation = TMP_InputField.CharacterValidation.Integer;
        sizeY.characterValidation = TMP_InputField.CharacterValidation.Integer;
        textBoxManager.HideAll();
        AddQuickLinks();
        SetColorPresets();
        if (GameMasterManager.IsEditor()) {
            cutsceneidmaster.SetActive(true);
        } else {
            cutsceneidmaster.SetActive(false);
        }
        if (GameMasterManager.instance.isSingleFileSaveMode) {
            foreach (GameObject g in toHideOnSingleLevel) {
                if (g != null) g.gameObject.SetActive(false);
            }
        }
        envDict = new Dictionary<string, EnvironmentGroup>();


    }


    public void SetNone() {
        SetArea(null);
    }

    public void SetArea(LevelGroup group) {
        if (group != null) {
            levelImage.sprite = group.iconSprite;
            noneText.SetActive(false);

        } else {
            levelImage.sprite = null;
            noneText.SetActive(true);

        }
        levelLevelGroup = group;
        SetAreaMaxSizeLabel();
        HideAreaSelectScreen();
        ErrorCheck();

    }


    public void HideHints() {
        textBoxManager.gameObject.SetActive(false);
    }

    public void ShowHints() {
        textBoxManager.gameObject.SetActive(true);
    }


    public void ShowAreaSelection() {
        foreach (SetAreaButton s in areaButtons) {
            s.UnlockOrNot();
        }
        areaSelectScreen.SetActive(true);
    }

    public void HideAreaSelectScreen() {
        areaSelectScreen.SetActive(false);
    }



    private void Update() {
        if (machine != null)
            machine.Update();

        //Added esc to exit per user request
        if (GameMasterManager.IsEditor() || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer) {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !GameMasterManager.instance.levelSelector.gameObject.activeInHierarchy && !inExplorerWindow) {
                GameMasterManager.instance.GoToMainMenu();
            }
        }

        if (!GameMasterManager.instance.MainMenuButton.gameObject.activeInHierarchy && !GameMasterManager.instance.levelSelector.gameObject.activeInHierarchy) GameMasterManager.instance.MainMenuButton.gameObject.SetActive(true);
        if (!(GameMasterManager.instance.playerController.machine.currentState is DeadState)) {
            GameMasterManager.instance.playerController.SetDead();
        }
        if (GameMasterManager.instance.gameplayUI.activeInHierarchy) GameMasterManager.instance.gameplayUI.SetActive(false);
        if (MiddleMouseButtonPressed() && !middleMouseGoalBlocked) {
            PlaceGoalAtCursor();
        }
       // if (readText) {
         //   ReadText(fileExplorer.fileName);
        //    readText = false;
       // }

    }

    public void ErrorCheck() {
        string s = "";
        bool canPublish = true;

        if (width <= 0 || height <= 0) {
            canPublish = false;
            s += "- Width/Height must be greater than 0\n";
        }


        if (width > maxLevelSize || height > maxLevelSize) {
            canPublish = false;
            s += "- Width/Height must be less than "+ maxLevelSize.ToString() +"\n";
        }

        if (input.text == "") {
            canPublish = false;
            s += "- Level can't have a blank name\n";
        }

        if (specialTiles != null && specialTiles.ContainsKey(goalCoord)) {
            canPublish = false;
            s += "- Goal block can't be in same place as special tile\n";
        }

        if (colorGrid != null) {

            if (colorGrid[goalCoord.x, goalCoord.y] == 0) {
                canPublish = false;
                s += "- Goal flag must be placed on a color block\n";
            } else if (goalCoord.y != (height - 1) && colorGrid[goalCoord.x, goalCoord.y] == colorGrid[goalCoord.x, goalCoord.y + 1]) {
                canPublish = false;
                s += "- Goal flag must have an empty space or different color block above it\n";
            }
        }
        #if !DISABLESTEAMWORKS
     
        #endif

        if (canPublish) {
            errorText.gameObject.SetActive(false);
            errorTextLabel.gameObject.SetActive(false);
            saveButton.interactable = true;
            exportJsonButton.interactable = true;
            playtestButton.interactable = true;
        } else {
            errorText.gameObject.SetActive(true);
            errorTextLabel.gameObject.SetActive(true);
            errorText.text = s;
            saveButton.interactable = false;
            exportJsonButton.interactable = false;
            playtestButton.interactable = false;
        }

        EnvironmentGroup gr = null;
        if (levelLevelGroup != null) {
            if (levelLevelGroup.envGroup != null) SetEnvColors(levelLevelGroup.envGroup);
        } else {
            if (GameMasterManager.currentLevelGroup != null && GameMasterManager.currentLevelGroup.envGroup != null)SetEnvColors(GameMasterManager.currentLevelGroup.envGroup);
        }


    }

    public Vector3 GridToWorld(Vector2Int v) {
        return new Vector3(v.x * pixelCursor.rect.width, v.y * pixelCursor.rect.width, 0);
    }

    public Vector2Int WorldToGrid(Vector3 v) {
        return new Vector2Int((int)(pixelCursor.anchoredPosition.x / pixelCursor.rect.width), (int)(pixelCursor.anchoredPosition.y / pixelCursor.rect.height));
    }

    int levelDepth;

    public void SetLevelDepthNumber(int i) {
        levelDepth = 3;
        levelDepthCursor.transform.SetParent(threeNumber);
        if (i == 4) {
            levelDepth = 4;
            levelDepthCursor.transform.SetParent(fourNumber);
        } else if (i == 5) {
            levelDepth = 5;
            levelDepthCursor.transform.SetParent(fiveNumber);
        } else if (i == 6) {
            levelDepth = 6;
            levelDepthCursor.transform.SetParent(sixNumber);
        }
        levelDepthCursor.transform.localPosition = Vector3.zero;
    }

    internal void SetHeightWidth(int v1, int v2) {
        colorGrid = new int[v1, v2];
        width = v1;
        height = v2;
        sizeX.text = v1.ToString();
        sizeY.text = v2.ToString();
    }

    public void MakeColorButtons() {

        foreach (ColorButton c in colorButtons) {
            c.gameObject.SetActive(false);
        }
        for (int i = colorButtons.Count; i < colors.Count; i++) {
            ColorButton c1 = Instantiate(colorButtonPrefab, colorButtonTransform);
            c1.creator = this;
            colorButtons.Add(c1);
        }

        for (int i = 0; i < colors.Count; i++) {
            colorButtons[i].gameObject.SetActive(true);
            colorButtons[i].SetColorIndex(colors[i], i);
        }

        SelectIndex(colorIndex, true);
        addDefaultPaletteButton.SetActive(!hasDefaultColors());
       // addColorButton.transform.SetAsLastSibling();
        addDefaultPaletteButton.transform.SetAsLastSibling();

    }

    public bool hasDefaultColors() {
        foreach (Color c in defaultColorPalette.colors) {
            if (!colors.Contains(c)) {
                return false;
            }
        }
        return true;
    }

    public void AddDefaultColors() {
        foreach (Color c in defaultColorPalette.colors) {
            if (!colors.Contains(c)) {
                colors.Add(c);
            }
        }
        MakeColorButtons();
    }


    public void AddPickerButton() {
        colors.Add(picker.CurrentColor);
        MakeColorButtons();

    }

    public void ReplacePickerIndex() {
        colors[colorIndex] = picker.CurrentColor;
        MakeColorButtons();
        RedrawTexture();
    }


    public void SetGridImage() {
        Texture2D tex = new Texture2D(width * gridSquareWidth, height * gridSquareWidth);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++) {
                for (int i = 0; i < gridSquareWidth; i++) {
                    for (int j = 0; j < gridSquareWidth; j++) {
                        if (i == 0 || i == (gridSquareWidth-1) || j == 0 || j == (gridSquareWidth-1)) {
                            tex.SetPixel((x * gridSquareWidth) + i, (y * gridSquareWidth) + j, Color.black);
                        } else {
                            tex.SetPixel((x * gridSquareWidth) + i, (y * gridSquareWidth) + j, new Color(0,0,0,0));
                        }
                    }
                }
            }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        Sprite s = Sprite.Create(tex, new Rect(new Vector2(), new Vector2(width * gridSquareWidth, height * gridSquareWidth)), new Vector2());
        gridImage.sprite = s;

        int amt = Math.Max(width, height);


        checkeredBack.pixelsPerUnitMultiplier = amt/10f;
        checkeredBack.rectTransform.sizeDelta = new Vector2(gridImage.rectTransform.rect.width, gridImage.rectTransform.rect.height);
        depthGrid.UpdateGrid();

    }


    public void PlayCustomLevelComplete() {
        BlockLevelGenerator.isCustomLevel = true;
        GameMasterManager.instance.PlayCustomLevelTest();
    }

    

    public void PlayTest() {

        String s = "";
        LevelGroup group;

        if (levelLevelGroup == null) s = null;
        else s = levelLevelGroup.envGroup.envKey;

        if (s == null) group = GameMasterManager.currentLevelGroup;
        else group = GameMasterManager.instance.GetLevelGroupFromString(s);

        Vector2Int v = new Vector2Int(width, height);
         

        if (group == null) {
            Environment e = Environment.instance;
            if (e == null) e = FindAnyObjectByType<Environment>();
            group = e.associatedLevelGroups[0];
        }

        if (group.envGroup.CanFitLevel(v)) {
            PlayCustomLevelComplete();
        } else {
            GameMasterManager.instance.confirmScreen.SetConfirmScreen("Level is Too Big. Current Level Size: " + width.ToString() + 
                                                                      "," + height.ToString() + 
                                                                      " Max Size For This Area: " + group.envGroup.maxLevelSize.x.ToString() + 
                                                                      "," + group.envGroup.maxLevelSize.y,
                                                                      new List<VoidDelegateString>() {
           // new VoidDelegateString(SwitchArea,"Switch To Big Enough Area",true),
            new VoidDelegateString(BackToArea, "Back To Editor", true)
            });
        }

        
    }

    public void SwitchArea() {

    }

    public void BackToArea() {

    }

    public static Color VectorToColor(SimplifiedVector3 c) {
        return new Color(c.x, c.y, c.z);
    }

    public static List<SimplifiedVector3> ConvertColorListToVector(List<Color> col) {
        List<SimplifiedVector3> v = new List<SimplifiedVector3>();
        foreach (Color c in col) {
            v.Add(new SimplifiedVector3(c.r,c.g,c.b));
        }
        return v;
    }

    public static List<Color> ConvertVectorListToColor(List<SimplifiedVector3> col) {
        List<Color> v = new List<Color>();
        foreach (SimplifiedVector3 c in col) {
            v.Add(new Color(c.x, c.y, c.z));
        }
        return v;
    }


    public static void ReloadSaveFiles(string pathTOModifiedFile) {
       // Debug.Log(pathTOModifiedFile + Time.time.ToString());

        foreach (KeyValuePair<string, LevelInfo> l in SaveManager.loadedLevelInfos) {
          //  Debug.Log("LEVEL: " + l.Key + " " + Time.time);
        }


        if (SaveManager.loadedLevelInfos.ContainsKey(pathTOModifiedFile)) {
            SaveManager.loadedLevelInfos.Remove(pathTOModifiedFile);
            
        }
        SaveManager.LoadCustomLevels();
    }

    public BlockSaveFile GetSaveFile() {
      //  Debug.Log("GET SAVE FILE " + Time.time);
        BlockSaveFile s = new BlockSaveFile();
        s.levelName = input.text;
        s.colorGrid = colorGrid;
        s.colors = ConvertColorListToVector(colors);
        s.width = width;
        s.height = height;
        s.goalSpot = goalCoord;
        s.levelType = levelType;
        s.cutsceneid = cutsceneid.text;
      //  print(((int)s.levelType).ToString());
        s.specialTile = new List<SpecialTile>();
        foreach (KeyValuePair<Vector2Int, SpecialTile> kv in specialTiles) {
           // Debug.Log("SaveAdded");
            s.specialTile.Add(kv.Value);
        }
        if (levelLevelGroup == null) {
            s.environmentName = null;
          //  SetEnvColors(GameMasterManager.envLevelGroup.envGroup);
        } else {
            s.environmentName = levelLevelGroup.envGroup.envKey;
           // SetEnvColors(levelLevelGroup.envGroup);
        }


        s.extraTags = new Dictionary<string, string>();

        s.AddDepthTag(levelDepth);
        if (allUnionToggle.isOn) s.AddAllUnion();
        if (allTimerToggle.isOn) s.AddAllTimer();

        s.oppositeInfo = oppositeSetter.GetPairs();
        s.messages = textBoxManager.GetStringList();
        s.depthGrid = new int[width, height];


        s.is3DLevel = is3D;
        if (s.is3DLevel) {
            s.threeDMap = new int[width, height, 3];

            for (int x = 0; x < s.threeDMap.GetLength(0); x++) {
                for (int y = 0; y < s.threeDMap.GetLength(1); y++) {
                    for (int z = 0; z < s.threeDMap.GetLength(2); z++) {
                        s.threeDMap[x, y, z] = 1;
                    }
                }
            }


        }

        for (int x =0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                s.depthGrid[x,y] = depthGrid.GetValue(x, y);
            }
        }




        return s;
    }


    public void SetEnvColors(EnvironmentGroup group) {

        foreach (Image i in coreBorderImages) {
            i.color = group.coreBorderColor;
        }
        foreach (Image i in coreCenterImages) {
            i.color = group.coreCenterColor;
        }

    }



    public void SetGoalCoord(Vector2Int v) {
        v = new Vector2Int(Mathf.Clamp(v.x, 0, width - 1), Mathf.Clamp(v.y, 0, height - 1));
        goalMarker.anchoredPosition =  GridToWorld(v);
        goalCoord = v;

        
        ErrorCheck();
       
        //print(Time.time);
    }

    public void DrawAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (colorGrid[x, y] != colorIndex + 1) {
            colorGrid[x, y] = colorIndex + 1;
            RedrawTexture();
        }
        ErrorCheck();
    }

    public void GrabColorAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (colorGrid[x, y] != colorIndex + 1 && colorGrid[x, y] > 0) {
            SelectIndex(colorGrid[x, y]-1, false);
            
        }
        ErrorCheck();
    }

    public void FillAllColorAtCursor(int v) {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (colorGrid[x, y] != v) {
            FillAllColorAtSpot(new Vector2Int(x, y), v);
            RedrawTexture();
        }
        ErrorCheck();
    }

    private void FillAllColorAtSpot(Vector2Int vector2Int, int v) {

        int originalcolor = colorGrid[vector2Int.x, vector2Int.y];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (colorGrid[x, y] == originalcolor) colorGrid[x, y] = v;
            }
        }
    }

    public void FillAtCursor(int v) {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (colorGrid[x, y] != v) {
            FillAtSpot(new Vector2Int(x, y), v);
            RedrawTexture();
        }
        ErrorCheck();
    }

    public void FillAtSpot(Vector2Int pos, int cIndex) {
        //Debug.Break();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        List<Vector2Int> toVisit = new List<Vector2Int>();
        toVisit.Add(pos);
        int originalColor = colorGrid[pos.x, pos.y];
        while (toVisit.Count > 0) {
            Vector2Int curr = toVisit[0];
            toVisit.RemoveAt(0);
            visited.Add(curr);
            colorGrid[curr.x, curr.y] = cIndex;
            List<Vector2Int> ns = GetNeighbours(curr);
            foreach (Vector2Int v in ns) {
                if (!visited.Contains(v) && !toVisit.Contains(v) && colorGrid[v.x, v.y] == originalColor) toVisit.Add(v);
            }
        }
    }


    public List<Vector2Int> GetNeighbours(Vector2Int coord) {
        List<Vector2Int> v = new List<Vector2Int>();

        if (coord.x > 0) v.Add(coord + Vector2Int.left);
        if (coord.y > 0) v.Add(coord + Vector2Int.down);
        if (coord.x < width-1) v.Add(coord + Vector2Int.right);
        if (coord.y < height - 1) v.Add(coord + Vector2Int.up);
        return v;
    }





    public void CreateImage(LevelType type, bool is3) {

       
        
       // levelGenerator.gameObject.SetActive(false);
        textBoxManager.HideAll();
        SetDirection((int)BlockDirection.DOWN);
        if (imageTileIndicator != null) {
            foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
                Destroy(kv.Value.gameObject);
            }
        } else {
            imageTileIndicator = new Dictionary<Vector2Int, ColorIcon>();
        }
        imageTileIndicator.Clear();
        if (specialTiles != null)specialTiles.Clear();

        bool canLoadSave = true;
        if (playTestSaveFile != null && (playTestSaveFile.width == 0 || playTestSaveFile.height == 0)) {
            canLoadSave = false;
        }
       // picker.gameObject.SetActive(false);
        levelType = type;
        is3D = is3;

        int[,] dGrid = new int[width, height];

        if (playTestSaveFile == null || !canLoadSave) {
            SetArea(null);
            //Debug.Log("NEW SAVE FILE " + Time.time);
            colorGrid = new int[10, 10];
            DefaultColors();
            SetGoalCoord(new Vector2Int(0, 0));
            input.text = "";
            width = defaultWidth;
            height = defaultHeight;
            SetLevelDepthNumber(3);
            currentOppositeInfo = null;
            oppositeSetter.Set(currentOppositeInfo);
            //Debug.Break();
            dGrid = new int[width, height];
            depthGrid.EnsureDepthGridIsClear();
        } else {
            SetArea(GameMasterManager.instance.GetLevelGroupFromString(playTestSaveFile.environmentName));
            colorGrid = playTestSaveFile.colorGrid;
            width = playTestSaveFile.width;
            height = playTestSaveFile.height;
            colors = ConvertVectorListToColor(playTestSaveFile.colors);
            MakeColorButtons();
            SetGoalCoord(playTestSaveFile.goalSpot);
            input.text = playTestSaveFile.levelName;
            textBoxManager.SetBasedOnStringList(playTestSaveFile.messages);
            SetLevelDepthNumber(playTestSaveFile.GetDepthNumber());
            currentOppositeInfo = playTestSaveFile.oppositeInfo;
            oppositeSetter.Set(currentOppositeInfo);
            dGrid = playTestSaveFile.depthGrid;
        }
        sizeX.text = width.ToString();
        sizeY.text = height.ToString();
        RedrawTexture();
        SelectIndex(0, true);

        newStart = new Vector3Int();
        newEnd = new Vector3Int();

        SetGridImage();
        
        machine = new StateMachine<BlockLevelCreator>(new PencilState(), this);

        imageTileIndicator = new Dictionary<Vector2Int, ColorIcon>();
        specialTiles = new Dictionary<Vector2Int, SpecialTile>();

        HideHints();

        if (playTestSaveFile != null && playTestSaveFile.specialTile != null) {
            specialTiles = new Dictionary<Vector2Int, SpecialTile>();
            foreach (SpecialTile s in playTestSaveFile.specialTile) {
                specialTiles.Add(s.position, s);
            }
            foreach (SpecialTile s in playTestSaveFile.specialTile) {

                if (s.id == SpecialTile.PULLOUTBUTTON) {
                    AddPulloutButton(s.position, s.colorIndex, s.direction, s.subid);
                } else if (s.id == SpecialTile.LADDERID) {
                    AddLadder(s.position, s.colorIndex, s.direction);
                } else if (s.id == SpecialTile.CANNONID) {
                    AddCannon(s.position, s.colorIndex, s.direction);
                } else if (s.id == SpecialTile.BLOCKERID) {
                    AddBlocker(s.position, s.colorIndex);
                } else if (s.id == SpecialTile.UNIONID) {
                    AddUnion(s.position, s.colorIndex);
                } else if (s.id == SpecialTile.TIMERID) {
                    AddTimer(s.position, s.colorIndex);
                } else if (s.id == SpecialTile.OPPOSITEID) {
                    AddOpposite(s.position, s.colorIndex, s.direction);
                } else if (s.id == SpecialTile.CLOUDID) {
                    AddCloud(s.position);
                } else if (s.id == SpecialTile.SUBGOALID) {
                    AddSubGoal(s.position);
                }
            }
        }

        if (playTestSaveFile != null) {
            allTimerToggle.SetIsOnWithoutNotify(playTestSaveFile.isAllTimer());
            allUnionToggle.SetIsOnWithoutNotify(playTestSaveFile.isAllUnion());
        } else {
            allUnionToggle.SetIsOnWithoutNotify(false);
            allTimerToggle.SetIsOnWithoutNotify(false);
        }

        depthGrid.SetGridValue(dGrid);
        HideDepthGrid();

        playTestSaveFile = null;

        if (levelType == LevelType.CRASH) {
            pulloutFullTransform.gameObject.SetActive(false);
            blockerTransform.gameObject.SetActive(false);
            oppositeTransform.gameObject.SetActive(false);
            timerTransform.gameObject.SetActive(false);
            unionTransform.gameObject.SetActive(false);
            cloudTransform.gameObject.SetActive(true);
        } else {
            pulloutFullTransform.gameObject.SetActive(true);
            blockerTransform.gameObject.SetActive(true);
            oppositeTransform.gameObject.SetActive(true);
            timerTransform.gameObject.SetActive(true);
            unionTransform.gameObject.SetActive(true);
            cloudTransform.gameObject.SetActive(false);
        }

        
        SetAreaMaxSizeLabel();
        

    }

    public void DefaultColors() {
        colors = new List<Color>(defaultColorPalette.colors);
        MakeColorButtons();
    }

    public void AddPrefabColorButton() {
        ColorPaletteSelectButton p = Instantiate(colorPaletteSelectButtonprefab, colorPaletteSelectGroupingSpot);
        p.gameObject.SetActive(false);
        p.creator = this;
        colorPaletteSelectButtons.Add(p);
    }

    public void SetColorPresets() {

        int toAdd = presetColorPalettes.Count - colorPaletteSelectButtons.Count;

        for (int i = 0; i < toAdd; i++) {
            AddPrefabColorButton();
        } 

        for (int i = 0; i < colorPaletteSelectButtons.Count; i++) {
            if (i < presetColorPalettes.Count) {
                colorPaletteSelectButtons[i].gameObject.SetActive(true);
                colorPaletteSelectButtons[i].Set(presetColorPalettes[i]);
            }
        }




    }

    public void OnOptionsChanged(int v) {
        if (v == 0) return;
        ApplyColorPalette(presetColorPalettes[v - 1]);
    }
    

    public void ApplyColorPalette(ColorPalette p ) {
        BlockSaveFile s = GetSaveFile();
        s = ApplyColorPalette(s, p);
        Load(s);
    }

    

    public static BlockSaveFile ApplyColorPalette(BlockSaveFile saveFile, ColorPalette palette) {

    //    print("HERE BOI");

        Dictionary<Color, int> colorCount = new Dictionary<Color, int>();
        Dictionary<Color, int> colorIndex = new Dictionary<Color, int>();
        Dictionary<int, Color> indexColor = new Dictionary<int, Color>();

        saveFile = new BlockSaveFile(saveFile);


        int idx = 0;
        for (int i = 0; i < saveFile.colors.Count; i++) {
            colorIndex.Add(VectorToColor(saveFile.colors[i]), idx);
            colorCount.Add(VectorToColor(saveFile.colors[i]), 0);
            indexColor.Add(idx, VectorToColor(saveFile.colors[i]));
            idx++;
        }

        for (int x = 0; x < saveFile.width; x++) {
            for (int y = 0; y < saveFile.height; y++) {
                if (saveFile.colorGrid[x,y] > 0) {
                    colorCount[indexColor[saveFile.colorGrid[x, y] - 1]]++;
                }
            }
        }

        Dictionary<int, int> colorSwitch = new Dictionary<int, int>();
        List<KeyValuePair<Color, int>> nums = new List<KeyValuePair<Color, int>>();

        foreach (KeyValuePair <Color, int> kv in colorCount) {
            nums.Add(kv);
        }

       

        nums.Sort(new VKColorIntComparison());

        for (int i = 0; i < saveFile.colors.Count; i++) {
            colorSwitch.Add(colorIndex[nums[i].Key], i);
        }

        for (int x = 0; x < saveFile.width; x++) {
            for (int y = 0; y < saveFile.height; y++) {
                if (saveFile.colorGrid[x, y] > 0) {
                    saveFile.colorGrid[x, y] = colorSwitch[saveFile.colorGrid[x, y] - 1] + 1;
                }
            }
        }

        saveFile.colors = ConvertColorListToVector(palette.colors);

        return saveFile;


    }

    public class VKColorIntComparison : Comparer<KeyValuePair<Color, int>> {
        public override int Compare(KeyValuePair<Color, int> x, KeyValuePair<Color, int> y) {
            return y.Value.CompareTo(x.Value);
        }
    }

    public void ReSize() {

        int newWidth = int.Parse(sizeX.text);
        int newHeight = int.Parse(sizeY.text);

        if (newWidth <= 0) newWidth = 1;
        if (newWidth >= maxLevelSize) newWidth = maxLevelSize;

        if (newHeight <= 0) newHeight = 1;
        if (newHeight >= maxLevelSize) newHeight = maxLevelSize;

        sizeX.SetTextWithoutNotify(newWidth.ToString());
        sizeY.SetTextWithoutNotify(newHeight.ToString());

        List<Vector2Int> tRemove = new List<Vector2Int>();
        
        //Get Rid Of cropped out specialTiles
        int[,] nGrid = new int[newWidth, newHeight];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (i < newWidth && j < newHeight) {
                    nGrid[i, j] = colorGrid[i, j];
                }
            }
        }
        width = newWidth;
        height = newHeight;
        colorGrid = nGrid;

        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            if (kv.Key.x >= width || kv.Key.y >= height) {
                Destroy(kv.Value.gameObject);
                tRemove.Add(kv.Key);
               // Debug.Log("SimageTileRemove");
                if (specialTiles.ContainsKey(kv.Key)) {
                    specialTiles.Remove(kv.Key);
                    //Debug.Log("SpecialTileRemove");
                }
            }
        }

        foreach (Vector2Int v in tRemove) {
            imageTileIndicator.Remove(v);
        }

        newStart = new Vector3Int(0, 0);
        newEnd = new Vector3Int(width, height);

        RedrawTexture();
        SetGridImage();

        SetGoalCoord(goalCoord);
        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            PlaceColorIcon(kv.Value, kv.Key);
        }

    }




    public void FlipVertical() {

    }

    float bufferTime = 0;


    [ContextMenu("Flip Horizontal")]
    public void FlipHorizontal () {

       // if (Time.time < bufferTime) return;
        bufferTime = Time.time + 1;
        int[,] replacement = new int[width, height];

        for (int x= 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int xcoord = (width- 1 - x);
                replacement[x, y] = colorGrid[xcoord, y];
            }
        }

        Dictionary<Vector2Int, ColorIcon> replaceIcons = new Dictionary<Vector2Int, ColorIcon>();
        Dictionary<Vector2Int, SpecialTile> relaceSpecialTiles = new Dictionary<Vector2Int, SpecialTile>();
        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            replaceIcons.Add(new Vector2Int(width - 1 - kv.Key.x, kv.Key.y), kv.Value);
            
        }

        foreach (KeyValuePair<Vector2Int, SpecialTile> kv in specialTiles) {

            SpecialTile s = new SpecialTile(kv.Value);
            if (s.isLadder()) {
                if (s.direction == BlockDirection.RIGHT) s.direction = BlockDirection.LEFT;
                else if (s.direction == BlockDirection.LEFT) s.direction = BlockDirection.RIGHT;
            }
            s.position = new Vector2Int(width - 1 - kv.Key.x, kv.Key.y);
            relaceSpecialTiles.Add(s.position, s);
        }
        imageTileIndicator = replaceIcons;
        specialTiles = relaceSpecialTiles;


        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            PlaceColorIcon(kv.Value, kv.Key);
            kv.Value.SetDirectionalIndicator(specialTiles[kv.Key].direction);
        }

        colorGrid = replacement;
        RedrawTexture();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public static Vector3Int newStart, newEnd;

    public void MinusDirection(int i) {
   
        //  if (Time.time < bufferTime) return;
        
        bufferTime = Time.time + 1;
        Vector3Int v = LadderBlock.DirectionToVector((BlockDirection)i);


        int[,] replacement = new int[width - Mathf.Abs(v.x), height - Mathf.Abs(v.y)];

        int startx = 0, starty = 0, endx = width, endy = height;

        BlockDirection b = (BlockDirection)i;

        switch (b) {
            case BlockDirection.UP:
                endy--;
                break;
            case BlockDirection.DOWN:
                starty++;
                break;
            case BlockDirection.LEFT:
                startx++;
                break;
            case BlockDirection.RIGHT:
                endx--;
                break;
        }

        newStart = new Vector3Int(startx, starty, 0);
        newEnd = new Vector3Int(endx, endy, 0);

        for (int x = startx; x < endx; x++) {
            for (int y = starty; y < endy; y++) {
                int xcoord = x;
                int ycoord = y;
                
                print("DIR" + b.ToString() +  "XCOORD " + xcoord.ToString() + " ycoord " + ycoord.ToString()  + " "  + Time.time);
                //  replacement[x - startx, y - starty] = 0;
                // colorGrid[x, y] = 0;
                if (b == BlockDirection.LEFT || b == BlockDirection.DOWN) {
                    replacement[x + (v.x), y + (v.y)] = colorGrid[x, y];
                } else {
                 //     replacement[x , y ] = 0;
                   //  colorGrid[x, y] = 0;
                    replacement[x , y] = colorGrid[x, y];
                }

            }
        }

        width = width - Math.Abs(v.x);
        height = height - Mathf.Abs(v.y);
        colorGrid = replacement;
        RedrawTexture();
        SetGridImage();
        SetGoalCoord(goalCoord);

        Dictionary<Vector2Int, ColorIcon> replaceIcons = new Dictionary<Vector2Int, ColorIcon>();
        Dictionary<Vector2Int, SpecialTile> relaceSpecialTiles = new Dictionary<Vector2Int, SpecialTile>();
        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {

            int xcoord = kv.Key.x;
            int ycoord = kv.Key.y;
            if (v.x < 0) xcoord++;
            if (v.y < 0) ycoord++;

            replaceIcons.Add(new Vector2Int(xcoord, ycoord), kv.Value);

        }

        foreach (KeyValuePair<Vector2Int, SpecialTile> kv in specialTiles) {

            int xcoord = kv.Key.x;
            int ycoord = kv.Key.y;
            if (v.x < 0) xcoord++;
            if (v.y < 0) ycoord++;

            SpecialTile s = new SpecialTile(kv.Value);
            s.position = new Vector2Int(xcoord, ycoord);
            relaceSpecialTiles.Add(s.position, s);
        }
        imageTileIndicator = replaceIcons;
        specialTiles = relaceSpecialTiles;


        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            PlaceColorIcon(kv.Value, kv.Key);
        }

        UpdateSizeInputs();


        EventSystem.current.SetSelectedGameObject(null);

    }

    public void AddDirection(int i) {
        bool minus = false;
      //  if (Time.time < bufferTime) return;
      if (i > 3) {
            i -= 4;
            minus = true;
        }
        bufferTime = Time.time + 1;
        Vector3Int v = LadderBlock.DirectionToVector((BlockDirection)i);
        
        
        int[,] replacement = new int[width + Mathf.Abs(v.x), height + Mathf.Abs(v.y)];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int xcoord = x;
                int ycoord = y;
                if (v.x < 0) xcoord++;
                if (v.y < 0) ycoord++;
                replacement[xcoord, ycoord] = colorGrid[x,y];
            }
        }

        newStart = new Vector3Int();
        newEnd = new Vector3Int();

        width = width + Math.Abs(v.x);
        height = height +  Mathf.Abs(v.y);
        colorGrid = replacement;
        RedrawTexture();
        SetGridImage();
        SetGoalCoord(goalCoord);

        Dictionary<Vector2Int, ColorIcon> replaceIcons = new Dictionary<Vector2Int, ColorIcon>();
        Dictionary<Vector2Int, SpecialTile> relaceSpecialTiles = new Dictionary<Vector2Int, SpecialTile>();
        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {

            int xcoord = kv.Key.x;
            int ycoord = kv.Key.y;
            if (v.x < 0) xcoord++;
            if (v.y < 0) ycoord++;

            replaceIcons.Add(new Vector2Int(xcoord, ycoord), kv.Value);

        }

        foreach (KeyValuePair<Vector2Int, SpecialTile> kv in specialTiles) {

            int xcoord = kv.Key.x;
            int ycoord = kv.Key.y;
            if (v.x < 0) xcoord++;
            if (v.y < 0) ycoord++;

            SpecialTile s = new SpecialTile(kv.Value);
            s.position = new Vector2Int(xcoord, ycoord);
            relaceSpecialTiles.Add(s.position, s);
        }
        imageTileIndicator = replaceIcons;
        specialTiles = relaceSpecialTiles;


        foreach (KeyValuePair<Vector2Int, ColorIcon> kv in imageTileIndicator) {
            PlaceColorIcon(kv.Value, kv.Key);
        }

        UpdateSizeInputs();
       

        EventSystem.current.SetSelectedGameObject(null);


    }

    public void UpdateSizeInputs() {
        sizeX.SetTextWithoutNotify(width.ToString());
        sizeY.SetTextWithoutNotify(height.ToString());
    }

    bool doingWorkAroundAlready;
    public IEnumerator ColorIndexWorkAround(ColorButton v) {
        if (doingWorkAroundAlready) yield break;
        doingWorkAroundAlready = true;
        colorIndexSelector.gameObject.SetActive(false);
        yield return null;
        yield return null;
        colorIndexSelector.transform.position = v.transform.position;
        colorIndexSelector.gameObject.SetActive(true);
        doingWorkAroundAlready = false;
    }

    public void SelectIndex(int i, bool delayByOneFrame) {
       
        foreach (ColorButton b in colorButtons) {
            b.DeSelect();
        }
        if (i < colorButtons.Count) {
            colorIndex = i;
            colorButtons[i].Select();
            if (!delayByOneFrame)colorIndexSelector.transform.position = colorButtons[i].transform.position;
            else {
                StartCoroutine(ColorIndexWorkAround(colorButtons[i]));
            }
            Color c = colorButtons[i].color;
            foreach (Image im in colorIconImages) {
                im.color = new Color(c.r, c.g, c.b, 1f);
            }
            picker.CurrentColor = colorButtons[i].color;
        }
        if (colors.Count > colorIndex)CursorObject.UpdateColorIfNeeded(colors[colorIndex]);
    }

    float tileWidth, tileheight;
    public void RedrawTexture() {


        Texture2D tex = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++) {
                Color c = new Color(1f,1f,1f,0f);
                if (colorGrid[x,y] > 0 && (colorGrid[x,y] - 1) < colors.Count) {
                   // print("case");
                    c = colors[colorGrid[x, y] - 1];
                }
                tex.SetPixel(x, y, c);
            }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        Sprite s = Sprite.Create(tex, new Rect(new Vector2(), new Vector2(width, height)), new Vector2());
        image.sprite = s;

        if (width == height) {
            tileWidth = defaultImageWidth / ((float)width);
            tileheight = defaultImageHeight / ((float)height);
            image.rectTransform.sizeDelta = new Vector2(defaultImageWidth, defaultImageHeight);
        } else if (width > height) {
            tileWidth = defaultImageWidth / ((float)width);
            tileheight = defaultImageWidth / ((float)width);
            image.rectTransform.sizeDelta = new Vector2(defaultImageWidth, defaultImageHeight * ((float)height / (float)width));
        } else {
            tileWidth = defaultImageWidth / ((float)height);
            tileheight = defaultImageWidth / ((float)height);
            image.rectTransform.sizeDelta = new Vector2(defaultImageWidth * ((float)width / (float)height), defaultImageHeight);
        }
        pixelCursor.sizeDelta = new Vector2(tileWidth, tileheight);
        goalMarker.sizeDelta = new Vector2(tileWidth, tileheight); ;
        cursorImage.rectTransform.sizeDelta = new Vector2(tileWidth, tileheight);
        
    }

    private int x, y = 0;
    bool isvalidCoord = false;

    public void GetCursorSpot() {
        x = (int)(pixelCursor.anchoredPosition.x / pixelCursor.rect.width);
        y = (int)(pixelCursor.anchoredPosition.y / pixelCursor.rect.height);
        if (pixelCursor.anchoredPosition.x < 0) x = -1;
        if (pixelCursor.anchoredPosition.y < 0) y = -1;
        if (x < 0 || y < 0 || x >= width || y >= height) isvalidCoord = false;
        else isvalidCoord = true;
    }

    internal void EraseAtCursor() {

        GetCursorSpot();
        if (!isvalidCoord) return;
        if (colorGrid[x, y] != 0) {
            colorGrid[x, y] = 0;

            Vector2Int coord = new Vector2Int(x, y);
            if (specialTiles.ContainsKey(coord)) {
                specialTiles.Remove(coord);
                if (imageTileIndicator.ContainsKey(coord)) {
                    Destroy(imageTileIndicator[coord].gameObject);
                    imageTileIndicator.Remove(coord);
                }
            }

            RedrawTexture();
            ErrorCheck();
        }
        
    }

    public void SelectPullOutButton(bool isSingle) {
        machine.ChangeState(new PlacePullOutButtonState(isSingle));
    }


    public void SelectCannon() {
        machine.ChangeState(new PlaceCannonState());
    }

    public void SelectLadderButton() {
        machine.ChangeState(new PlaceLadderState());
    }


    public void SelectEraserButton() {
        machine.ChangeState(new EraseState());
    }

    public void SelectBucket() {
        machine.ChangeState(new BucketState());
    }

    public void SelectPencil() {
        machine.ChangeState(new PencilState());
    }

    public void SelectGoal() {
        machine.ChangeState(new PlaceGoalState());
    }

    public void SelectCross() {
        machine.ChangeState(new PlaceBlockerState());
    }

    public void SelectTimer() {
        machine.ChangeState(new PlaceTimerState());
    }

    public void SelectUnion() {
        machine.ChangeState(new PlaceUnionState());
    }

    public void SelectOpposite() {
        machine.ChangeState(new PlaceOppositeBlockState());
    }

    public void SelectCloud() {
        machine.ChangeState(new PlaceCloudState());
    }


    internal void PlaceGoalAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (specialTiles.ContainsKey(new Vector2Int(x, y))) return;
        SetGoalCoord(new Vector2Int(x, y));
    }

    public void Save() {
        BlockSaveFile b = GetSaveFile();
        SaveManager.SaveCustomLevel(b);
    }


    public void LoadLevelButton() {
        DoNothing();
        GameMasterManager.instance.SelectLevelForEditor();


    }

    public static BlockSaveFile exportFile;

    public void ExportJson() {

   
        exportFile = GetSaveFile();
        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ExportJsonEnum());
    }

    public void ExportPNG() {
        exportFile = GetSaveFile();
        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("PNG", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ExportImageEnum());
    }

    public void Load(BlockSaveFile b) {
      
        if (b == null) return;
        b.EnsureDepthGridIsNotNull();
        HideDepthGrid();
        currentOppositeInfo = b.oppositeInfo;
        oppositeSetter.Set(currentOppositeInfo);
        playTestSaveFile = b;
        cutsceneid.text = b.cutsceneid;
        CreateImage(b.levelType, b.is3DLevel);
        SetGoalCoord(goalCoord);
    }


    public void ShowDepthGrid() {
        depthGrid.Show();
    }

    public void HideDepthGrid() {
        depthGrid.Hide();
    }


    public void Load(string contents) {
        //picker.gameObject.SetActive(false);
        try {
            BlockSaveFile b = JsonTool.StringToObject<BlockSaveFile>(contents);
            Load(b);
        } catch (JsonReaderException j) {
            Debug.Log("Loaded invalid file: \n" + contents);
        } /* catch (Exception e) {
            Debug.Log("Other Exception " + e.Message);
        } */
        
    }

    public void AddPulloutButton(Vector2Int coord, int cIndex, BlockDirection direction, int zerofullonesingle) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.PULLOUTBUTTON;
        tile.position = coord;
        tile.colorIndex = cIndex;
        tile.direction = direction;
        tile.subid = zerofullonesingle;
        specialTiles.Add(coord, tile);

        ColorIcon selectedImage = pullOutFullImage;
        if (zerofullonesingle == 1) selectedImage = pullOutSingleImage;

        ColorIcon i = Instantiate(selectedImage, drawImage);
       
        PlaceColorIcon(i, coord);
        i.SetRotation(tile.direction);
       // RectTransform rectTransform = i.GetComponent<RectTransform>();
       // rectTransform.sizeDelta = new Vector2(tileWidth, tileheight);
       // rectTransform.anchoredPosition = new Vector2(coord.x * tileWidth, coord.y * tileheight);
       // print(rectTransform.anchoredPosition);
        imageTileIndicator.Add(coord, i);
        i.image.color = colors[cIndex];
    }

    public void PlaceColorIcon(ColorIcon i, Vector2Int coord) {
        i.Awake();
        i.SetSize(coord, new Vector2(tileWidth, tileheight));
    }


    public void AddBlocker(Vector2Int coord, int cIndex) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.BLOCKERID;
        tile.position = coord;
        tile.colorIndex = cIndex;
        
        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(blockerImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[cIndex]);
        i.SetRotation(tile.direction);
    }

    public void AddSubGoal(Vector2Int coord) {
        //dfrDebug.Break();
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }
         
        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.SUBGOALID;
        tile.position = coord;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(subGoalImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetCoreColor(coreCenterImages[0].color, coreBorderImages[0].color);
        //i.SetRotation(tile.direction);
    }


    public void AddCoin(Vector2Int coord, int cIndex) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.COINID;
        tile.position = coord;
        tile.colorIndex = cIndex;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(coinImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        //i.SetColor(colors[cIndex]);
        //i.SetRotation(tile.direction);
    }





    public void AddTimer(Vector2Int coord, int cIndex) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.TIMERID;
        tile.position = coord;
        tile.colorIndex = cIndex;
        tile.direction = BlockDirection.DOWN;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(timerImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[cIndex]);
    }
    public void AddUnion(Vector2Int coord, int cIndex) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.UNIONID;
        tile.position = coord;
        tile.colorIndex = cIndex;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(unionImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[cIndex]);
        i.SetRotation(tile.direction);
    }

    public void AddCannon(Vector2Int coord, int cIndex, BlockDirection dir) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.CANNONID;
        tile.position = coord;
        tile.colorIndex = cIndex;
        tile.direction = dir;
        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(cannonImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[cIndex]);
        i.SetRotation(tile.direction);

    }

    public void AddLadder(Vector2Int coord, int cIndex, BlockDirection dir) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.LADDERID;
        tile.position = coord;
        tile.colorIndex = cIndex;
        tile.direction = dir;
        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(ladderImage, drawImage);
        PlaceColorIcon(i, coord);
        // RectTransform rectTransform = i.GetComponent<RectTransform>();
        //  rectTransform.sizeDelta = new Vector2(tileWidth, tileheight);
        // rectTransform.anchoredPosition = new Vector2(coord.x * tileWidth, coord.y * tileheight);
        //  print(rectTransform.anchoredPosition);
        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[cIndex]);
        i.SetDirectionalIndicator(dir);

    }

    public void PlacePulloutButtonAtCursor(bool isSingle) {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x,y);
        int s = 0;
        if (isSingle) s = 1;
        AddPulloutButton(coord, colorIndex, selectedDirection, s);
    }

    public void PlaceLadderAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddLadder(coord, colorIndex, selectedDirection);

    }

    public void PlaceCannonAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddCannon(coord, colorIndex, selectedDirection);
    }

    public void PlaceBlockerAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddBlocker(coord, colorIndex);
    }

    public void PlaceSubGoalAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddSubGoal(coord);
        
    }


    public void PlaceCloudAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        
        AddCloud(coord);
       
    }


    public void PlaceOppositeAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddOpposite(coord, colorIndex, selectedDirection);
    }


    public void AddCloud(Vector2Int coord) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.CLOUDID;
        tile.position = coord;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(cloudImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
    }



    public void AddOpposite(Vector2Int coord, int colorIndex, BlockDirection d) {
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            if (imageTileIndicator.ContainsKey(coord)) {
                Destroy(imageTileIndicator[coord].gameObject);
                imageTileIndicator.Remove(coord);
            }
        }

        SpecialTile tile = new SpecialTile();
        tile.id = SpecialTile.OPPOSITEID;
        tile.position = coord;
        tile.colorIndex = colorIndex;
        tile.direction = d;

        specialTiles.Add(coord, tile);

        ColorIcon i = Instantiate(oppositeImage, drawImage);
        PlaceColorIcon(i, coord);

        imageTileIndicator.Add(coord, i);
        i.SetColor(colors[colorIndex]);
        i.SetRotation(tile.direction);

        if (tile.isPositiveOpposite()) {
            i.image.sprite = positiveSprite;
        } else {
            i.image.sprite = negativeSprite;
        }

    }

    public void PlaceUnionAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddUnion(coord, colorIndex);
    }

    public void PlaceTimerAtCursor() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        if (goalCoord == new Vector2Int(x, y)) return;
        if (colorGrid[x, y] <= 0) return;
        Vector2Int coord = new Vector2Int(x, y);
        AddTimer(coord, colorIndex);
    }



    public static bool inExplorerWindow = false;

    public void ImportJsonFile() {

      

        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        FileBrowser.SetDefaultFilter(".json");
       // FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ImportJsonEnum());
    }


    public IEnumerator ExportJsonEnum() {
        inExplorerWindow = true;


        EnsureCursorLast();
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, true, null, exportFile.levelName, "Save custom level JSON file", "Save");
        
       // Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
             //   Debug.Log(FileBrowser.Result[i]);
            SaveManager.SaveFileAtPath(exportFile, FileBrowser.Result[0]);
            ReloadSaveFiles(FileBrowser.Result[0]);
        }
        inExplorerWindow = false;
    }

    public IEnumerator ExportImageEnum() {
        inExplorerWindow = true;


        EnsureCursorLast();
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, true, null, exportFile.levelName, "Save custom level PNG file", "Save");

      //  Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
               // Debug.Log(FileBrowser.Result[i]);
            CreateImageAtPath(exportFile, FileBrowser.Result[0]);
            //SaveManager.SaveFileAtPath(exportFile, FileBrowser.Result[0]);
        }
        inExplorerWindow = false;
    }

    private void CreateImageAtPath(BlockSaveFile exportFile, string path) {
        int width = exportFile.width;
        int height = exportFile.height;

        List<Color> colors = ConvertVectorListToColor(exportFile.colors);




        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (exportFile.colorGrid[x, y] > 0) {
                    tex.SetPixel(x, y, colors[exportFile.colorGrid[x, y] - 1]);
                } else {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
        
        
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        

        // For testing purposes, also write to a file in the project folder
         File.WriteAllBytes(path, bytes);
    }



    public IEnumerator ImportJsonEnum() {

      
        DoNothing();
        inExplorerWindow = true;
        EnsureCursorLast();
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
       // Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
           // Debug.Log(FileBrowser.Result[i]);
            ReadText(FileBrowser.Result[0]);
        }
        inExplorerWindow = false;
        ReturnFromDoNothingState();
    }

    public void ImportImageFile() {


        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("PNG", ".png"), new FileBrowser.Filter("JPG", ".jpg"));
        FileBrowser.SetDefaultFilter(".png");
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ImportImageEnum());
    }


    //public SimpleFileBrowser simple;

    public void AddQuickLinks() {
#if !UNITY_SWITCH
        FileBrowser.AddQuickLink("Custom Levels", SaveManager.GetCustomLevelFolderPath(), null);
#endif
    }

    public static void EnsureCursorLast() {
        CursorObject.instance.transform.SetAsLastSibling();
    }

    public IEnumerator ImportImageEnum() {
        DoNothing();
        inExplorerWindow = true;
        EnsureCursorLast();
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        //Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++) {
                //    Debug.Log(FileBrowser.Result[i]);
            }



            Texture2D tex = null;
            byte[] fileData;

            fileData = File.ReadAllBytes(FileBrowser.Result[0]);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            //print(tex.width + " " + tex.height);

            List<Color> color = new List<Color>();
            Dictionary<Color, int> cToIndex = new Dictionary<Color, int>();


            BlockSaveFile s = new BlockSaveFile();
            s.levelType = levelType;
            s.width = tex.width;
            s.height = tex.height;
            s.levelName = "Image";
            s.colorGrid = new int[tex.width, tex.height];
            int idx = 1;
            for (int x = 0; x < tex.width; x++) {
                for (int y = 0; y < tex.height; y++) {
                    Color c = tex.GetPixel(x, y);
                    if (c.a < 1) {
                        s.colorGrid[x, y] = 0;
                    } else {
                        if (cToIndex.ContainsKey(c)) {

                        } else {
                            cToIndex.Add(c, idx);
                            color.Add(c);
                            idx++;
                        }
                        s.colorGrid[x,y] = cToIndex[c];
                    }
                }
            }

            s.colors = ConvertColorListToVector(color);
           // print(JsonTool.ObjectToString<BlockSaveFile>(s));
            Load(JsonTool.ObjectToString<BlockSaveFile>(s));


        }
        inExplorerWindow = false;
        ReturnFromDoNothingState();
    }

    public void LoadBlockSaveFile(BlockSaveFile s) {
        Load(JsonTool.ObjectToString<BlockSaveFile>(s));
    }


    public void Crop() {

        int xmin = int.MaxValue;
        int xmax = -1;
        int ymin = int.MaxValue;
        int ymax = -1;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                if (colorGrid[x,y] != 0) {
                    if (x < xmin) xmin = x;
                    if (x > xmax) xmax = x;
                    if (y < ymin) ymin = y;
                    if (y > ymax) ymax = y;
                }

            }
        }

        int xMove = 0;
        int yMove = 0;

        if (xmin > 0) {
            xMove = -xmin;
        }

        if (ymin > 0) {
            yMove = -ymin;
        }

        


        if (xmin == -1) return;

        int newWidth = xmax - xmin + 1;
        int newHeight = ymax - ymin + 1;

        int xindex = 0;
        int yindex = 0;

        int[,] newGrid = new int[newWidth, newHeight];
        for (int x = xmin; x <= xmax; x++) {
            yindex = 0;
            for (int y = ymin; y <= ymax; y++) {
                newGrid[xindex, yindex] = colorGrid[x, y];
                yindex++;
            }
            xindex++;
        }

        colorGrid = newGrid;
        width = newWidth;
        height = newHeight;
        sizeX.SetTextWithoutNotify(newWidth.ToString());
        sizeY.SetTextWithoutNotify(newHeight.ToString());

       

        goalCoord -= new Vector2Int(xmin, ymin);

        List<SpecialTile> slist = new List<SpecialTile>();
        List<ColorIcon> cList = new List<ColorIcon>();

        foreach (KeyValuePair<Vector2Int, ColorIcon> s2 in imageTileIndicator) {
            cList.Add(s2.Value);
        }

        foreach (KeyValuePair<Vector2Int, SpecialTile> s2 in specialTiles) {
            slist.Add(s2.Value);
        }


        imageTileIndicator.Clear();
        specialTiles.Clear();

        int i = 0;
        foreach (SpecialTile sss in slist) {
            sss.position = sss.position + new Vector2Int(xMove, yMove);
            imageTileIndicator.Add(sss.position, cList[i]);
            specialTiles.Add(sss.position, sss);
            PlaceColorIcon(cList[i], sss.position);
            i++;
        }
        ReSize();
    }





    void ReadText(string path) {
        Load(File.ReadAllText(path));
    }

    public void RemoveSpecialTile() {
        GetCursorSpot();
        if (!isvalidCoord) return;
        
        Vector2Int coord = new Vector2Int(x, y);
        if (specialTiles.ContainsKey(coord)) {
            specialTiles.Remove(coord);
            Destroy(imageTileIndicator[coord].gameObject);
            imageTileIndicator.Remove(coord);
        }
    }

    

    public void SetDirection(int i) {
        if (levelType == LevelType.PULL && (i == (int)BlockDirection.FORWARD || i == (int)BlockDirection.BACKWARD)) {
            i = 0;
        }

        BlockDirection d = (BlockDirection)i;

        selectedDirection = d;
        upButton.color = defaultDirectionColor;
        downButton.color = defaultDirectionColor;
        rightButton.color = defaultDirectionColor;
        leftButton.color = defaultDirectionColor;
        forwardButton.color = defaultDirectionColor;
        backButton.color = defaultDirectionColor;


        
        pulloutFullTransform.transform.localEulerAngles = new Vector3(0, 0, DirectionToZRotation(d));
        pulloutSingleTransform.transform.localEulerAngles = new Vector3(0, 0, DirectionToZRotation(d));
        cannonTransform.transform.localEulerAngles = new Vector3(0, 0, DirectionToZRotation(d));
        if (d == BlockDirection.UP || d == BlockDirection.RIGHT) {
            oppositeImageToolIcon.sprite = positiveSprite;
        } else {
            oppositeImageToolIcon.sprite = negativeSprite;
        }

        switch (d) {
            case BlockDirection.UP:
                upButton.color = selectDirectionColor;
                break;
            case BlockDirection.DOWN:
                downButton.color = selectDirectionColor;
                break;
            case BlockDirection.LEFT:
                leftButton.color = selectDirectionColor;
                break;
            case BlockDirection.RIGHT:
                rightButton.color = selectDirectionColor;
                break;
            case BlockDirection.FORWARD:
                forwardButton.color = selectDirectionColor;
                break;
            case BlockDirection.BACKWARD:
                backButton.color = selectDirectionColor;
                break;
        }




    }

    public void HideDirectionTab() {
        directionTab.SetActive(false);
    }

    public void ShowDirectionTab() {
        directionTab.SetActive(true);
        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);
        forwardButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        SetDirection((int)selectedDirection);
        SetNormalText();
    }

    public void ShowForwardBackward() {
        forwardButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void HideLeftAndRightDirections() {
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        forwardButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }



    public bool LeftButtonPressed() {
        if (BlockLevelCreator.inExplorerWindow) return false;
        if (GameMasterManager.instance.keyboardScreen.gameObject.activeInHierarchy) return false;
        if (Gamepad.current != null && GameMasterManager.instance.inputManger.jumpDown) return true;
        if (Mouse.current != null)return Mouse.current.leftButton.wasPressedThisFrame;
        return false;
    }

    public bool LeftButtonHeld() {
        if (BlockLevelCreator.inExplorerWindow) return false;
        if (GameMasterManager.instance.keyboardScreen.gameObject.activeInHierarchy) return false;
        if (Gamepad.current != null) {

          //  Debug.Log("GAMEPAD CHECK " + Time.time);
            if (Gamepad.current.aButton.isPressed || Gamepad.current.buttonSouth.isPressed) {
              //  Debug.Log("SECOND CHECK " + Time.time);
                return true;
            }
        }
        if (Mouse.current != null) return Mouse.current.leftButton.isPressed;
        return false;
    }

    public bool RightButtonPressed() {
        if (BlockLevelCreator.inExplorerWindow) return false;
        if (GameMasterManager.instance.keyboardScreen.gameObject.activeInHierarchy) return false;
        if (Gamepad.current != null && Gamepad.current.buttonEast.isPressed) return true;
        if (Mouse.current != null) return Mouse.current.rightButton.wasPressedThisFrame;
        return false;
    }

    public bool MiddleMouseButtonPressed() {
        if (BlockLevelCreator.inExplorerWindow) return false;
        if (GameMasterManager.instance.keyboardScreen.gameObject.activeInHierarchy) return false;
        if (Gamepad.current != null && Gamepad.current.buttonWest.isPressed) return true;
        return Mouse.current.middleButton.wasPressedThisFrame;
    }

    public Vector2 GetMousePosition() {
        return CursorObject.instance.transform.position; //Mouse.current.position.ReadValue();
    }

    public void ReturnFromDoNothingState() {
        machine.ChangeState(CreatorDoNothingState.returnto);
    }

    public void DoNothing() {
        machine.ChangeState( new CreatorDoNothingState(machine.currentState));
    }

    public Color GetCurrentColor() {
        return colorButtons[colorIndex].color;
    }

    public Color GetColorIndex(int itttt) {
        return colorButtons[itttt].color;
    }

    public void PointerEnter() {
        machine.currentState.CursorEnterSpecialZone(machine);
    }

    public void PointerExit() {
        CursorObject.SetDefaultCursorSprite();
    }

    public bool RightButtonHeld() {
        if (Gamepad.current != null && (Gamepad.current.bButton.isPressed || Gamepad.current.buttonEast.isPressed)) return true;
        if (Mouse.current != null )return Mouse.current.rightButton.isPressed;
        return false;
    }

    public static float DirectionToZRotation(BlockDirection b) {


        switch (b) {
            case BlockDirection.UP:
                return 180;

            case BlockDirection.DOWN: return 0;

            case BlockDirection.LEFT: return 270;

            case BlockDirection.RIGHT: return 90;
            case BlockDirection.FORWARD:
                return 0;
            case BlockDirection.BACKWARD:
                return 0;
               
        }

        return 0;

    }


    public static string LeftClickString() {
        if (!GameMasterManager.IsCurrentlyGamePad()) return BlockLevelCreator.LMB;
        return GameMasterManager.ReplacePrompts("[Submit]");
    }

    public static string RightClickString() {
        if (!GameMasterManager.IsCurrentlyGamePad()) return BlockLevelCreator.RMB;
        return GameMasterManager.ReplacePrompts("[Cancel]");
    }

    public static string MiddleMouseClickString() {
        if (!GameMasterManager.IsCurrentlyGamePad()) return BlockLevelCreator.MMB;
        return GameMasterManager.ReplacePrompts("[MiddleMouse]");
    }

    public void SetOppoisteText() {
        upText.text = "Positive";
        downText.text = "Negative";
    }

    public void SetNormalText() {
        upText.text = "Up";
        downText.text = "Down";
    }


   public void ShowNameInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Level Name");
   }

   public void ShowSizeXInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(sizeX, "Size X");
    }

    public void ShowSizeYInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(sizeY, "Size Y");
    }

    public void SetHintInput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Hint");
    }

    public void ColorInput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Color Hex Value");

    }


}

public class CreatorDoNothingState : State<BlockLevelCreator> {


    public static State<BlockLevelCreator> returnto;

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
       // GameMasterManager.instance.fader.gameObject.SetActive(false);
    }

    public override void Exit(StateMachine<BlockLevelCreator> obj) {
       // GameMasterManager.instance.fader.gameObject.SetActive(true);
    }

    public  CreatorDoNothingState(State<BlockLevelCreator> to) {
        
        returnto = to;
    }
}

public class PencilState : State<BlockLevelCreator> {

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.middleMouseGoalBlocked = false;
        obj.target.HideDirectionTab();
        obj.target.SetToolCursor(obj.target.pencilTransform);
        obj.target.toolText.SetText("Pencil");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Draw Color\n" + BlockLevelCreator.MiddleMouseClickString() + " Place Goal\n" + BlockLevelCreator.RightClickString() + " Erase Color\n";
    }

   

    public override void Update(StateMachine<BlockLevelCreator> obj) {

        

        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition(); // Input.mousePosition;

        if (obj.target.LeftButtonHeld() /*Input.GetMouseButton(0)*/) {
            if (obj.target.depthGrid.groupingObject.gameObject.activeInHierarchy) return;
            if (Keyboard.current.leftShiftKey.isPressed) {
                obj.target.GrabColorAtCursor();
            } else {
                obj.target.DrawAtCursor();
            }
        }
        if (obj.target.RightButtonHeld()) {
            obj.target.EraseAtCursor();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetPencilSprite(obj.target.GetCurrentColor());
    }
}


public class BucketState : State<BlockLevelCreator> {

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.middleMouseGoalBlocked = true;
        obj.target.HideDirectionTab();
        obj.target.SetToolCursor(obj.target.bucketTransform);
        obj.target.toolText.SetText("Fill");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Fill Color\n" + BlockLevelCreator.MiddleMouseClickString() + " Fill All Color\n" + BlockLevelCreator.RightClickString() + " Erase Fill;\n";


    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {



        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition(); // Input.mousePosition;

        if (obj.target.LeftButtonHeld() /*Input.GetMouseButton(0)*/) {
            obj.target.FillAtCursor(obj.target.colorIndex + 1);
        } else if (obj.target.RightButtonHeld()) {
            obj.target.FillAtCursor(0);
        } else if (obj.target.MiddleMouseButtonPressed()) {
            obj.target.FillAllColorAtCursor(obj.target.colorIndex + 1);
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        obj.target.middleMouseGoalBlocked = true;
        CursorObject.SetBucketSprite(obj.target.GetCurrentColor());
    }
}



public class PlaceGoalState : State<BlockLevelCreator> {

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.HideDirectionTab();
        obj.target.SetToolCursor(obj.target.goalTransform);
        obj.target.toolText.SetText("Place Goal");
        CursorObject.SetDefaultCursorSprite();
        obj.target.toolPromptText.text = BlockLevelCreator.MiddleMouseClickString() + " Place Goal\n" + 
                                         BlockLevelCreator.LeftClickString() + " Place Goal\n" + 
                                         BlockLevelCreator.RightClickString() + " Place Sub Goal";
    }
    public override void Update(StateMachine<BlockLevelCreator> obj) {

        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceGoalAtCursor();
        }
        if (obj.target.RightButtonPressed()) {
            obj.target.PlaceSubGoalAtCursor();
        }
    }


}

public class EraseState : State<BlockLevelCreator> {

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.HideDirectionTab();
        obj.target.SetToolCursor(obj.target.eraserTransform);
        obj.target.toolText.SetText("Eraser");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Erase\n";
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {


        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition(); // Input.mousePosition;

        if (obj.target.LeftButtonHeld() /*Input.GetMouseButton(0)*/) {
            obj.target.EraseAtCursor();
        }
        if (obj.target.RightButtonPressed()) {
           
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetEraseSprite();
    }



}


public class PlacePullOutButtonState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {

        if (BlockLevelCreator.levelType == LevelType.PULL) {
            obj.target.HideLeftAndRightDirections();
            if (obj.target.selectedDirection == BlockDirection.LEFT || obj.target.selectedDirection == BlockDirection.RIGHT) {
                obj.target.SetDirection((int)BlockDirection.DOWN);
            }
        }

        obj.target.ShowDirectionTab();
        if (BlockLevelCreator.levelType == LevelType.PULL) {
            obj.target.HideLeftAndRightDirections();
        }
            if (single) {
            obj.target.SetToolCursor(obj.target.pulloutSingleTransform);
        } else {
            obj.target.SetToolCursor(obj.target.pulloutFullTransform);
        }
       
        obj.target.toolText.SetText("Pull Out Block");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
    }

    public bool single;

    public PlacePullOutButtonState(bool single) {
        this.single = single;
    }


    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlacePulloutButtonAtCursor(single);
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetPulloutSprite(obj.target.GetCurrentColor(), single);
    }
}

public class PlaceTimerState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.toolText.SetText("Timer");
        obj.target.SetToolCursor(obj.target.timerTransform);
        obj.target.HideDirectionTab();
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetTimer(obj.target.GetCurrentColor());
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceTimerAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }


}

public class PlaceUnionState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.toolText.SetText("Union");
        obj.target.SetToolCursor(obj.target.unionTransform);
        obj.target.HideDirectionTab();
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceUnionAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetUnion(obj.target.GetCurrentColor());
    }
}
public class PlaceBlockerState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.toolText.SetText("Blocker");
        obj.target.SetToolCursor(obj.target.blockerTransform);
        obj.target.HideDirectionTab();
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceBlockerAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetBlocker(obj.target.GetCurrentColor());
    }
}
public class PlaceCannonState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.SetDirection((int)obj.target.selectedDirection);
        
        obj.target.ShowDirectionTab();
        obj.target.toolText.SetText("Cannon");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
        obj.target.SetToolCursor(obj.target.cannonTransform);
    }

    


    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceCannonAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetCannonSprite(obj.target.GetCurrentColor());
    }
}

public class PlaceCloudState : State<BlockLevelCreator> {


    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.HideDirectionTab();
        obj.target.toolText.SetText("Cloud");
        obj.target.SetToolCursor(obj.target.cloudTransform);
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";

    }


    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceCloudAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetCloudSprite();
    }

    

    public override void Exit(StateMachine<BlockLevelCreator> obj) {
        base.Exit(obj);
    }


}

public class PlaceOppositeBlockState : State<BlockLevelCreator> {
    public override void Enter(StateMachine<BlockLevelCreator> obj) {

        if (obj.target.selectedDirection == BlockDirection.LEFT) {
            obj.target.selectedDirection = BlockDirection.UP;
        }

        if (obj.target.selectedDirection == BlockDirection.RIGHT) {
            obj.target.selectedDirection = BlockDirection.DOWN;
        }

        obj.target.SetDirection((int)obj.target.selectedDirection);
        obj.target.ShowDirectionTab();
        obj.target.HideLeftAndRightDirections();
        obj.target.toolText.SetText("Opposite");
        obj.target.SetToolCursor(obj.target.oppositeTransform);
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
        obj.target.SetOppoisteText();
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceOppositeAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        if (obj.target.selectedDirection == BlockDirection.UP || obj.target.selectedDirection == BlockDirection.RIGHT) {
            CursorObject.SetOpposite(obj.target.positiveSprite, obj.target.GetCurrentColor());
        } else {
            CursorObject.SetOpposite(obj.target.negativeSprite, obj.target.GetCurrentColor());
        }
    }

    public override void Exit(StateMachine<BlockLevelCreator> obj) {
        obj.target.SetNormalText();
    }


}

public class PlaceLadderState : State<BlockLevelCreator> {

    public override void Enter(StateMachine<BlockLevelCreator> obj) {
        obj.target.ShowDirectionTab();
        obj.target.ShowForwardBackward();
        obj.target.SetToolCursor(obj.target.ladderTransform);
        obj.target.toolText.SetText("Ladder Block");
        obj.target.toolPromptText.text = BlockLevelCreator.LeftClickString() + " Place\n" + BlockLevelCreator.RightClickString() + " Erase\n";
    }

    public override void Update(StateMachine<BlockLevelCreator> obj) {
        obj.target.pixelCursor.transform.position = obj.target.GetMousePosition();
        if (obj.target.LeftButtonPressed()) {
            obj.target.PlaceLadderAtCursor();
        } else if (obj.target.RightButtonPressed()) {
            obj.target.RemoveSpecialTile();
        }
    }

    public override void CursorEnterSpecialZone(StateMachine<BlockLevelCreator> obj) {
        CursorObject.SetLadderSprite(obj.target.GetCurrentColor(), obj.target.selectedDirection);
    }
}
