using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCutscene : CutScene
{
    public Block topBlock;

    public string resetString, postResetString;
    public float lingerTime = 3f;

    public StateMachine<ResetCutscene> machine;


    public override void StartCutscene() {
        topBlock = GameMasterManager.instance.generator.GetFirstBlockOfIndex(3);
        topBlock.SetPullLevelInstantly(3);
        machine = new StateMachine<ResetCutscene>(new PreReset(), this);
    }

    private void Update() {
        if (machine != null) machine.Update();
    }

    private class PreReset : State<ResetCutscene> {

        public override void Enter(StateMachine<ResetCutscene> obj) {
            obj.target.manager.ShowText(obj.target.resetString);
            obj.target.manager.resetArrow.SetActive(true);
        }

        public override void Update(StateMachine<ResetCutscene> obj) {
            if (obj.target.topBlock.currentPullLevel == 0) {
                obj.ChangeState(new PostReset());
            }
        }

        public override void Exit(StateMachine<ResetCutscene> obj) {
            obj.target.manager.resetArrow.SetActive(false);
        }

    }

    private class PostReset : State<ResetCutscene> {


        bool isStopped;
        float nextTime;

        public override void Enter(StateMachine<ResetCutscene> obj) {
            obj.target.manager.ShowText(obj.target.postResetString);
        }


        public override void Update(StateMachine<ResetCutscene> obj) {
            if (!isStopped && !obj.target.manager.textBox.textMoving) {
                isStopped = true;
                nextTime = Time.time + obj.target.lingerTime;
            }

            if (isStopped && Time.time > nextTime) {
                obj.target.manager.EndCurrentCutscene();
            }
        }


    }


}
