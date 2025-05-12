using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour {

    public bool isCutscenePlaying;
    public CutScene currentCutscene;
    public List<CutScene> registeredScenes = new List<CutScene>();
    public BlockLevelGenerator generator;
    public CutSceneTextBox textBox;
    public InputManager inputManager;
    public PlayerController player;
    public GameObject resetArrow;
    public ButtonImagePrompt interactButton;

    public void ShowCutscene(string id) {
        CutScene c = GetCutscene(id);
        if (c == null) {
            HideCutsceneManager();
            GameMasterManager.instance.hintButton.gameObject.SetActive(true);
        } else {
            Debug.Log("Show Cutscene");
            StartCutscene(c);
            generator.tutorialNPC.gameObject.SetActive(true);
        }
    }

    public void HideCutsceneManager() {
        isCutscenePlaying = false;
        gameObject.SetActive(false);
        textBox.gameObject.SetActive(false);
        resetArrow.SetActive(false);
        interactButton.gameObject.SetActive(false);
    }


    public CutScene GetCutscene(string id) {
        foreach (CutScene s in registeredScenes) {
            if (s != null) {
                if (id == s.key) {
                    s.manager = this;
                    return s;
                }
            }
        }
        Debug.LogError("Could not find cutscene with ID: " + id);
        return null;
    }

    public void HideText() {
        textBox.gameObject.SetActive(false);
    }


    internal void ShowCutscene(CutScene currentCutscene) {
        StartCutscene(currentCutscene);
    }

    private void StartCutscene(CutScene c) {
        gameObject.SetActive(true);
        GameMasterManager.instance.hintButton.gameObject.SetActive(false);
        interactButton.gameObject.SetActive(false);
        foreach (CutScene cc in registeredScenes) {

            if (cc != c) {
                cc.gameObject.SetActive(false);
            }

        }
        c.gameObject.SetActive(true);
        c.StopAllCoroutines();
        if (isCutscenePlaying) {
            if (currentCutscene) currentCutscene.EndCutscene();
        }
        isCutscenePlaying = true;
        currentCutscene = c;
        c.StartCutscene();
    }

    private void Update() {
        if (isCutscenePlaying) {
            if (currentCutscene != null) {
                currentCutscene.UpdateCutScene();
            } else isCutscenePlaying = false;
        }
    }

    public void ShowText(string s) {
        Debug.Log("Show Text " + s);
        textBox.gameObject.SetActive(true);
        textBox.SetText(s);
        
        
    }

    public void EndCurrentCutscene() {
        if (currentCutscene) {
            currentCutscene.EndCutscene();
            if (currentCutscene.CanReplayCutscene()) {
                GameMasterManager.instance.hintButton.gameObject.SetActive(true);
            } else {
                GameMasterManager.instance.hintButton.gameObject.SetActive(false);
            }
        }
        HideCutsceneManager();
           


    }

    public void CompleteText() {
        textBox.CompleteText();
    }

}


public class CutScene : MonoBehaviour {

    [HideInInspector]public CutSceneManager manager;
    public string key;

    public virtual void StartCutscene() {

    }



    public virtual void UpdateCutScene() {

    }



    public virtual void EndCutscene() {

    }

    public virtual bool CanReplayCutscene() {
        return false;
    }


}

