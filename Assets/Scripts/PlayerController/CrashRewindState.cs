using System.Collections.Generic;
using UnityEngine;

public class CrashRewindState : State<PlayerController> {
    public TimeController controller;
    float nextTime;
    float interval = 0;
    Vector3 targetPos = new Vector3();
    Quaternion targetQuat;
    float moveSpeed = 1f;
    float rotSpeed = 360f;

    float rewindMax;

    bool showedUIYet = false;
    float timeToSHowUI = 0;

    public override void Enter(StateMachine<PlayerController> obj) {
        movedBlocksThisRewind = new List<Block>();
        TimeController.isReversing = true;
        obj.target.rewindEffect.StartShowing();
        obj.target.FreezeRigidbodyMovement();
        obj.target.warpingEnabled = false;
        obj.target.floater.StopFloating();
        controller = obj.target.generator.timeController;
        controller.StopRecording();
        interval = obj.target.generator.timeController.recordInterval / controller.rewindMultiple;
        nextTime = Time.time + interval;
        if (controller.IsNextNull()) {
            obj.ChangeState(new PlayerIdleState());
            return;
        }
        targetPos = controller.GetLastPosition();
        targetQuat = controller.GetLastRotation();
        GameMasterManager.instance.generator.oppositePullLevel = controller.GetLastOppositeLevel();
        timeToSHowUI = Time.time + 0.5f;
        showedUIYet = false;

        if (TimeController.playerPosition[TimeController.playerPosition.Length - 1] == null) {
            rewindMax = controller.currentIndex;
        } else {
            rewindMax = controller.currentIndex + TimeController.playerPosition.Length;
        }
        obj.target.rewindBar.fillAmount = 1;
        obj.target.rewindSound.Play();
        obj.target.rewindStart.Play();
        SetAnimStuff();
        exiting = false;

    }

    public void SetAnimStuff() {
        PlayerPositionInfo pInfo = controller.GetLastInfo();
        if (pInfo.pulling != pulling) {
            if (pInfo.pulling) GameMasterManager.instance.playerController.anim.SetTrigger(PlayerController.pushingString);
            else {
                GameMasterManager.instance.playerController.anim.ResetTrigger(PlayerController.pushingString);
                GameMasterManager.instance.playerController.anim.SetTrigger(PlayerController.returnToIdleString);
            }
            pulling = pInfo.pulling;
        }

        if (pInfo.walking != walking) {
            GameMasterManager.instance.playerController.anim.SetBool(PlayerController.walking, pInfo.walking);
            walking = pInfo.walking;
        }
    }

    bool walking = false;
    bool pulling = false;


    public bool areBlocksMoving() {
        bool f = false;
        foreach (Block b in GameMasterManager.instance.generator.blocks) {
            if (b.moving) f = true;
        }
        return f;

    }

    bool exiting;

    bool crashMoving = false;

    public List<Block> currentBlocksMoving;

    public void EnsureBlocksAreGood() {
        if (movedBlocksThisRewind != null) {
            foreach (Block b in movedBlocksThisRewind) {
                b.SetGoalCrashPosition(b.crashRevertToPos, true);
            }
        }
    }

    public List<Block> movedBlocksThisRewind = new List<Block>();

    public override void Update(StateMachine<PlayerController> obj) {

        if (exiting) {
            if (!areBlocksMoving()) {
                EnsureBlocksAreGood();
                obj.target.generator.FullySetCrashBlocks();
                obj.ChangeState( new GravityCheckState(new PlayerIdleState()));
            }
            return;
        }


        if (!showedUIYet && Time.time >= timeToSHowUI) {
            showedUIYet = true;
            obj.target.rewindUI.SetActive(true);
        }


        if (obj.target.inputManager.timeRewindButtonPressed || (!PlayerController.instance.feetCheck.grounded) || crashMoving || (currentBlocksMoving!= null && currentBlocksMoving.Count > 0)) {

            if (Time.time >= nextTime) {
                if (controller.IsNextNull()) {
                    
                    obj.ChangeState(new GravityCheckState(new PlayerIdleState()));
                    return;
                }
                obj.target.transform.position = targetPos;
                obj.target.transform.rotation = targetQuat;
                targetPos = controller.GetLastPosition();
                targetQuat = controller.GetLastRotation();
                CrashMoveInfo crashMoveInfo = controller.GetLastCrashInfo();

                if (crashMoveInfo != null) {
                    currentBlocksMoving = crashMoveInfo.movingBlocks;
                    if (crashMoveInfo.finishedThisFrame != null) {
                        foreach (CrashMoveBlockPosition pos in crashMoveInfo.finishedThisFrame) {
                            if (!movedBlocksThisRewind.Contains(pos.block)) movedBlocksThisRewind.Add(pos.block);
                            pos.block.SetGoalCrashPosition(pos.startPos, false);
                            pos.block.crashRevertToPos = pos.startPos; 
                        }
                    }

                    foreach (Block b in currentBlocksMoving) {
                        if (!movedBlocksThisRewind.Contains(b)) movedBlocksThisRewind.Add(b);
                        if (b.transform.localPosition != b.crashRevertToPos) {

                            float speed = 4;
                            if (b.crashStartPosition.y != b.crashMovePosition.y) speed = 8;

                            b.transform.localPosition = Vector3.MoveTowards(b.transform.localPosition, b.crashRevertToPos, speed * Time.deltaTime);
                        }
                    }

                } else {
                    if (currentBlocksMoving != null) {
                        EnsureBlocksAreGood();

                    }
                    currentBlocksMoving = null;
                }


                GameMasterManager.instance.generator.oppositePullLevel = controller.GetLastOppositeLevel();
                SetAnimStuff();
                moveSpeed = Vector3.Distance(targetPos, obj.target.transform.position) / interval;
               
                controller.DecreaseIndex();
                nextTime = Time.time + interval;

                



            } else {
                obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, targetPos, moveSpeed * Time.deltaTime);
                obj.target.transform.rotation = Quaternion.RotateTowards(obj.target.transform.rotation, targetQuat, rotSpeed * Time.deltaTime);
            }
            obj.target.rewindBar.fillAmount = ((float)controller.currentIndex) / rewindMax;

        } else {
            exiting = true;
        }
    }


    public override void Exit(StateMachine<PlayerController> obj) {
        obj.target.UnFreezeRigidbodyMovement();
        obj.target.generator.timeController.StartRecording();
        TimeController.isReversing = false;
        obj.target.rewindEffect.Hide();
        obj.target.rewindUI.SetActive(false);
        obj.target.rewindSound.Stop();
        obj.target.rewindStop.Play();
    }

}

