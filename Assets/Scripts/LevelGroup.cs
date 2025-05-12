using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "LevelGroup")]
public class LevelGroup : ScriptableObject {

    [SerializeField] private string levelName;
    public Sprite iconSprite;
    public List<TextAsset> jsonLevels = new List<TextAsset>();
    public List<LevelInfo> levels = new List<LevelInfo>();


    public EnvironmentGroup envGroup;
    public LevelAchievementObject levelAchievement;
   

    [Header("UnlockInfo")]
    public StoryChapters StoryChapter;
    public List<StoryChapters> mustBeUnlockedToAppear = new List<StoryChapters>();
    public List<UnlockCondition> unLockConditions = new List<UnlockCondition>();

    public bool excludeFromLevelSelect = false;

    internal bool isLevelUnlocked() {
        if (GameMasterManager.gameSaveFile.unlockedStoryChapters.Contains(StoryChapter)) {
            return true;
        }
        foreach (UnlockCondition c in unLockConditions) {
            if (!c.IsConditionMet()) { return false; }
        }
        return true;
    }

    internal int GetCompletedLevelNumber() {
        int i = 0;
        for (int x = 0; x < levels.Count; x++) {
            if (GameMasterManager.gameSaveFile.completedStoryLevels.Contains(levels[x].saveFile.GetSaveKey())) {
                i++;
            }
        }
        return i++;
    }



    internal string GetUnlockString() {
        foreach (UnlockCondition c in unLockConditions) {
            if (!c.IsConditionMet()) return c.GetUnlockConditionString();
        }
        return "";
    }

    internal bool IsVisible() {

        foreach (StoryChapters s in mustBeUnlockedToAppear) {
            if (!GameMasterManager.gameSaveFile.unlockedStoryChapters.Contains(s)) {
                Debug.Log("false");
                return false;
            }
        }
        return true;
    }

  

    public string GetLevelName() {
        return levelName;
    }

    public bool AllLevelsCompleted() {
        foreach (LevelInfo l in levels) {
            
            if (!GameMasterManager.gameSaveFile.completedStoryLevels.Contains(l.saveFile.GetSaveKey()) && !GameMasterManager.gameSaveFile.skippedLevels.Contains(l.saveFile.GetSaveKey())) {
                return false;
            }
        }
        return true;
    }


    [ContextMenu("Clear All")]
    public void ClearAll() {
        levels.RemoveAll(t => (t == null || t.saveFile == null));
        jsonLevels.RemoveAll(t => (t == null));
    }

    [ContextMenu("MakeTextures")]
    public void MakeTexturesForAllLevels() {

        levels = new List<LevelInfo>();
        foreach (TextAsset t in jsonLevels) {
            LevelInfo l = new LevelInfo(t);
            l.saveFile.environmentName = envGroup.envKey;
            l.storyOverideName = (GameMasterManager.instance.levels.IndexOf(this) + 1).ToString() + "-" + (jsonLevels.IndexOf(t) + 1).ToString() + " " + l.saveFile.levelName;
            levels.Add(l);
        }

       // foreach (LevelInfo l in levels) {
         //   l.MakeSprite();
       // }
#if UNITY_EDITOR
      //  EditorUtility.SetDirty(this);
#endif
    }


    [ContextMenu("ApplyColorPalette")]
    public void ApplyColorPaletteToAllLevels() {


        foreach (TextAsset t in jsonLevels) {

            BlockSaveFile s = JsonTool.StringToObject<BlockSaveFile>(t.text);
            //s = BlockLevelCreator.ApplyColorPalette(s, colorPalette);


#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(t);
            Debug.Log(path);
            File.WriteAllText(path, JsonTool.ObjectToString<BlockSaveFile>(s));
#endif

        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif


    }


}
