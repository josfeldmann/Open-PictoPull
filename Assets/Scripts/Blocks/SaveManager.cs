using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using System.Linq;

using UnityEngine.ProBuilder.MeshOperations;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif


[System.Serializable]
public class SingleSaveFile {
   public Dictionary<string,BlockSaveFile> saveFiles;
}



public static class SaveManager {
    internal static void SaveCustomLevel(BlockSaveFile b) {


        if (!GameMasterManager.instance.isSingleFileSaveMode) {
            string path = GetCustomLevelPath(b.levelName);
            SaveFileAtPath(b, path);
            BlockLevelCreator.ReloadSaveFiles(path);
        } else {
            loadedLevelInfos[b.GetSaveKey()] = new LevelInfo(b);
            CreateSingleLevelSaveFileFromLoadedLevelInfos();
        }
    }

    public static Dictionary<string, LevelInfo> loadedLevelInfos = new Dictionary<string, LevelInfo>();

    
    #if UNITY_SWITCH
    private static readonly SwitchSaveManager SwitchSaveManager = new SwitchSaveManager();
    #endif

    public static void SaveFileAtPath(BlockSaveFile b, string path) {
        string data = JsonTool.ObjectToString<BlockSaveFile>(b);
        File.WriteAllText(path, data);
    }

    public static string GetScreenShotPath()
    {
        string folder = GetSaveLocationPath() + "/Screenshots";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        return folder;
    }

    public static string DocumentsPath() {
        string path = "";

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        } else {
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        } // if

        return path;
    }

    public static string GetSaveLocationPath() {
        string docpath = Directory.GetParent(Application.dataPath).FullName;
        //Debug.Log(docpath);
        
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor) {
                return Application.dataPath;
            } else {
            if (!Directory.Exists(docpath + "/SAVES")) Directory.CreateDirectory(docpath + "/SAVES");

            return docpath + "/SAVES";
            // return Application.dataPath + "/Saves";
                //return Application.persistentDataPath
        }
    }

    public static string GetCustomLevelFolderPath() {
        string path = GetSaveLocationPath() + "/CustomLevels";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }


    public static string GetCustomLevelPackPath() {
        string path = GetSaveLocationPath() + "/CustomLevelPacks";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    public static string GetCustomLevelPackPath(string s) {
        string path = GetSaveLocationPath() + "/CustomLevelPacks/" + s;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    public static void SaveCustomLevelPack(LevelPack s, List<LevelInfo> levelInfos, Sprite sprite) {
        string folderPath = SaveManager.GetCustomLevelPackPath(s.packName);
        string levelPackInfoLocation = folderPath + "/" + packInfoString;
        string imagePackLocation = folderPath + "/" + packImageString;

        File.WriteAllText(levelPackInfoLocation, JsonTool.ObjectToString(s));
        
        foreach (LevelInfo i in levelInfos) {
            string file = folderPath + "/" + i.saveFile.levelName + ".json";
            File.WriteAllText(file, JsonTool.ObjectToString(i.saveFile));
        }

        SaveSpriteAtPath( sprite, imagePackLocation);

    }

    public static void DeleteLevelPack(string pathToDelete) {
        if (Directory.Exists(pathToDelete)) {
           // Debug.Log(pathToDelete);
            Directory.Delete(pathToDelete, true);
        }
    }
    
    public static void SaveSpriteAtPath(Sprite s, string path) {
        
            int width = (int)s.rect.width;
            int height = (int) s.rect.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tex.SetPixel(x, y, s.texture.GetPixel(x, y));
                }
            }
            tex.Apply();
            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(path, bytes);
    }

    

    public static void LoadCustomLevels() {
//#if !UNITY_SWITCH
        if ( Application.platform  != RuntimePlatform.Switch && !GameMasterManager.instance.isSingleFileSaveMode) {
            string cusPath = GetCustomLevelFolderPath();
            // string[] paths  = Directory.GetFiles(cusPath, "*.json");
            string[] paths = Directory.GetDirectories(cusPath);
            foreach (string s in paths) {
                string folderName = Path.GetFileName(s);
                // Debug.Log(folderName);
                string[] jsons = Directory.GetFiles(s, "*.json");
                foreach (string jsonfile in jsons) {

                    string ssssss = jsonfile.Replace('\\', '/');

                    if (!loadedLevelInfos.ContainsKey(ssssss)) {
                        //string level = s + "/" + folderName + ".json";
                        if (File.Exists(ssssss)) {
                            BlockSaveFile sav = JsonTool.StringToObject<BlockSaveFile>(File.ReadAllText(ssssss));
                            LevelInfo l = new LevelInfo(sav);

                            loadedLevelInfos.Add(ssssss, l);
                        } else {
                            Debug.Log(ssssss);
                        }
                    }
                }
            }
        } else {

            SingleSaveFile s = LoadSingleSaveFile();
            if (s != null && s.saveFiles != null) {
                foreach (KeyValuePair<string, BlockSaveFile> kv in s.saveFiles) {
                    loadedLevelInfos[kv.Key] = new LevelInfo(kv.Value);
                }
            }



        }
//#endif
    }

    



    public static List<LevelInfo> LoadAllLevelsAtPath(string s) {
        List<LevelInfo> levels = new List<LevelInfo>();

        string[] jsons = Directory.GetFiles(s, "*.json");
        foreach (string jsonfile in jsons) {
            if (!loadedLevelInfos.ContainsKey(jsonfile)) {
                //string level = s + "/" + folderName + ".json";
                if (File.Exists(jsonfile)) {
                    BlockSaveFile sav = JsonTool.StringToObject<BlockSaveFile>(File.ReadAllText(jsonfile));
                    LevelInfo l = new LevelInfo(sav);
                    levels.Add(l);
                } else {
                    Debug.Log(jsonfile);
                }
            }
        }


        return levels;
    }


    public const string packInfoString = "PackInfo.txt";
    public const string packImageString = "PackImage.png";
    public const string isSteamFile = "steamBool.txt";
    public const string progressPackFile = "packProgress.txt";

    public static List<LevelPack> LoadCustomLevelPacks() {
        List<LevelPack> packs = new List<LevelPack>();
        
#if !UNITY_SWITCH
        string cusPath = GetCustomLevelPackPath();
        //Debug.Log(cusPath);
        string[] paths = Directory.GetDirectories(cusPath);

        foreach (string s in paths) {
            string packInfo = s + "/" + packInfoString;
            string packImage = s + "/" + packImageString;
            if (File.Exists(packInfo)) {
                LevelPack pack = JsonTool.StringToObject<LevelPack>(File.ReadAllText(packInfo));
                if (File.Exists(packImage)) {
                    pack.sprite = LoadNewSprite(packImage);
                } else {
                    pack.sprite = GameMasterManager.instance.levelPackManager.defaultPackSprite;
                }
                pack.levelPath = s;
                packs.Add(pack);
            }
        }
#endif

        return packs;
    }

    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 16.0f) {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;
        Texture2D SpriteTexture = LoadTexture(FilePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        

        return NewSprite;
    }

    public static Texture2D LoadTexture(string FilePath) {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath)) {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData)) {          // Load the imagedata into the texture (size is set automatically)
                Tex2D.filterMode = FilterMode.Point;
                return Tex2D;
            }
        }
        return null;                     // Return null if load failed
    }



    public static string SerializeLevelFile(BlockSaveFile b) {
        return JsonTool.ObjectToString<BlockSaveFile>(b);
    }

    public static string GetCustomLevelPath(string name) {

        if (!Directory.Exists(GetCustomLevelFolderPath() + "/" + name)) Directory.CreateDirectory(GetCustomLevelFolderPath() + "/" + name);

        return GetCustomLevelFolderPath() + "/" + name + "/" + name + ".json";
    }

    internal static BlockSaveFile GetSaveFile(string text) {
        if (File.Exists(GetCustomLevelPath(text))) {
            return JsonTool.StringToObject<BlockSaveFile>(File.ReadAllText(GetCustomLevelPath(text)));
        }
        return null;
    }

    public static string GetGameSaveFilePath() {
        return GetSaveLocationPath() + "/saveFile.json";
    }

    internal static void SaveGameFile(GameSaveFile gameSaveFile) {
        GameMasterManager.gameSaveFile = gameSaveFile;
        gameSaveFile.trackerSaveFile ??= new AchievementTrackerSaveFile();
        GameMasterManager.instance.tracker.SaveTime();
        
        #if !UNITY_SWITCH || UNITY_EDITOR
        string saveLocation = GetGameSaveFilePath();
        File.WriteAllText(saveLocation, JsonTool.ObjectToString<GameSaveFile>(gameSaveFile));
        #else
        SwitchSaveManager.Save(gameSaveFile);
        #endif
    }

   static bool installedStuff;
   static uint folderSize;
    static string path;

    public static string steamworkshopfolder;

    internal static void EnsureSteamWorkshopItemsAreDownloaded() {
        #if !DISABLESTEAMWORKS
        if (SteamManager.Initialized) {
            //Debug.Log("Number of subscribed items: " + SteamUGC.GetNumSubscribedItems());
            var nItems = SteamUGC.GetNumSubscribedItems();
            if (nItems > 0) {
                PublishedFileId_t[] PublishedFileID = new PublishedFileId_t[nItems];
                uint ret = SteamUGC.GetSubscribedItems(PublishedFileID, nItems);

               

                //We want to iterate through all the file IDs in order to get their install info (folder out: Returns the absolute path to the folder containing the content by copying it) HOWEVER it only has the path if k_EItemStateInstalled is set
                foreach (PublishedFileId_t i in PublishedFileID) {
                    //SteamAPICall_t handle = SteamUGC.RequestUGCDetails(i, 5);
                    installedStuff = SteamUGC.GetItemInstallInfo(i, out ulong SizeOnDisk, out string Folder, 1024, out uint punTimeStamp); //Must name the outs exactly the same as in docs, it returned null with folder, but making it Folder works
                                                                                                                                           //print(Folder);
                                                                                                                                           //string[] path = Directory.GetFiles(Folder);
                                                                                                                                           // string filename = Path.GetFileName(path[0]);
                                                                                                                                         // string fullPath = path[0];
                                                                                                                                           //string destPath = Application.persistentDataPath + "/" + filename;
                                                                                                                                //print(destPath);
                                                                                                                                           // System.IO.File.Copy(fullPath, destPath, true);
                    steamworkshopfolder = Path.GetDirectoryName(Folder);
                    Debug.Log("Subscribed item: " + i.m_PublishedFileId + " " + Time.time.ToString());
                    Debug.Log("Folder " + Path.GetDirectoryName(Folder));
                    if (File.Exists(Folder + "/PackInfo.txt")) {
                        Debug.Log("PUZZLE PACK PATH");
                        string levelPackDirectory = GetCustomLevelPackPath() + "/" + i.m_PublishedFileId;
                        if (!Directory.Exists(levelPackDirectory)) {
                            Directory.CreateDirectory(levelPackDirectory);
                        }
                        
                        foreach (string s in Directory.GetFiles(Folder)) {
                            string dest = levelPackDirectory + "/" + Path.GetFileName(s);
                            if (!File.Exists(dest)) {
                                File.Copy(s, dest);
                            }
                        }

                        if(File.Exists(levelPackDirectory + "/" + packInfoString) && !File.Exists(Folder + "/" + isSteamFile )) {
                            File.Create(Folder + "/" + isSteamFile);
                        }


                    } else {
                        Debug.Log("PUZZLE " + Folder + " " + Time.time );
                        if (Directory.Exists(Folder)) {
                            
                            string[] paths = Directory.GetFiles(Folder, "*.json");
                            if (paths.Length == 0) {
                                SteamUGC.DownloadItem(new PublishedFileId_t(i.m_PublishedFileId), true);
                            } else {
                                foreach (string s in paths) {
                                    string filePath = GetCustomLevelFolderPath() + "/" + i.m_PublishedFileId + "/" + Path.GetFileName(s);
                                    Debug.Log(filePath);
                                    if (!File.Exists(filePath)) {
                                        if (!Directory.Exists(GetCustomLevelFolderPath() + "/" + i.m_PublishedFileId)) Directory.CreateDirectory(GetCustomLevelFolderPath() + "/" + i.m_PublishedFileId);
                                        File.Copy(s, filePath);
                                    }

                                    
                                }
                            }
                        } else {
                            SteamUGC.DownloadItem(new PublishedFileId_t(i.m_PublishedFileId), true);
                        }


                    }

                    

                }
            }
            path = Application.persistentDataPath;

           // foreach (string file in System.IO.Directory.GetFiles(path)) {
           //     string tempFile = (System.IO.Path.GetFileName(file)); //we only want file names, not the whole directory to the file + name
           //     if (string.Equals(tempFile, "Player.log") == false && string.Equals(tempFile, "Player-prev.log") == false) {
           //         listOfCustomLevels = listOfCustomLevels + tempFile + "\n";  //Make a string that concats all the files so that we can assign it to the scrollView
          //      }
          //  }
          
        }
        GameMasterManager.instance.levelPackManager.UpdateLevelPacks();
#endif
    }

    public static GameSaveFile CreateNewEmptyGameSaveFileWithCheckingDemo() {

        //Removing demo check for now 2024
        /*
        try {
            string getDemoSavePath = Directory.GetParent(Application.dataPath).Parent.Parent.FullName + "/PictoPull Demo/SAVES/savefile.json";

            string oldDemoSavePath = Application.persistentDataPath + "/savefile.json";

           // Debug.Log("Get current Demo " + getDemoSavePath);
           // Debug.Log("Get old Demo " + oldDemoSavePath);

            if (File.Exists(getDemoSavePath)) {
                String s = File.ReadAllText(getDemoSavePath);
                GameSaveFile gg = JsonTool.StringToObject<GameSaveFile>(s);
                return gg;
            } else if (File.Exists(oldDemoSavePath)) {
                String s = File.ReadAllText(oldDemoSavePath);
                GameSaveFile gg = JsonTool.StringToObject<GameSaveFile>(s);
                return gg;
            }

             
        } catch (Exception e) {
            Debug.LogError(e);
        }
        */

        var g = CreateNewEmptyGameSaveFile();
        return g;
    }

    public static GameSaveFile CreateNewEmptyGameSaveFile()
    {
        GameSaveFile g = new GameSaveFile();
        g.completedStoryLevels = new HashSet<string>();
        g.completedCustomLevels = new HashSet<string>();
        g.skippedLevels = new HashSet<string>();
        g.unlockedStoryChapters = new List<StoryChapters>();
        foreach (LevelGroup gr in GameMasterManager.instance.levelsStartUnlocked) {
            g.unlockedStoryChapters.Add(gr.StoryChapter);
        }

        return g;
    }
    
    internal static GameSaveFile LoadGameSaveFile()
    {

        GameSaveFile gg = null;

        #if !UNITY_SWITCH || UNITY_EDITOR
        string saveLocation = GetGameSaveFilePath();
        if (!File.Exists(saveLocation)) {
            PlayerPrefs.SetInt(GameMasterManager.LEVELSELECTPLAYERPREF, -2);
            SaveGameFile(CreateNewEmptyGameSaveFile());
        }

        string contents = File.ReadAllText(saveLocation);
        gg = JsonTool.StringToObject<GameSaveFile>(contents);
        #else
        gg = SwitchSaveManager.Load();
        #endif
        if (gg.completedStoryLevels == null) gg.completedStoryLevels = new HashSet<string>();
        if (gg.completedCustomLevels == null) gg.completedCustomLevels = new HashSet<string>();
        if (gg.completedStoryLevels == null) gg.completedStoryLevels = new HashSet<string>();
        if (gg.skippedLevels == null) gg.skippedLevels = new HashSet<string>();
        return gg;

    }
    #if !DISABLESTEAMWORKS
    public static string GetSteamFilePathInCustomFolder(PublishedFileId_t m_nPublishedFileId) {

        string folder = GetCustomLevelFolderPath() + "/" + ((ulong)m_nPublishedFileId).ToString();

        string[] paths = Directory.GetFiles(folder, "*.json");
        if (paths.Length == 0) return null;
        else return paths[0];


    }
#endif

    public static void EnsureLevelIsLoaded(string levelPath) {


        
            if (!loadedLevelInfos.ContainsKey(levelPath)) {
                //string level = s + "/" + folderName + ".json";
                if (File.Exists(levelPath)) {
                    BlockSaveFile sav = JsonTool.StringToObject<BlockSaveFile>(File.ReadAllText(levelPath));
                    LevelInfo l = new LevelInfo(sav);
                    loadedLevelInfos.Add(levelPath, l);
                } else {
                    Debug.Log(levelPath);
                }
            }
        

    }

    public static LevelInfo GetCustomLevelFromFolderPath(string levelPath) {
        return loadedLevelInfos[levelPath];
    }

    public static void DeleteCustomLevel(LevelInfo levelInfo) {
        try {
            string levelPathd = loadedLevelInfos.FirstOrDefault(x => x.Value == levelInfo).Key;
            File.Delete(levelPathd);
            loadedLevelInfos.Remove(levelPathd);
            LoadCustomLevels();
        } catch (Exception e){
            Debug.LogError(e.Message);
        }
    }


    public static SingleSaveFile LoadSingleSaveFile() {
        if (Application.platform == RuntimePlatform.Switch) {

#if UNITY_SWITCH
        return SwitchSaveManager.LoadSingleSaveFile();
#endif

        } else {
            
            if (File.Exists(GetSingleLevelFilePath())) {
                string contents = File.ReadAllText(GetSingleLevelFilePath());
                SingleSaveFile s = JsonTool.StringToObject<SingleSaveFile>(contents);
                return s;
            } else {
                SingleSaveFile s = new SingleSaveFile();
                s.saveFiles = new Dictionary<string, BlockSaveFile>();
                SaveSingleSaveGameFile(s);
                return s;
            }

        }
        return new SingleSaveFile();
    }

    public static void SaveSingleSaveGameFile(SingleSaveFile s) {
        
        if (Application.platform == RuntimePlatform.Switch) {
#if UNITY_SWITCH
        SwitchSaveManager.SaveSingleSaveFile(s);
#endif
        } else {
            string contents = JsonTool.ObjectToString(s);
            File.WriteAllText(GetSingleLevelFilePath(), contents);
        }
    }

    public static string GetSingleLevelFilePath() {
        return GetSaveLocationPath() + "/singleLevels.json";
    }

    public static void CreateSingleLevelSaveFileFromLoadedLevelInfos() {

        SingleSaveFile s = new SingleSaveFile();
        s.saveFiles = new Dictionary<string, BlockSaveFile>();
        foreach (KeyValuePair<string, LevelInfo> lv in loadedLevelInfos) {
            s.saveFiles.Add(lv.Key, lv.Value.saveFile);
        }
        SaveSingleSaveGameFile(s);

    }




}


