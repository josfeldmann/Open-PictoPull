using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Achievements/IntAchievement")]
public class IntAchievementObject : AchievementObject {

    public List<IntComparison> ints = new List<IntComparison>();

    public override float GetProgressMax() {
        foreach (IntComparison i in ints) {
            return i.amount;
        }
        return 0;
    }

    public override float GetCurrentProgress() {
       
        foreach (IntComparison i in ints) {
            if (!GameMasterManager.gameSaveFile.trackerSaveFile.savedInts.ContainsKey(i.key)) return 0;
            return GameMasterManager.gameSaveFile.trackerSaveFile.savedInts[i.key];
        }
        return 0;
    }

    public override bool IsFulfilled(AchievementTracker tracker) {
        if (GameMasterManager.isDemo) return false;

        foreach (IntComparison s in ints) {
            
            if (GameMasterManager.gameSaveFile.trackerSaveFile.savedInts.ContainsKey(s.key)) {
                int amt = GameMasterManager.gameSaveFile.trackerSaveFile.savedInts[s.key];
                switch (s.intComparison) {
                    case INTCOMPARISON.GREATERTHAN:
                        if (amt <= s.amount) return false;
                        break;
                    case INTCOMPARISON.GREATEREQUAL:
                        if (amt < s.amount) return false;
                        break;
                    case INTCOMPARISON.LESS:
                        if (amt >= s.amount) return false;
                        break;
                    case INTCOMPARISON.LESSEQUAL:
                        if (amt > s.amount) return false;
                        break;
                    case INTCOMPARISON.EQUAL:
                        if (amt != s.amount) return false;
                        break;
                }
            } else {
                return false;
            }
        }
        return true;
    }
}




