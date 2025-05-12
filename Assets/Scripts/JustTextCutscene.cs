using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustTextCutscene : StoryCutScene {

   
    public CinemachineVirtualCamera virtualCam;
    public List<string> text;


    public float timeBefore = 2;
    public float timeAfter = 2;

    public List<GameObject> toShow = new List<GameObject>();
    public bool SkipLevelScene = false;



    public override void HideCutSceneProps() {
       foreach (GameObject g in toShow) {
            g.gameObject.SetActive(false);
        }
        virtualCam.gameObject.SetActive(false);

    }

    private void Update() {
        
    }

    public override IEnumerator CutScene() {
        GameMasterManager.instance.generator.player.gameObject.SetActive(false);
        
        GameMasterManager.instance.generator.tutorialNPC.gameObject.SetActive(false);
        virtualCam.gameObject.SetActive(true);

        foreach (GameObject g in toShow) {
            g.gameObject.SetActive(true);
        }

        SwitchToCameraAngle(virtualCam);
        yield return new WaitForSeconds(timeBefore);
        DisplaySpeechBox(text);
        yield return WaitTillSpeechIsDone();
        yield return new WaitForSeconds(timeAfter);
        if (!SkipLevelScene)
        yield return GetCutsceneFromCurrentLevel();
    }


}
