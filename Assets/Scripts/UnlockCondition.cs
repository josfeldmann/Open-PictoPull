using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "UnlockConditions/CompleteAllLevels")]
public class UnlockCondition : ScriptableObject {


    public List<LevelGroup> watchedGroups = new List<LevelGroup>();


    public bool IsConditionMet() {
        foreach (LevelGroup g in watchedGroups) {
            if (!g.AllLevelsCompleted()) return false;
        }
        return true;
    }

    public string GetUnlockConditionString() {
        foreach (LevelGroup g in watchedGroups) {
            if (!g.AllLevelsCompleted()) return "Complete all " + g.GetLevelName() + " levels";
        }
        return "Unlocked!";

    }
}
