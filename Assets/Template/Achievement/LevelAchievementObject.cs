using UnityEngine;

[CreateAssetMenu(menuName = "Achievements/LevelAchievement")]
public class LevelAchievementObject : AchievementObject {

    public LevelGroup levelGroup;


    public int NumberOfCompleteLevels() {
        int amt = 0;
        
        foreach (LevelInfo l in levelGroup.levels) {
            if (GameMasterManager.gameSaveFile.completedStoryLevels.Contains(l.saveFile.levelName)) amt++;
        }
        
        return amt;

    }
    public override bool IsFulfilled(AchievementTracker tracker) {
        if (GameMasterManager.isDemo) return false;
        if (NumberOfCompleteLevels() >= levelGroup.levels.Count) return true;
        return false;
    }

    public override float GetProgressMax() {
        return levelGroup.levels.Count;
    }

    public override float GetCurrentProgress() {
        return NumberOfCompleteLevels();
    }



}




