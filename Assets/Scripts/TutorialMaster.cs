using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMaster : MonoBehaviour
{


    public StateMachine<TutorialMaster> controller;
    public PlayerController player;
    public TextDisplayer textDisplayer;
    public BlockLevelGenerator generator;

    public List<string> templateStrings;
    public List<string> actualStrings;
    public int currentIndex = 0;

    public List<ControlVariable> controlsToCheck = new List<ControlVariable>();
   

    private void Awake() {
       
    }

    public void SetTemplateStrings(List<string> s) {
        if (s == null || s.Count == 0) {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        templateStrings = new List<string>(s);
        SetActualStrings();
    }

    public void SetActualStrings() {
        actualStrings = new List<string>();
        for (int i = 0; i < templateStrings.Count; i++) {
            string s = templateStrings[i];


            foreach (ControlVariable c in controlsToCheck) {
                if (s.Contains(ButtonPromptManager.ControlVariableToString(c))) {
                    s = s.Replace(ButtonPromptManager.ControlVariableToString(c), ButtonString(ButtonPromptManager.GetSpriteTextIcon(c)));
                }
            }
            actualStrings.Add(s);

        }
        textDisplayer.SetText(actualStrings[0]);
    }

    public static string ButtonString(string s) {

        if (s == null || s.Length == 0) return s;

        return "<sprite name=\"" + s + "\">";
    }

    private void Update() {
       
    }


}

public class DisplayText : State<TutorialMaster> {


    public override void Update(StateMachine<TutorialMaster> obj) {
        
    }


    public override void AdaptToChangedControlScheme(ButtonPromptManager manager, StateMachine<TutorialMaster> obj) {
        
    }


}

public class WASDToMove : State<TutorialMaster> {


    public float time;

    public override void Enter(StateMachine<TutorialMaster> obj) {
        obj.target.textDisplayer.SetText("WASD TO MOVE\nSPACE TO JUMP\n");
        time = Time.time + 5;
    }


    public override void Update(StateMachine<TutorialMaster> obj) {
        if (Time.time > time) {
            obj.target.controller.ChangeState(new ShiftTutorialState());
        }
        if (obj.target.player.currentlyGrabbing) {
            obj.ChangeState(new PlayerGrabbingTutorialState());
        }
    }



}



public class ShiftTutorialState : State<TutorialMaster> {

    public float time;

    public override void Enter(StateMachine<TutorialMaster> obj) {
        obj.target.textDisplayer.SetText("Face a block and press shift to grab");
        time = Time.time + 5;
    }


    public override void Update(StateMachine<TutorialMaster> obj) {
        if (Time.time > time) {
            obj.target.controller.ChangeState(new WASDToMove());
        }
        if (obj.target.player.currentlyGrabbing) {
            obj.ChangeState(new PlayerGrabbingTutorialState());
        }
        }
}



public class PlayerGrabbingTutorialState : State<TutorialMaster> {

   

    public override void Enter(StateMachine<TutorialMaster> obj) {
        obj.target.textDisplayer.SetText("Press W/S to push back and forth");
        
    }


    public override void Update(StateMachine<TutorialMaster> obj) {
        if (obj.target.player.currentlyGrabbing) {

        } else {
            if (obj.target.generator.firstBlockPulledOutYet) {
                obj.ChangeState(new WinTutorialState());
            } else {
                obj.ChangeState(new ShiftTutorialState());
            }
            
        }
    }

}


public class WinTutorialState : State<TutorialMaster> {



    public override void Enter(StateMachine<TutorialMaster> obj) {
        obj.target.textDisplayer.SetText("Pull out the block with the crown sticker on it and grab the crown to win!");

    }


    public override void Update(StateMachine<TutorialMaster> obj) {
       if (!obj.target.generator.firstBlockPulledOutYet) {
            obj.ChangeState(new WASDToMove());
        }
    }

}