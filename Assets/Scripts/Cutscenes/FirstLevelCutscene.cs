using System.Collections.Generic;
using UnityEngine;






public class FirstLevelCutscene : CutScene {


    private StateMachine<FirstLevelCutscene> machine;

    [TextArea]
    public string wasdToMoveString;
    public float timeOnFirstString = 3.0f;

    [TextArea]
    public string pullMoveString;

    [TextArea]
    public string pulllingMoveString;



    [TextArea]
    public string endStateText;
    public float timeOnEndString = 3.0f;


    public Block block;


    public override void StartCutscene() {
        machine = new StateMachine<FirstLevelCutscene>(new WASDToMoveState(), this);
        block =  manager.generator.GetBlockNumber(0);
        manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(0, 0, -0.5f), block);

    }


    private void Update() {
        if (machine != null && machine.currentState != null)machine.Update();
        manager.generator.arrowIndicator.HideCondition(HideCondition());
       // if (block.currentPullLevel == 0 && !manager.generator.arrowIndicator.IsAttachedTo(block)) {  }
    }

    public bool HideCondition() {
        if (block.currentPullLevel != 0) return true;
        if (manager.player.isPulling()) return true;
        if (machine.currentState is WASDToMoveState) return true;

        return false;
    }

    private class WASDToMoveState : State<FirstLevelCutscene> {

        float fTime;

        public override void Enter(StateMachine<FirstLevelCutscene> obj) {
            obj.target.manager.ShowText(obj.target.wasdToMoveString);
            fTime = Time.time + obj.target.timeOnFirstString;
        }
        bool walkedYet;
        public override void Update(StateMachine<FirstLevelCutscene> obj) {
           // Debug.Log("UPDATE " + Time.time);
            if (!obj.target.manager.textBox.textMoving && (obj.target.manager.inputManager.horizontal != 0 || obj.target.manager.inputManager.vertical != 0 || obj.target.manager.inputManager.jumpDown)) {
                walkedYet = true;
            }

            if ((!obj.target.manager.textBox.textMoving && walkedYet && Time.time > fTime) || obj.target.manager.player.isPulling()) {
               // Debug.Log("?????");
                obj.ChangeState(new PullPromptState());
            }
        }


    }

    private class PullPromptState : State<FirstLevelCutscene> {


        public override void Enter(StateMachine<FirstLevelCutscene> obj) {
            obj.target.manager.ShowText(obj.target.pullMoveString);
        }


        public override void Update(StateMachine<FirstLevelCutscene> obj) {
            if (obj.target.manager.player.isPulling()) {
                obj.ChangeState(new PullMovePromptState());
            }
        }

    }

    private class PullMovePromptState : State<FirstLevelCutscene> {

        public override void Enter(StateMachine<FirstLevelCutscene> obj) {
            obj.target.manager.ShowText(obj.target.pulllingMoveString);
        }

        bool moved;

        public override void Update(StateMachine<FirstLevelCutscene> obj) {
            
            if (obj.target.manager.inputManager.vertical < 0) {
                moved = true;
            }

            if (!obj.target.manager.player.isPulling()) {
                if (moved) {
                    obj.ChangeState(new EndState());
                } else {
                    obj.ChangeState(new PullPromptState());
                }
            }


        }

        public class EndState : State<FirstLevelCutscene> {


            public override void Enter(StateMachine<FirstLevelCutscene> obj) {
                obj.target.manager.ShowText(obj.target.endStateText);
            }

            bool stopped = false;
            float endTime = 0;

            public override void Update(StateMachine<FirstLevelCutscene> obj) {
                if (!stopped && !obj.target.manager.textBox.textMoving) {
                    stopped = true;
                    endTime = Time.time + obj.target.timeOnEndString;
                }

                if (stopped && Time.time > endTime) {
                   // obj.target.manager.EndCurrentCutscene();
                }
            }

        }


    }



   



}

