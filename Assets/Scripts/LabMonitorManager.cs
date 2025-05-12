using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LabMonitorManager : MonoBehaviour {

    public List<LabMonitor> monitors = new List<LabMonitor>();
    public List<Sprite> sprites;
    public float transitionSpeed = 5;
    public float PuzzleChangeTime = 60;
    public List<SpriteRenderer> keySprites = new List<SpriteRenderer>();
    public Transform beetTransform;
    public bool complete = false;
    public Sprite checkMark;
    
    public float nextChangeTime;

    private void Awake() {
        complete = false;
         foreach (LabMonitor l in monitors) {
            l.manager = this;
            l.spriteRenderer.enabled = false;
         }
        beetTransform.gameObject.SetActive(false);
        ChangeToNewColor();
        nextChangeTime = Time.time + PuzzleChangeTime;
    }


    private void Update() {
       // if (Time.time > nextChangeTime && !complete) {
          //  ChangeToNewColor();
       // }
    }

    public void ChangeToNewColor() {
        nextChangeTime = Time.time + PuzzleChangeTime;
        sprites = new List<Sprite>( sprites.Shuffle());
        monitors = new List<LabMonitor> (monitors.Shuffle());
        
        for (int i = 0; i < monitors.Count; i++ ) {
            monitors[i].manager = this;
            monitors[i].SetMonitor(sprites[i], i);
            
        }

        for (int i = 0; i < keySprites.Count; i++) {
            keySprites[i].sprite = sprites[i];
        }
    }

    public void Click(LabMonitor l) {
        Sprite s = l.spriteRenderer.sprite;
        if (l.spriteRenderer.sprite == checkMark) {
            return;
        }
        bool goodCheck = false;
        int cnt = 0;
        foreach (SpriteRenderer ss in keySprites) {
            if (ss.sprite == checkMark) {
                cnt++;
            } else if (ss.sprite == l.spriteRenderer.sprite) {
                goodCheck = true;
                ss.sprite = checkMark;
                cnt++;
                l.boxCOllider.enabled = false;
                l.spriteRenderer.sprite = checkMark;
                if (PlayerController.monitors.Contains(l)) PlayerController.instance.RemoveMonitor(l);
            }
        }

        if (goodCheck) {
            if (cnt == 4) {
                beetTransform.gameObject.SetActive(true);
            }
        } else {
            ChangeToNewColor();
        }


    }









}

