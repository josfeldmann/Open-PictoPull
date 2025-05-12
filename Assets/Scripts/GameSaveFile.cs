using System.Collections.Generic;

[System.Serializable]
public class GameSaveFile {
    public List<StoryChapters> unlockedStoryChapters;
    public HashSet<string> completedStoryLevels;
    public HashSet<string> skippedLevels;
    public HashSet<string> completedCustomLevels;
    public int currentSelectedLevelGroup = 0;
    public string avatarID = "";
    public int bodyID = 0;
    public string lastEnv;
    public HashSet<string> viewedStoryCutscenes;
    public AchievementTrackerSaveFile trackerSaveFile;

    public bool hidegameplayUI = false;
    public bool skipCutscenes = false;
    
#if UNITY_SWITCH
    public float musicVol = 0.5f;
    public float sfxVol = 0.5f;
#endif

}
