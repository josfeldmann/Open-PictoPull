using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Achievements/BoolAchievement")]
public class BoolAchievementObject : AchievementObject {

    public List<string> boolIds = new List<string>();

    public override bool IsFulfilled(AchievementTracker tracker) {

      //  Debug.Log(name + " " + Time.time);

        if (GameMasterManager.isDemo) return false;
        foreach (var s in boolIds)
        {
            return GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.TryGetValue(s, out var b) && b;
        }
        return true;
    }

    public void CompleteAchievement() {
        foreach (string s in boolIds) {
            if (!GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.ContainsKey(s))GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.Add(s, true);
        }
        GameMasterManager.instance.tracker.CheckAchievements<BoolAchievementObject>();
    }

}

[System.Serializable]
public enum INTCOMPARISON { GREATERTHAN = 0, GREATEREQUAL = 1, LESS = 2, LESSEQUAL = 3, EQUAL = 4 }


[System.Serializable]
public class IntComparison {
    public string key;
    public INTCOMPARISON intComparison;
    public int amount;
}




