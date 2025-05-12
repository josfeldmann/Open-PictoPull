using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InteractionPromptText : MonoBehaviour
{
    public TextMeshPro text;
    public Transform playerTransform;
    public string interactText;
    public string launchText;
    public string warpText = "[Interact]";
    public string storyLevelSelectText = "[Interact] Change Area";
    public string talkText = "[Interact] Talk";
    public string monitorText = "[Interact] Monitor";
    public Vector3 position = new Vector3(0,0,-2f);

    public void SetWarpText() {
        string s = "";
        s = interactText.Replace(ButtonPromptManager.ControlVariableToString(ControlVariable.INTERACT), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(ControlVariable.INTERACT)));
        text.SetText(s);
        SetPosition();
        DisableIfUIHidden();
    }

    public void DisableIfUIHidden() {
        if(GameMasterManager.gameSaveFile.hidegameplayUI) {
            gameObject.SetActive(false);
        }
    }


    private void Start() {
        //SetText();
    }

    private void Update() {
        SetPosition();
    }

    public void SetPosition() {
        transform.position = playerTransform.position + position;
    }

    public void SetStoryLevelSelectText() {
        string s = "";
        s = storyLevelSelectText.Replace(ButtonPromptManager.ControlVariableToString(ControlVariable.INTERACT), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(ControlVariable.INTERACT)));
        text.SetText(s);
        SetPosition();
        DisableIfUIHidden();
    }

    public void SetTalkText() {
        string s = "";
        s = talkText.Replace(ButtonPromptManager.ControlVariableToString(ControlVariable.INTERACT), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(ControlVariable.INTERACT)));
        text.SetText(s);
        SetPosition();
        DisableIfUIHidden();
    }

    public void SetCannonText() {
        string s = "";
        s = launchText.Replace(ButtonPromptManager.ControlVariableToString(ControlVariable.INTERACT), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(ControlVariable.INTERACT)));
        text.SetText(s);
        SetPosition();
        DisableIfUIHidden();
    }

    public void SetMonitorText() {
        string s = "";
        s = monitorText.Replace(ButtonPromptManager.ControlVariableToString(ControlVariable.INTERACT), TutorialMaster.ButtonString(ButtonPromptManager.GetSpriteTextIcon(ControlVariable.INTERACT)));
        text.SetText(s);
        SetPosition();
        DisableIfUIHidden();
    }
}
