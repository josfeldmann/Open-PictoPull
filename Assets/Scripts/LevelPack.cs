using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LevelPack {


    

    public string packName;
    public string packDescription;
    public string author;
    public int numberOfLevels;
    public Vector2Int difficultyRange;
    [NonSerialized] public Sprite sprite;
    public string levelPath;
    [NonSerialized] public BuiltInLevelPack associatedLevelPack;

    public List<LevelInfo> GetSaveFiles() {
        List<LevelInfo> l = new List<LevelInfo>();

        if (associatedLevelPack != null) {
            foreach (TextAsset t in associatedLevelPack.assets) {
                LevelInfo level = new LevelInfo(t);
                l.Add(level);
            }
        }

        if (levelPath != null && levelPath.Length > 0) {
            List<LevelInfo> loadedLevels = SaveManager.LoadAllLevelsAtPath(levelPath);
            foreach (LevelInfo level in loadedLevels) {
                l.Add(level);
            }
        }
        return l;
    }


    public bool isSteam() {
        if (associatedLevelPack != null) return false;
        if (levelPath == null) return false;
        if (File.Exists(levelPath + "/" + SaveManager.isSteamFile)) {
            return true;
        }
        return false;
    }

    public void CompleteLevel(int currentLevelPackIndex) {
        if (associatedLevelPack == null) {

            string progressFile = levelPath + "/" + SaveManager.progressPackFile;
            if (!File.Exists(progressFile)) {
                LevelPackProgressFile p = new LevelPackProgressFile();
                p.completedLevels = new List<string>();
                File.WriteAllText(progressFile, JsonTool.ObjectToString(p));
            }
            LevelPackProgressFile l = JsonTool.StringToObject<LevelPackProgressFile>(File.ReadAllText(progressFile));
            if (l.completedLevels == null) l.completedLevels= new List<string>();
            BlockSaveFile b = GameMasterManager.currentLevelPackLevels[currentLevelPackIndex].saveFile;
            if (!l.completedLevels.Contains(b.levelName)) l.completedLevels.Add(b.levelName);
            File.WriteAllText(progressFile, JsonTool.ObjectToString(l));
        } else {

        }
        GameMasterManager.currentLevelPackProgressFile = GetProgressFile();
    }

    public LevelPackProgressFile GetProgressFile() {
        if (levelPath == null || levelPath.Length == 0) {
            LevelPackProgressFile p = new LevelPackProgressFile();
            p.completedLevels = new List<string>();

            return p;
        }
        string progressFile = levelPath + "/" + SaveManager.progressPackFile;
        if (!File.Exists(progressFile)) {
            LevelPackProgressFile p = new LevelPackProgressFile();
            p.completedLevels = new List<string>();
            File.WriteAllText(progressFile, JsonTool.ObjectToString(p));
        }
        LevelPackProgressFile l = JsonTool.StringToObject<LevelPackProgressFile>(File.ReadAllText(progressFile));
        if (l.completedLevels == null) l.completedLevels = new List<string>();
        return l;
    }

    public string GetSpritePath() {
        return levelPath + "/" + SaveManager.packImageString;
    }
}



public class LevelPackProgressFile {

    public List<string> completedLevels;
    


}

