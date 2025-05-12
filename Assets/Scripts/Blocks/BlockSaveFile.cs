#if !DISABLESTEAMWORKS
using Steamworks;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SimplifiedVector3 {
    public float x; public float y; public float z;
    public SimplifiedVector3(float x1, float y1, float z1) {
        x = x1;
        y = y1;
        z = z1;
    }
}


[System.Serializable]
public class OppositePairs {
    
    public List<int> positiveIndexes = new List<int>();
    public List<int> negativeIndexes = new List<int>();
}


[System.Serializable]
public class BlockSaveFile 
{


    public const String FourLevel = "4", FiveLevel = "5", SixLevel = "6", AllUnion = "All�nion", AllTimer = "AllTimer";


    public LevelType levelType;
    public string levelName;
    public string levelDescription;
    public int width, height;
    public List<SimplifiedVector3> colors;
    public int[,] colorGrid;
    public Vector2Int goalSpot = new Vector2Int();
    public Vector3Int threeDGoalSpot;
    public List<SpecialTile> specialTile;
    public string cutsceneid;
    public List<string> messages = new List<string>();
    public string associatedSteamID;
    public int difficulty;
    public string environmentName;
    public bool isSteam;
    public OppositePairs oppositeInfo;
    public int[,] depthGrid = null;

    //3D level stuff
    public bool is3DLevel = false;
    public int[,,] threeDMap;
    

    public Dictionary<string, string> extraTags = new Dictionary<string, string>();


    public string GetSaveKey() {
        return levelName;
    }

    public bool is4Level() {
        if (extraTags == null) return false;
        return extraTags.ContainsKey(FourLevel);
    }

    public bool is5Level() {
        if (extraTags == null) return false;
        return extraTags.ContainsKey(FiveLevel);
    }
    public bool isSixLevel() {
        if (extraTags == null) return false;
        return extraTags.ContainsKey(SixLevel);
    }



    public BlockSaveFile() {

    }

    public BlockSaveFile(BlockSaveFile s) {
        this.levelName = s.levelName;
        this.levelDescription = s.levelDescription;
        this.environmentName = s.environmentName;
        this.levelType = s.levelType;
        this.width = s.width;
        this.height = s.height;
        this.colors = new List<SimplifiedVector3>(s.colors);
        this.goalSpot = s.goalSpot;
        this.specialTile = new List<SpecialTile>(s.specialTile);
        this.cutsceneid = s.cutsceneid;
        this.messages = new List<string>(s.messages);
        colorGrid = new int[s.colorGrid.GetLength(0), s.colorGrid.GetLength(1)];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                colorGrid[x, y] = s.colorGrid[x, y];
            }
        }
        this.difficulty = s.difficulty;
        this.associatedSteamID = s.associatedSteamID;
        this.isSteam = s.isSteam;
        this.oppositeInfo = s.oppositeInfo;
        this.extraTags = s.extraTags;
        this.depthGrid = s.depthGrid;
        this.EnsureDepthGridIsNotNull();

        this.is3DLevel = s.is3DLevel;
        this.threeDMap = s.threeDMap;
        this.threeDGoalSpot = s.threeDGoalSpot;

    }

    internal Color GetColor(int v) {
        return new Color(colors[v].x, colors[v].y, colors[v].z);
    }

    internal int GetDepthNumber() {
        if (is4Level()) return 4;
        if (is5Level()) return 5;
        if (isSixLevel()) return 6;
        return 3;
    }

    public void AddDepthTag(int levelDepth) {
        if (levelDepth == 4) {
            extraTags.Add(FourLevel, "4");
        } else if (levelDepth == 5) {
            extraTags.Add(FiveLevel, "5");
        } else if (levelDepth == 6) {
            extraTags.Add(SixLevel, "6");
        }
    }

    public bool isAllUnion() {
        return extraTags.ContainsKey(AllUnion) || extraTags.ContainsKey("AllÜnion");
    }

    public bool isAllTimer() {
        return extraTags.ContainsKey(AllTimer);
    }

    public void AddAllUnion() {
        extraTags.Add(AllUnion, AllUnion);
    }

    public void AddAllTimer() {
        extraTags.Add(AllTimer, AllTimer);
    }

    public bool isOpposite() {
        return oppositeInfo != null && oppositeInfo.positiveIndexes != null && oppositeInfo.negativeIndexes != null && oppositeInfo.positiveIndexes.Count > 0 && oppositeInfo.negativeIndexes.Count > 0;
    }

    internal void EnsureDepthGridIsNotNull() {
        if (depthGrid == null) {
            depthGrid = new int[width, height];
        }
    }
}


public class SpecialTile {

    public const int LADDERID = 0, PULLOUTBUTTON = 1, CANNONID = 2, UNIONID = 3, BLOCKERID = 4, TIMERID = 5, OPPOSITEID = 6, CLOUDID = 7, SUBGOALID = 8, COINID = 9;
    public const int FULLPULLOUT = 0, SINGLEPULLOUT = 1;

    public int id;
    public int subid;
    public int colorIndex;
    public Vector2Int position;
    public BlockDirection direction = BlockDirection.UP;
   

    public SpecialTile(SpecialTile value) {
        this.id = value.id;
        this.subid = value.subid;
        this.colorIndex = value.colorIndex;
        this.position = value.position;
        this.direction = value.direction;
    }

    public SpecialTile() {

    }

    public bool isLadder() {
        return id == LADDERID;
    }

    public bool isCannon() {
        return id == CANNONID;
    }

    public bool isFullPullout() {
        return id == PULLOUTBUTTON && subid == FULLPULLOUT;
    }

    public bool isSinglePullout() {
        return id == PULLOUTBUTTON && subid == SINGLEPULLOUT;
    }

    public bool isTimer() {
        return id == TIMERID;
    }

    public bool isBlocker() {
        return id == BLOCKERID;
    }

    public bool isOpposite() {
        return id == OPPOSITEID;
    }

    public bool isUnion() {
        return id == UNIONID;
    }

    public bool isPositiveOpposite() {
        return direction == BlockDirection.UP || direction == BlockDirection.RIGHT;
    }

    internal bool isCloud() {
        if (id == CLOUDID) return true;
        return false;
    }
}
