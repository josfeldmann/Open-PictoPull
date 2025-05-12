using Cinemachine;
using JetBrains.Annotations;
//using Sirenix.OdinInspector.Editor.Drawers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StoryCutScene : MonoBehaviour
{

    public string cutsceneid;
    [HideInInspector]
    public List<string> currentStringList;
    [HideInInspector]
    public int currentStringIndex;

    public void DisplaySpeechBox(List<string> s) {
        
        currentStringList = s;
       
        if (currentStringList.Count > 0) {
            currentStringIndex = 0;
            GameMasterManager.instance.cutsceneManager.ShowText(s[0]);
            if (currentStringList.Count > 1) {
                GameMasterManager.instance.cutsceneManager.interactButton.gameObject.SetActive(true);
            } else {
                GameMasterManager.instance.cutsceneManager.interactButton.gameObject.SetActive(true);
            }
        }
    }

    public void HideSpeechBox() {

    }

    public void SwitchToCameraAngle(CinemachineVirtualCamera cam) {
        cam.gameObject.SetActive(true);
        GameMasterManager.instance.cameraManager.ShowCustomCamera(cam);
    }


    public void SetToCameraAngle(CinemachineVirtualCamera cam, Transform t) {
        cam.transform.position = t.position;
        cam.transform.rotation = t.rotation;
    }

    public IEnumerator MoveCamera(CinemachineVirtualCamera cam, Transform t, float speed, float rotSpeed, float time) {

        float doneTime = Time.time + time;

        while ((cam.transform.position != t.position || transform.rotation != cam.transform.rotation) && Time.time <= doneTime) {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, t.position, speed * Time.deltaTime);
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, t.rotation, rotSpeed * Time.deltaTime);
            yield return null;
        }

        cam.transform.position = t.position;
        cam.transform.rotation = t.rotation;


    }

    public IEnumerator WaitTillSpeechIsDone() {



        while (currentStringIndex < currentStringList.Count) {
            GameMasterManager.instance.cutsceneManager.ShowText(currentStringList[currentStringIndex]);
            if (currentStringIndex == currentStringList.Count-1) {
                GameMasterManager.instance.cutsceneManager.interactButton.gameObject.SetActive(true);
            } else {
                GameMasterManager.instance.cutsceneManager.interactButton.gameObject.SetActive(true);
            }
            while (GameMasterManager.instance.cutsceneManager.textBox.textMoving) {
                if (GameMasterManager.instance.inputManger.interactDown) {
                    GameMasterManager.instance.cutsceneManager.CompleteText();
                }
                yield return null;
            }

            yield return null;
            while (!GameMasterManager.instance.inputManger.interactDown) {
                yield return null;
            }
            currentStringIndex++;


            yield return null;
            


        }
        GameMasterManager.instance.cutsceneManager.HideText();



    }




    public virtual IEnumerator CutScene() {



        yield return null;




    }

    public IEnumerator GetCutsceneFromCurrentLevel() {
        if (GameMasterManager.currentGameMode != GameMode.STORYLEVEL) yield break;
        HideCutSceneProps();
        GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
        GameMasterManager.instance.playerController.gameObject.SetActive(true);
        if (GameMasterManager.currentLevel == null) yield break;
        string cID = GameMasterManager.currentLevel.saveFile.cutsceneid;
        if (cID != null && cID.Length>0) {
          //  Debug.Log("dSJKJDADSLKLJ");
            CutScene c =GameMasterManager.instance.cutsceneManager.GetCutscene(cID);

            if (c!=null) {
                GameMasterManager.instance.cutsceneManager.ShowCutscene(c);
                while (GameMasterManager.instance.cutsceneManager.isCutscenePlaying) {
                    yield return null;
                }
            }
        }


    }



    public virtual void HideCutSceneProps() {

    }


}
