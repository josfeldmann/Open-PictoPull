using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAchievement : MonoBehaviour
{

    public AchievementTracker tracker;
    public AchievementObject objectToCheck;
    public AchievementObject objectToCheck2;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        #if !UNITY_SWITCH
        if (Keyboard.current.kKey.wasPressedThisFrame) {
            //GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.Add("Test", true);
            tracker.CheckAllAchievements();
        }
        #endif
    }
}
