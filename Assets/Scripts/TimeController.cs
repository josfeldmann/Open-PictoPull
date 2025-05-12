using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


[System.Serializable]
public class PlayerPositionInfo {
    public Vector3 vec;
    public Quaternion quat;
    public bool grounded;
    public bool pulling;
    public bool walking;
    public int oppositeLevel;
    public CrashMoveInfo crashMoveInfo;

    public PlayerPositionInfo(Vector3 vec, Quaternion quat, List<BlockMovement> b, bool g, bool p, bool w, int oppositeLevel, List<BlockTimerValue> timers, CrashMoveInfo crash) {
        this.vec = vec;
        this.quat = quat;
        movedBlocks = b;
        grounded = g;
        walking = w;
        pulling = p;
        this.oppositeLevel = oppositeLevel;
        timerVals = timers;
        crashMoveInfo = crash;
        
    }
    public List<BlockMovement> movedBlocks = new List<BlockMovement>();
    public List<BlockTimerValue> timerVals = new List<BlockTimerValue>();
    
}


[System.Serializable]
public class BlockMovement{
    public Block block;
    public int setPullLevel;
    

    public BlockMovement(Block b, int returnValue) {
        this.block = b;
        this.setPullLevel = returnValue;
    }
}


[System.Serializable]
public class BlockTimerValue {

    public Block block;
    public float timerVal;

    public BlockTimerValue(Block b, float timerValue) {
        this.block = b;
        this.timerVal = timerValue;
    }

}






public class TimeController : MonoBehaviour
{
    public static TimeController instance;
    public static bool isReversing;
    public static float rewindSpeedInterval;
    public float recordInterval = 0.1f;
    public float rewindMultiple = 2f;
    public float storedSeconds = 200f;
    public static int arrayLength = 0;
    
    public Transform playerTransform;
    public bool recording;
    public int currentIndex = 0;
    public float lastTime = 0;

    [HideInInspector] public static List<BlockMovement> blocksMovedThisInterval = null;
    [HideInInspector] public static List<BlockTimerValue> blockTimerThisInterval = null;
    [HideInInspector] public static PlayerPositionInfo[] playerPosition;


    [HideInInspector] public static List<Block> crashBlocksMovingThisInterval;
    [HideInInspector] public static List<CrashMoveBlockPosition> crashMovesCompletedThisInterval;



    private void Awake() {
        instance = this;
        rewindSpeedInterval = rewindMultiple;
    }

    public static void AddBlockMove(Block b, int returnValue) {
        
        if (blocksMovedThisInterval == null) blocksMovedThisInterval = new List<BlockMovement>();
        blocksMovedThisInterval.Add(new BlockMovement(b, returnValue));
    }

    public static void AddBlockTimerThisFrame(Block b) {
        if (blockTimerThisInterval == null) blockTimerThisInterval = new List<BlockTimerValue>();
        blockTimerThisInterval.Add(new BlockTimerValue(b, b.timerReTract));
    }


    public void ResetTimeline() {
        arrayLength = (int)(storedSeconds / recordInterval);
        playerPosition = new PlayerPositionInfo[arrayLength];
        blocksMovedThisInterval = null;
        crashBlocksMovingThisInterval = null;
        crashMovesCompletedThisInterval = null;
        currentIndex = 0;
        prevIndex = 0;
        lastTime = Time.time;
    }

    public void StartRecording() {
        recording = true;
    }

    public void StopRecording() {
        recording = false;
    }

    int prevIndex;

    private void Update() {
        if (recording && GameMasterManager.inGameArea && (playerPosition[prevIndex] == null || playerPosition[prevIndex].vec != playerTransform.position || GameMasterManager.instance.generator.timerPresent || GameMasterManager.instance.playerController.InCrashMoveState())) {

            if (Time.time > lastTime) {
                bool walking = GameMasterManager.instance.playerController.machine.currentState is PlayerIdleState && (GameMasterManager.instance.inputManger.horizontal != 0 || GameMasterManager.instance.inputManger.horizontal != 0);
                bool pulling = GameMasterManager.instance.playerController.machine.currentState is PlayerPushState;

                CrashMoveInfo c = null;

                if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
                    if (crashBlocksMovingThisInterval != null && crashBlocksMovingThisInterval.Count > 0) {
                        c = new CrashMoveInfo();
                        c.finishedThisFrame = crashMovesCompletedThisInterval;
                        c.movingBlocks = crashBlocksMovingThisInterval;
                        
                    }
                }

                playerPosition[currentIndex] = new PlayerPositionInfo(playerTransform.position, playerTransform.rotation, blocksMovedThisInterval, PlayerController.instance.feetCheck.grounded, pulling, walking, GameMasterManager.instance.generator.oppositePullLevel, blockTimerThisInterval, c);

                if (blocksMovedThisInterval != null) {
                    blocksMovedThisInterval = null;
                }
                if (blockTimerThisInterval != null) {
                    blockTimerThisInterval = null;
                }

                if (crashBlocksMovingThisInterval != null) {
                    crashBlocksMovingThisInterval = null;
                }

                if (crashMovesCompletedThisInterval != null) {
                    crashMovesCompletedThisInterval = null;
                }
               
                prevIndex = currentIndex;
                currentIndex++;
                if (currentIndex >= arrayLength) {
                    currentIndex = 0;
                }
                lastTime = Time.time + recordInterval;
            }
        }
    }

    public Quaternion GetLastRotation() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].quat;
        }
        return playerPosition[currentIndex - 1].quat;
    }

    public bool GetLastGrounded() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].grounded;
        }
        return playerPosition[currentIndex - 1].grounded;
    }

    public PlayerPositionInfo GetLastInfo() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1];
        }
        return playerPosition[currentIndex - 1];
    }

    public Vector3 GetLastPosition() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].vec;
        }
        return playerPosition[currentIndex - 1].vec;
    }

    public int GetLastOppositeLevel() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].oppositeLevel;
        }
        return playerPosition[currentIndex - 1].oppositeLevel;
    }


    public void DecreaseIndex() {
        playerPosition[currentIndex] = null;

        currentIndex--;
        if (currentIndex  < 0) {
            currentIndex = playerPosition.Length - 1;
        }
    }

    public bool IsNextNull() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1] == null;
        }
        return playerPosition[currentIndex - 1] == null;
    }

    public bool RewindBlockThisFrame() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].movedBlocks != null;
        }
        return playerPosition[currentIndex - 1].movedBlocks != null;
    }

    public List<BlockMovement> GetLastMovedBlocks() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].movedBlocks;
        }
        return playerPosition[currentIndex - 1].movedBlocks;
    }

   

   

    public bool RewindTimerThisFrame() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].timerVals != null;
        }
        return playerPosition[currentIndex - 1].timerVals != null;
    }

    public List<BlockTimerValue> GetLastTimeRewind() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].timerVals;
        }
        return playerPosition[currentIndex - 1].timerVals;
    }

    internal void AddCrashBlockMove(Block block) {
        if (crashBlocksMovingThisInterval == null) crashBlocksMovingThisInterval = new List<Block>();
        crashBlocksMovingThisInterval.Add(block);
    }

    public void AddCrashEndMove(CrashMoveBlockPosition b) {
        if (crashMovesCompletedThisInterval == null) crashMovesCompletedThisInterval = new List<CrashMoveBlockPosition>();
        crashMovesCompletedThisInterval.Add(b);
    //    Debug.Log("END END END " + Time.time.ToString() + " " + b.startPos.ToString() + " " + b.endPos.ToString());
    }

    internal CrashMoveInfo GetLastCrashInfo() {
        if (currentIndex == 0) {
            return playerPosition[playerPosition.Length - 1].crashMoveInfo;
        }
        return playerPosition[currentIndex - 1].crashMoveInfo;
    }
}


public class CrashMoveInfo {

    public List<Block> movingBlocks = new List<Block>();
    public List<CrashMoveBlockPosition> finishedThisFrame;



}

public class CrashMoveBlockPosition {
    public Block block;
    public Vector3 startPos;
    public Vector3 endPos;
    internal Vector3 crashRevertToPos;
}


