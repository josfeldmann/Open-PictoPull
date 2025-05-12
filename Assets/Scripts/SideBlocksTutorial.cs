using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBlocksTutorial : CutScene {
    public override void StartCutscene() {


        bottomBlock = GameMasterManager.instance.generator.GetBlockNumber(0);
        midBlock = GameMasterManager.instance.generator.GetBlockNumber(1);
        thirdBlock = GameMasterManager.instance.generator.GetBlockNumber(2);
        

        machine = new StateMachine<SideBlocksTutorial>(new FirstString(), this);

    }

    public override void UpdateCutScene() {
        if (machine != null) machine.Update();
        SetPosition();
    }

    public StateMachine<SideBlocksTutorial> machine;

    public string firstString = "To solve this puzzle you will need to pull blocks from the side", secondString = "First pull out the bottom two blocks as much as possible", thirdString = "Now pull out the purple or red block",
     fourthString = "Now pull it out twice with [Vertical]!";
    public float lingerTime = 3;

    public Block bottomBlock, midBlock, thirdBlock;

    Vector3 sidePos = new Vector3(3, 2, 0.5f);

    public void SetPosition() {
        if (manager.player.isPulling()) {
            manager.generator.arrowIndicator.Hide();
            return;
        }
        if (bottomBlock.currentPullLevel < 3) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(bottomBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 0, -0.5f), bottomBlock);
        } else if (midBlock.currentPullLevel < 2) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(midBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 1, -0.5f), midBlock);
        } else if (thirdBlock.currentPullLevel < 1) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(thirdBlock)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 2, -0.5f), thirdBlock);
        } else if (thirdBlock.currentPullLevel > 0 &&  thirdBlock.currentPullLevel < 2) {
            if (manager.generator.arrowIndicator.transform.localPosition != sidePos)manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.LEFT, sidePos, thirdBlock);
        } else {
            manager.generator.arrowIndicator.Hide();
        }

    }



    private class FirstString : State<SideBlocksTutorial> {
        public override void Enter(StateMachine<SideBlocksTutorial> obj) {
            obj.target.manager.ShowText(obj.target.firstString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SideBlocksTutorial> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && Time.time > nextTime) {
                obj.ChangeState(new SecondString());
            }
        }

    }



    private class SecondString : State<SideBlocksTutorial> {
        public override void Enter(StateMachine<SideBlocksTutorial> obj) {
            obj.target.manager.ShowText(obj.target.secondString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SideBlocksTutorial> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && obj.target.bottomBlock.currentPullLevel == 3 && obj.target.midBlock.currentPullLevel == 2) {
                obj.ChangeState(new ThirdString());
            }
        }

    }

    private class ThirdString : State<SideBlocksTutorial> {
        public override void Enter(StateMachine<SideBlocksTutorial> obj) {
            obj.target.manager.ShowText(obj.target.thirdString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SideBlocksTutorial> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && (obj.target.thirdBlock.currentPullLevel == 1) && !obj.target.manager.player.isPulling()) {
                obj.ChangeState(new FourthString());
            }
        }

    }
    private class FourthString : State<SideBlocksTutorial> {
        public override void Enter(StateMachine<SideBlocksTutorial> obj) {
            obj.target.manager.ShowText(obj.target.fourthString);
        }

        float nextTime = 0;

        bool isStopped = false;

        public override void Update(StateMachine<SideBlocksTutorial> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && Time.time > nextTime && (obj.target.thirdBlock.currentPullLevel >= 2)) {
                obj.target.manager.EndCurrentCutscene();
            }
        }

    }

}


