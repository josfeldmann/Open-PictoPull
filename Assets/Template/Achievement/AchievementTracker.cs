#if !UNITY_WEBGL && !DISABLESTEAMWORKS
using Steamworks;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementTracker : MonoBehaviour {

    public GameMasterManager masterManager;
    public List<AchievementObject> toDisplayAchievements = new List<AchievementObject>();
    public AchievementDisplayerTab displayerTab;
    public bool displayingAchievement = false;
    public AchievementTab achievementState;
    public RectTransform inPosition, outPosition;
    public float tabSpeed = 200f;
    public float waitTime = 1.5f;
    private float timer;
    public List<AchievementObject> achievements = new List<AchievementObject>();
    private Dictionary<string, AchievementObject> achievementDict = new Dictionary<string, AchievementObject>();
    public BoolAchievementObject completeAllAchievement;

    private void Awake()
    {
#if UNITY_SWITCH
        achievements = achievements.Where(achievement => achievement.isAvailableOnSwitch).ToList();
#endif
    }

    public void CheckAllAchievements() {
        foreach (AchievementObject a in achievements) {
            CheckAchievement(a);
        }
        CheckCompleteAllAchivement();
    }

    public void Setup() {
        if ( GameMasterManager.gameSaveFile.trackerSaveFile == null) GameMasterManager.gameSaveFile.trackerSaveFile = new AchievementTrackerSaveFile();
        achievementDict = new Dictionary<string, AchievementObject>();
        foreach (AchievementObject a in achievements) {
            achievementDict.Add(a.GetID(), a);
        }
        if (!displayingAchievement) displayerTab.rect.anchoredPosition = outPosition.anchoredPosition;
    }


    public static float lastSavedTime = 0;
    
    public static float GetCurrentTime() {
       float amt =  GameMasterManager.gameSaveFile.trackerSaveFile.GetTotalTimePlayed() + Time.time - lastSavedTime;

        return amt;
    }



    public void SaveTime() {
        try {
            GameMasterManager.gameSaveFile.trackerSaveFile.SaveCurrentTime(GetCurrentTime());
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
        lastSavedTime = Time.time;
    }





    

    public void CheckAchievement(AchievementObject achievement) {
        string id = achievement.GetID();
        if (GameMasterManager.gameSaveFile.trackerSaveFile.completedAchievements.Contains(id)) {
            return;
        }

        if (achievement.IsFulfilled(this)) {
            GameMasterManager.gameSaveFile.trackerSaveFile.completedAchievements.Add(id);
            GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime.Add(id, AchievementTracker.GetCurrentTime());
            toDisplayAchievements.Add(achievement);
            StartShowingAchievement();
#if !UNITY_WEBGL && !DISABLESTEAMWORKS
            try {
                if (SteamManager.isSteamBuild && achievement.associatedSteamAchievement != null && achievement.associatedSteamAchievement != null) {

                    Steamworks.SteamUserStats.SetAchievement(achievement.associatedSteamAchievement);
                    Steamworks.SteamUserStats.StoreStats();
                }
            } catch (Exception e) {
                Debug.LogError(e.StackTrace);
            }
        #endif
            GameMasterManager.instance.SaveGameFile();
        }
    }

    public void ForceCheckAllSteamAchievements() {
#if !DISABLESTEAMWORKS
        try {
            bool updated = false;
            foreach (AchievementObject a in achievements) {
                if (GameMasterManager.gameSaveFile.trackerSaveFile.completedAchievements.Contains(a.GetID())) {
                    print("ACHIEVEMENT " + a.GetName() + Time.time);
                    bool tutorialCompleted;
                    Steamworks.SteamUserStats.GetAchievement(a.associatedSteamAchievement, out tutorialCompleted);
                    if (tutorialCompleted == false) {
                        Steamworks.SteamUserStats.SetAchievement(a.associatedSteamAchievement);
                        updated = true;
                        print(a.associatedSteamAchievement);
                    }
                }
            }
            if (updated) Steamworks.SteamUserStats.StoreStats();
        } catch (Exception e) {
            Debug.LogError(e.StackTrace);
        }
#endif
    }

    

    public void StartShowingAchievement() {
        if (displayingAchievement) {

        } else if (toDisplayAchievements.Count > 0) {
            displayingAchievement = true;
            displayerTab.SetAchievement(toDisplayAchievements[0]);
            displayerTab.rect.anchoredPosition = outPosition.anchoredPosition;
            achievementState = AchievementTab.IN;
        }
    }

    private void Update() {
        if (displayingAchievement) {
            switch (achievementState) {
                case AchievementTab.IN:
                    if (displayerTab.rect.anchoredPosition != inPosition.anchoredPosition) {
                        displayerTab.rect.anchoredPosition = Vector2.MoveTowards(displayerTab.rect.anchoredPosition, inPosition.anchoredPosition, tabSpeed * Time.deltaTime);
                    } else {
                        achievementState = AchievementTab.WAIT;
                        timer = Time.time + waitTime;
                    }
                    break;
                case AchievementTab.OUT:
                    if (displayerTab.rect.anchoredPosition != outPosition.anchoredPosition) {
                        displayerTab.rect.anchoredPosition = Vector2.MoveTowards(displayerTab.rect.anchoredPosition, outPosition.anchoredPosition, tabSpeed * Time.deltaTime);
                    } else {
                        displayingAchievement = false;
                        toDisplayAchievements.RemoveAt(0);
                        if (toDisplayAchievements.Count > 0) StartShowingAchievement();
                    }
                    break;
                case AchievementTab.WAIT:
                    if (timer < Time.time) {
                        achievementState = AchievementTab.OUT;
                    }
                    break;
            }
        }
    }

    public void CheckAchievements<T>() where T : AchievementObject {
        foreach (AchievementObject a in achievements) {
            if (a is T)CheckAchievement(a);
        }
        CheckCompleteAllAchivement();
    }


    public void CheckCompleteAllAchivement() {
        if (GameMasterManager.gameSaveFile.trackerSaveFile.completedAchievements.Contains(completeAllAchievement.GetID())) return;
        int i = 0; 
        foreach (AchievementObject o in achievements) {
            if (!GameMasterManager.gameSaveFile.trackerSaveFile.completedAchievements.Contains(o.GetID())) {
                i++;
            }
        }

        if (i <= 1) {
            completeAllAchievement.CompleteAchievement();
        }


    }

    
}
