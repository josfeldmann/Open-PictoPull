using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class StringMessage {

    [TextArea]
    public string message;
    public float lingerTime = 3f;
    

}

public class StringCutscene : CutScene
{
    public bool endWhenDone = true;

    public List<StringMessage> messages = new List<StringMessage>();

    bool isStopped = false;
    float nextTime = 0;
    int index = 0;


    public override void StartCutscene() {
        manager.ShowText(messages[0].message);
        isStopped = false;
        index = 0;
        manager.interactButton.gameObject.SetActive(true);
        
    }

    public override void UpdateCutScene() {
        

        
        if (!isStopped && !manager.textBox.textMoving) {
            isStopped = true;
            nextTime = Time.time + messages[index].lingerTime;
        } else if (!isStopped && manager.textBox.textMoving && GameMasterManager.instance.inputManger.interactDown) {
            manager.CompleteText();
        }

        if (isStopped && GameMasterManager.instance.inputManger.interactDown) {
            index++;
            if (index >= messages.Count) {
                if (endWhenDone) manager.EndCurrentCutscene();
                else manager.HideText();
            } else {
                manager.ShowText(messages[index].message);
                isStopped = false;
            }
        }
    }

    public override bool CanReplayCutscene() {
        return true;
    }

    public void SetMessages(List<string> m) {
        messages = new List<StringMessage>();
        foreach (string ss in m) {
            if (ss != null) {
                StringMessage sm = new StringMessage();
                sm.message = ss;
                messages.Add(sm);
            }
        }
    }
}
