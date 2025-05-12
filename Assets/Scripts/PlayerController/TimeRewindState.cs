using JetBrains.Annotations;
using UnityEngine;


public class TimeRewindState : State<PlayerController> {

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

        //Debug.Log("Entered Rewind " + Time.time.ToString());

        TimeController.isReversing = true;
        obj.target.rewindEffect.StartShowing();
        obj.target.FreezeRigidbodyMovement();
        obj.target.warpingEnabled = false;
        obj.target.floater.StopFloating();
        controller = obj.target.generator.timeController;
        controller.StopRecording();
        interval = obj.target.generator.timeController.recordInterval/controller.rewindMultiple;
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

    public override void Update(StateMachine<PlayerController> obj) {

        if (exiting) {
            if (!areBlocksMoving()) {
                if (GameMasterManager.currentLevel.saveFile.levelType == LevelType.CRASH)obj.target.generator.FullySetCrashBlocks();
                obj.ChangeState(new PlayerIdleState());
            }
            return;
        }


        if (!showedUIYet && Time.time >= timeToSHowUI) {
            showedUIYet = true;
            obj.target.rewindUI.SetActive(true);
        }

     
        if (obj.target.inputManager.timeRewindButtonPressed || (!PlayerController.instance.feetCheck.grounded) || crashMoving) {

            if (Time.time >= nextTime) {
                if (controller.IsNextNull()) {
                    obj.ChangeState(new PlayerIdleState());
                    return;
                }
                obj.target.transform.position = targetPos;
                obj.target.transform.rotation = targetQuat;
                targetPos = controller.GetLastPosition();
                targetQuat = controller.GetLastRotation();
                GameMasterManager.instance.generator.oppositePullLevel = controller.GetLastOppositeLevel();
                SetAnimStuff();
                moveSpeed = Vector3.Distance(targetPos, obj.target.transform.position) / interval;
                if (controller.RewindBlockThisFrame()) {
                    foreach (BlockMovement b in controller.GetLastMovedBlocks()) {
                        b.block.SetPullLevel(b.setPullLevel);
                    }
                }

               // Debug.Log("CheckingCrash");

             

                if (controller.RewindTimerThisFrame()) {
                    foreach (BlockTimerValue b in controller.GetLastTimeRewind()) {
                        b.block.SetTimeRewind(b.timerVal);
                    }
                }

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
       // Debug.Log("Exited Rewind " + Time.time.ToString());
    }


}

