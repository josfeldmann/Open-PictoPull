using Newtonsoft.Json;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThreeDLevelCreator : MonoBehaviour
{

    public Vector3Int size = new Vector3Int(10, 10, 10);
    public Vector3Int defaultLevelSize = new Vector3Int(10, 10, 10);


    public ThreeDBlockMarker[,,] markerGrid;

    public List<ThreeDBlockMarker> spawnedMarkers = new List<ThreeDBlockMarker>();



    public ThreeDCameraController threeDCamera;

    StateMachine<ThreeDLevelCreator> stateMachine;


    [Header("Physical Parts")]

    public GameObject goalSpot;

    public Transform spawnMarkerHere;
    public ThreeDBlockMarker markerPrefab;
    public GameObject gridBase;
    public GameObject placementPlane;




    [Header("UI")]
    public GameObject UITransform;
    public Transform colorIndexAttachPoint;
    public ColorButton colorIndexPrefab;
    public RectTransform colorCursor;
    public List<ColorButton> spawnedColorButtons = new List<ColorButton>();
    public ColorPalette defaultColorPalette;
    public int currentColorPaletteIndex;


    public TMP_InputField nameInput, sizeX, sizeY, sizeZ;


    
    public void SetGoalSpot(int x, int y, int z) {

        SetGoalSpot(new Vector3Int(x, y, z));

    }


    public void SetGoalSpot(Vector3Int v) {

        goalSpot.transform.localPosition = v;

    }



    public void SetUI(bool b) {
        UITransform.SetActive(b);
        colorCursor.gameObject.SetActive(b);
    }

    public List<Color> currentColorList;

    public void SetColors(List<Color> colors) {

        currentColorList = colors;

        int toAdd = colors.Count -  spawnedColorButtons.Count;

        for (int i = 0; i < toAdd; i++) {
            ColorButton b = Instantiate(colorIndexPrefab, colorIndexAttachPoint);
            spawnedColorButtons.Add(b);
        }

        foreach (ColorButton b in spawnedColorButtons) {
            gameObject.gameObject.SetActive(false);
        }

        for (int i = 0; i < colors.Count; i++) {
            ColorButton b = spawnedColorButtons[i];
            b.gameObject.SetActive(true);
            b.threeDLevelCreator = this;
            b.SetColorIndex(colors[i], i);
        }


    }



    public BlockSaveFile GetBlockSaveFile() {

        BlockSaveFile b = new BlockSaveFile();

        b.levelName = nameInput.text;
        b.width = size.x;
        b.height = size.y;
        b.is3DLevel = true;
        b.threeDMap = new int[size.x, size.y, size.z];
        b.threeDGoalSpot = new Vector3Int((int)goalSpot.transform.localPosition.x, (int)goalSpot.transform.localPosition.y, (int)goalSpot.transform.localPosition.z);
        b.levelType = LevelType.CRASH;
        b.colors = BlockLevelCreator.ConvertColorListToVector(defaultColorPalette.colors);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {

                    b.threeDMap[x, y, z] = 0;
                    if (markerGrid[x,y,z] != null) {
                        b.threeDMap[x,y,z] =  markerGrid[x, y, z].colorIndex + 1;
                    }

                }
            }
        }

        return b;




    }



    public void PlayTestButton() {
        BlockSaveFile b = GetBlockSaveFile();
        Debug.Log(JsonTool.ObjectToString(b));

        GameMasterManager.instance.PlayCustomLevelTest(b);
    }


    public void SaveButton() {

        BlockSaveFile b = GetBlockSaveFile();

        SaveManager.SaveCustomLevel(b);
    }

    BlockSaveFile exportFile;

    public void ExportJsonButton() {


        exportFile = GetBlockSaveFile();
        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ExportJsonEnum());
    }



    public IEnumerator ExportJsonEnum() {
        //inExplorerWindow = true;


        BlockLevelCreator.EnsureCursorLast();
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, true, null, exportFile.levelName, "Save custom level JSON file", "Save");

        // Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                //   Debug.Log(FileBrowser.Result[i]);
                SaveManager.SaveFileAtPath(exportFile, FileBrowser.Result[0]);
            BlockLevelCreator.ReloadSaveFiles(FileBrowser.Result[0]);
        }
        //inExplorerWindow = false;
    }



    public void ImportJsonFile() {



        FileBrowser.SetLoadPath(SaveManager.GetCustomLevelFolderPath());
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        // FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ImportJsonEnum());
    }

    public IEnumerator ImportJsonEnum() {


        
        BlockLevelCreator.EnsureCursorLast();
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        // Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success) {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                // Debug.Log(FileBrowser.Result[i]);
                ReadText(FileBrowser.Result[0]);
        }
        
    }



   

    void ReadText(string path) {
        Load(File.ReadAllText(path));
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



    private void Awake() {
        
    }

    public void SetIndex(int index) {
        currentColorPaletteIndex = index;
        colorCursor.transform.position = spawnedColorButtons[index].transform.position;
    }

    public void NumpadIndex() {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            SetIndex(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            SetIndex(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            SetIndex(2);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) {
            SetIndex(3);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame) {
            SetIndex(4);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame) {
            SetIndex(5);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame) {
            SetIndex(6);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame) {
            SetIndex(7);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame) {
            SetIndex(8);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame) {
            SetIndex(9);
        }
    }

    public void Update() {

        NumpadIndex();

        if (stateMachine != null) {
            stateMachine.Update();
        }



    }



    public void RemoveAllMarkers() {
        foreach (ThreeDBlockMarker marker in spawnedMarkers) {
            Destroy(marker.gameObject);
        }
        spawnedMarkers = new List<ThreeDBlockMarker>();
    }

    public void SetDefaultLevel() {
        RemoveAllMarkers();
        gameObject.SetActive(true);
        nameInput.text = "File";
        SetColors(defaultColorPalette.colors);
        SetSize(size);
        SetIndex(0);
        SetGoalSpot(0, 0, 0);
        threeDCamera.StartLevelEditor();
        gameObject.SetActive(true);
    }

    public void Load(BlockSaveFile b) {
        RemoveAllMarkers();
        gameObject.SetActive(true);
        nameInput.text = b.levelName;
        SetColors(BlockLevelCreator.ConvertVectorListToColor(b.colors));
        SetSize(new Vector3Int(b.threeDMap.GetLength(0), b.threeDMap.GetLength(1), b.threeDMap.GetLength(2)));
        SetIndex(0);
        SetGoalSpot(b.threeDGoalSpot.x, b.threeDGoalSpot.y, b.threeDGoalSpot.z);
        threeDCamera.StartLevelEditor();

        for (int x = 0; x < b.threeDMap.GetLength(0); x++) {
            for (int y = 0; y < b.threeDMap.GetLength(1); y++) {
                for (int z = 0; z < b.threeDMap.GetLength(2); z++) {
                    int val = b.threeDMap[x, y, z];
                    if (val > 0) {
                        SpawnMarkerAtGridPosition(new Vector3Int(x, y, z), val - 1);
                    }
                }
            }
        }
        gameObject.SetActive(true);
    }






    public int GetInt(TMP_InputField input) {
        int x = 0;
        if (int.TryParse(input.text, out x)) {
            return x;
        }
        return 1;
    }

    public void SetSizeButton() {
        
        SetSize(new Vector3Int(GetInt(sizeX), GetInt(sizeY), GetInt(sizeZ)));
    }


    public void SetSize(Vector3Int v) {
        RemoveAllMarkers();
        size = v;
        gridBase.transform.localScale = new Vector3(size.x, 1, size.z);
        markerGrid = new ThreeDBlockMarker[size.x, size.y, size.z];
        threeDCamera.transform.position = transform.position + new Vector3(size.x / 2f, 5, -5);
        //

        sizeX.SetTextWithoutNotify(v.x.ToString());
        sizeY.SetTextWithoutNotify(v.y.ToString());
        sizeZ.SetTextWithoutNotify(v.z.ToString());


    }

    public void RemoveMarker(ThreeDBlockMarker marker) {
        spawnedMarkers.Remove(marker);
        markerGrid[marker.position.x, marker.position.y, marker.position.z] = null;
        Destroy(marker.gameObject);
    }

    Vector3Int lastSpot;

    internal void SpawnMarkerAtMouseClick(Vector3Int spot, Vector3 realSpot, Vector3 normal) {
        
        Vector3 calcSpot = new Vector3(spot.x, spot.y, spot.z) + normal*0.5f;
        spot = new Vector3Int((int)calcSpot.x, (int)calcSpot.y, (int)calcSpot.z);

        if (markerGrid[spot.x, spot.y, spot.z] != null) return;

        SpawnMarkerAtGridPosition(spot, currentColorPaletteIndex);
        
    }

    public void SpawnMarkerAtGridPosition(Vector3Int v, int color) {

        ThreeDBlockMarker marker = Instantiate(markerPrefab, spawnMarkerHere);
        marker.creator = this;
        marker.SetColor(color);
        marker.transform.localPosition = v;
        spawnedMarkers.Add(marker);
        markerGrid[v.x, v.y, v.z] = marker;
        lastSpot = v;


    }

    public void MainMenuButton() {
        gameObject.SetActive(false);
        GameMasterManager.instance.GoToMainMenu();
    }

    internal void MoveColor(int v) {
        currentColorPaletteIndex += v;
        if (currentColorPaletteIndex < 0) currentColorPaletteIndex = currentColorList.Count - 1;
        if (currentColorPaletteIndex >= currentColorList.Count) currentColorPaletteIndex = 0;
        SetIndex(currentColorPaletteIndex);
    }
}
