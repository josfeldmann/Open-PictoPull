using Cinemachine;
using System.Collections;
using UnityEngine;

public class VillageCutScene : StoryCutScene {

    public Animator tvDummy;
    public NPCLookAtPlayer lookAt;
    public CinemachineVirtualCamera cutCam;
    public GameObject capsuleDummy;
    public Animator PlayerDummy;
    public Transform capsuleStartSpot, capsuleEndSpot, playerSpawnSpotRelativeToCapsule;
    public float capsuleFallSpeed;
    public Vector3 fullSizeScale = new Vector3(0.33f, 0.33f, 0.33f);
    public Transform cameraStartSpot, cameraEndSpot, cameraMidSpot;
    public override void HideCutSceneProps() {
        tvDummy.gameObject.SetActive(false);
        capsuleDummy.SetActive(true);
        capsuleDummy.transform.position = capsuleEndSpot.transform.position;
        PlayerDummy.gameObject.SetActive(false);
        cutCam.gameObject.SetActive(false);
    }

    public override IEnumerator CutScene() {

        GameMasterManager.instance.generator.player.gameObject.SetActive(false);
        GameMasterManager.instance.generator.tutorialNPC.gameObject.SetActive(false);

        lookAt.enabled = false;



        SwitchToCameraAngle(cutCam);
        tvDummy.transform.forward = new Vector3(0, 0, -1);
        tvDummy.gameObject.SetActive(true);
        capsuleDummy.gameObject.SetActive(true);
        PlayerDummy.gameObject.SetActive(false);
        capsuleDummy.transform.position = capsuleStartSpot.position;


        yield return new WaitForSeconds(2);
        lookAt.enabled = true;

        Vector3 lookAtVector = capsuleEndSpot.transform.position - tvDummy.transform.position;
        lookAtVector.y = 0;
        lookAtVector.Normalize();
        lookAtVector = -lookAtVector;

        Quaternion q = Quaternion.LookRotation(lookAtVector, Vector3.up);


        while (capsuleDummy.transform.position != capsuleEndSpot.position || tvDummy.transform.rotation == q) {
            capsuleDummy.transform.position = Vector3.MoveTowards(capsuleDummy.transform.position, capsuleEndSpot.position, capsuleFallSpeed * Time.deltaTime);
            tvDummy.transform.rotation = Quaternion.RotateTowards(tvDummy.transform.rotation, q, 180f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        PlayerDummy.transform.position = playerSpawnSpotRelativeToCapsule.position;
        PlayerDummy.transform.localScale = Vector3.zero;
        PlayerDummy.gameObject.SetActive(true);

        while (PlayerDummy.transform.localScale != fullSizeScale) {
            PlayerDummy.transform.localScale = Vector3.MoveTowards(PlayerDummy.transform.localScale, fullSizeScale, Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        DisplaySpeechBox(new System.Collections.Generic.List<string>() { "Ah PictoCorp finally sent someone with arms!"});
        yield return WaitTillSpeechIsDone();

        yield return MoveCamera(cutCam, cameraMidSpot, 10f, 180f,2f);

        DisplaySpeechBox(new System.Collections.Generic.List<string>() { "Some prankster has put a number of our PictoCores in these block contraptions.",
                                                                         "We've dubbed these puzzles \"PictoPulls\" and we need someone handy to retrieve the PictoCores stuck in them." });
                                                                   
        yield return WaitTillSpeechIsDone();

        yield return MoveCamera(cutCam, cameraStartSpot, 10f, 180f, 1f);
        

        DisplaySpeechBox(new System.Collections.Generic.List<string>() { "Lets get to work, I'll start by showing you how to solve a PictoPull"});
        yield return WaitTillSpeechIsDone();
        yield return new WaitForSeconds(0.5f);


        lookAt.enabled = false;

        yield return GetCutsceneFromCurrentLevel();
    }
}
