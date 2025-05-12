using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCutscene : CutScene {

    [TextArea]
    public string firstString = "Blocks can be pulled out up to 3 times", secondString = "Pull out the green block 3 times and jump on top of it";

    [TextArea]
    public string thirdString = "Now you can pull out the red block 2 times", fourthString = "You can't pull out a block if you would fall as a result of the pull";

    [TextArea]
    public string finalString = "Now pull the yellow block to reveal the treasure and grab it";

    public StateMachine<SecondCutscene> machine;

    public float lingerTime = 3f;

    public override void StartCutscene() {
        machine = new StateMachine<SecondCutscene>(new SecondString(), this);

        BottomBlock = GameMasterManager.instance.generator.GetBlockNumber(0);
        midBlock = GameMasterManager.instance.generator.GetBlockNumber(1);
        topBlock = GameMasterManager.instance.generator.GetBlockNumber(2);
        manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 0, -0.5f), BottomBlock);

    }

    public void SetPosition() {
        if (manager.player.isPulling()) return;
        if (BottomBlock.currentPullLevel < 3) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(BottomBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 0, -0.5f), BottomBlock);
        } else if (midBlock.currentPullLevel < 2) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(midBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 1, -0.5f), midBlock);
        } else if (topBlock.currentPullLevel < 1) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(topBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 2, -0.5f), topBlock);
        } else {
            manager.generator.arrowIndicator.Hide();
        }

    }

    public Block BottomBlock, midBlock, topBlock;


    public bool HideCondition() {
        if (manager.player.isPulling()) return true;    
        return false;
    }

    public override void UpdateCutScene() {
        if (machine != null) machine.Update();
        manager.generator.arrowIndicator.HideCondition(HideCondition());
        SetPosition();
        
    }

    private class FirstString : State<SecondCutscene> {
        public override void Enter(StateMachine<SecondCutscene> obj) {
            obj.target.manager.ShowText(obj.target.firstString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SecondCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && Time.time > nextTime) {
                obj.ChangeState(new SecondString());
            }
        }

    }

    private class SecondString : State<SecondCutscene> {
        public override void Enter(StateMachine<SecondCutscene> obj) {
            obj.target.manager.ShowText(obj.target.secondString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SecondCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && !obj.target.manager.player.isPulling() && obj.target.BottomBlock.currentPullLevel == 3) {
                obj.ChangeState(new FourthString());
            }
        }

    }

    private class ThirdString : State<SecondCutscene> {
        public override void Enter(StateMachine<SecondCutscene> obj) {
            obj.target.manager.ShowText(obj.target.thirdString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SecondCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && nextTime < Time.time) {
                obj.ChangeState(new FourthString());
            }
        }

    }

    private class FourthString : State<SecondCutscene> {
        public override void Enter(StateMachine<SecondCutscene> obj) {
            obj.target.manager.ShowText(obj.target.fourthString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SecondCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && nextTime < Time.time && obj.target.midBlock.currentPullLevel == 2) {
                obj.ChangeState(new FinalString());
            }
        }

    }

    private class FinalString : State<SecondCutscene> {
        public override void Enter(StateMachine<SecondCutscene> obj) {
            obj.target.manager.ShowText(obj.target.finalString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SecondCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && Time.time > nextTime) {
                //obj.target.manager.EndCurrentCutscene();
            }
        }


    }


}
