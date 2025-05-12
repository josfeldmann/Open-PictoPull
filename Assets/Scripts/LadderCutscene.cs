using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderCutscene : CutScene
{
    StateMachine<LadderCutscene> machine;

    public Block firstBlock, secondBlock;
    public LadderBlock ladderBlock;

    public string ladder1 = "This puzzle has portals. Pullout the green block to show the portals.", ladder2 = "Now Jump on the portal and press [Interact] to travel through the portal", ladder3 = "Both sides of the portal must be unblocked in order for it to be used. Portals can also point in any direction", ladder4 = "See if you can figure out how to get up to that final portal.";
    public float endLinger = 3f;



    public override void StartCutscene() {
        firstBlock = manager.generator.GetBlockNumber(0);
        secondBlock = manager.generator.GetBlockNumber(1);
        ladderBlock = manager.generator.GetLadder(0);
        machine = new StateMachine<LadderCutscene>(new PulloutFirstBlock(), this);
        
    }
    public override void UpdateCutScene() {
        machine.Update();
    }




    public class PulloutFirstBlock : State<LadderCutscene> {

        public override void Enter(StateMachine<LadderCutscene> obj) {
            obj.target.manager.ShowText(obj.target.ladder1);
        }


        public override void Update(StateMachine<LadderCutscene> obj) {
            if (obj.target.firstBlock.currentPullLevel > 1 && obj.target.secondBlock.currentPullLevel > 0) {
                obj.ChangeState(new JumpOnLadderAndPressF());
            }
        }

    }


    public class JumpOnLadderAndPressF : State<LadderCutscene> {

        public override void Enter(StateMachine<LadderCutscene> obj) {
            obj.target.manager.ShowText(obj.target.ladder2);
        }

        public override void Update(StateMachine<LadderCutscene> obj) {
            if (obj.target.manager.player.transform.position.y > 4) {
                obj.ChangeState(new SecondToEndScene());
            }
        }


    }

    public class SecondToEndScene : State<LadderCutscene> {
        public override void Enter(StateMachine<LadderCutscene> obj) {
            obj.target.manager.ShowText(obj.target.ladder3);
            time = -1;
        }

        float time = -1;

        public override void Update(StateMachine<LadderCutscene> obj) {
            if (time == -1) {
                if (!obj.target.manager.textBox.textMoving) {
                    time = Time.time + obj.target.endLinger;
                } 
            } else {
                if (time < Time.time) {
                    obj.ChangeState(new EndScene());
                }
            }
        }
    }



    public class EndScene : State<LadderCutscene> {
        public override void Enter(StateMachine<LadderCutscene> obj) {
            obj.target.manager.ShowText(obj.target.ladder4);
            time = -1;
        }

        float time = -1;

        public override void Update(StateMachine<LadderCutscene> obj) {
            if (time == -1) {
                if (!obj.target.manager.textBox.textMoving) {
                    time = Time.time + obj.target.endLinger;
                }
            } else {
                if (time < Time.time) {
                    obj.target.manager.EndCurrentCutscene();
                }
            }
        }
    }


}
