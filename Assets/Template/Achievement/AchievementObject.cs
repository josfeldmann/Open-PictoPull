using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementObject : ScriptableObject
{
    [SerializeField] protected string id;
    [SerializeField] protected string achievementName;
    [SerializeField] protected string achivementDescription;
    public Sprite achievementSprite;
    public string associatedSteamAchievement;
    public bool useProgressBar;
    public bool isAvailableOnSwitch;

    public string GetID() {
        return id;
    }

    public string GetDescription() {
        return achivementDescription;
    }

    public string GetName() {
        return achievementName;
    }

    public virtual bool IsFulfilled(AchievementTracker tracker) {
        return false;
    }

    public virtual float GetProgressMax() {
        return 0;
    }

    public virtual float GetCurrentProgress() {
        return 0;
    }
}


[System.Serializable]
public class AchievementTrackerSaveFile {


    public const string NUMBEROFJUMPKEY = "JUMPS", LEVELSCOMPLETED = "LEVELSCOMPLETED", TOTALTIME = "TOTALTIME", TOTALJUMPS = "TOTALJUMPS", TOTALPULLOUTS = "TOTALPULLOUTS", TOTALPUSHES = "TOTALPUSHES", TOTALPORTALS = "TOTALPORTALS", TOTALCANNONS = "TOTALCANNONS", TOTALRESETS = "TOTALRESETS", ARTIST = "ARTIST";


    public HashSet<string> completedAchievements = new HashSet<string>();
    public Dictionary<string, float> achievementCompletionTime = new Dictionary<string, float>();

  

    public Dictionary<string, bool> savedBools = new Dictionary<string, bool>();
    public Dictionary<string, int> savedInts = new Dictionary<string, int>();
    public Dictionary<string, float> savedFloats = new Dictionary<string, float>();
    public Dictionary<string, string> savedStrings = new Dictionary<string, string>();

    public float GetTotalTimePlayed() {
        if (!savedFloats.ContainsKey(TOTALTIME)) {
            savedFloats.Add(TOTALTIME, 0);
        }
        return savedFloats[TOTALTIME];
    }

    internal int GetJumps() {
        return GetInt(TOTALJUMPS);
    }

    public void SaveCurrentTime(float v) {
        savedFloats[TOTALTIME] = v;
    }

    public int GetCompletedLevels() {
        if (!savedInts.ContainsKey(LEVELSCOMPLETED)) {
            savedInts.Add(LEVELSCOMPLETED, 0);
        }
        return savedInts[LEVELSCOMPLETED];
    }

    public void AddInt(string key, int amt) {
        if (!savedInts.ContainsKey(key)) {
            savedInts[key] = 0;
        }
        savedInts[key] += amt;
    }

    public int GetInt(string key) {
        if (!savedInts.ContainsKey(key)) {
            return 0;
        }
        return savedInts[key];
    }

    public void AddJumps(int amt) {
        AddInt(TOTALJUMPS,amt);
    }

    public void AddPushes(int amt) {
        AddInt(TOTALPUSHES, amt);
    }

    public void AddPortals(int amt) {
        AddInt(TOTALPORTALS, amt);
    }

    public int GetPullouts() {
       return GetInt(TOTALPULLOUTS);
    }


    public int AddJumps() {
        return GetInt(TOTALJUMPS);
    }

    public int GetPushes() {
        return GetInt(TOTALPUSHES);
    }

    public int GetPortals() {
        return GetInt(TOTALPORTALS);
    }

    public int GetCannons() {
        return GetInt(TOTALCANNONS);
    }

    public int GetResets() {
        return GetInt(TOTALRESETS);
    }

    public void AddPullouts(int amt) {
        AddInt(TOTALPULLOUTS, amt);
    }

    public void AddCannons(int amt) {
        AddInt(TOTALCANNONS, amt);
    }

    public void AddResets(int amt) {
        AddInt(TOTALRESETS,amt);
    }





}

public enum AchievementTab {
    IN = 0, OUT = 1, WAIT = 2
}
