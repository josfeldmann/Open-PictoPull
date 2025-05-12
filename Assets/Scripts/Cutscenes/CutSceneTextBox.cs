using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutSceneTextBox : MonoBehaviour {

    public int soundEveryXCharacters = 5;
    public int stopSoundPrior = 20;
    public TextMeshProUGUI text;
    public float characterRevealinterval = 0.05f;
    float time;
    public bool textMoving;

    public List<AudioPlayer> audioSources = new List<AudioPlayer>();

    int index;
    private void Awake() {
        index = Random.Range(0, audioSources.Count);
    }


    public void SetText(string s) {

        

        text.text = GameMasterManager.ReplacePrompts(s);
        text.maxVisibleCharacters = 0;
        textMoving = true;
        nextchartime = Time.time;
        index = Random.Range(0, audioSources.Count);
        audioSources[index].Play();
        cIndex = 0;
        stopCharacter = text.text.Length - stopSoundPrior;
    }

    float nextchartime = 0;
    int cIndex = 0;
    int stopCharacter = 0;

    private void Update() {
         if (textMoving) {
            if (nextchartime <= Time.time) { 
                text.maxVisibleCharacters += 1;
                cIndex++;
                if (cIndex >= soundEveryXCharacters && text.maxVisibleCharacters < stopCharacter) {
                    index += Random.Range(1, audioSources.Count);
                    index %= audioSources.Count;
                    audioSources[index].Play();
                    cIndex = 0;
                }
               
                if (text.maxVisibleCharacters >= text.text.Length) {
                    textMoving = false;
                } else {
                    nextchartime = Time.time + characterRevealinterval;
                }
            }
        }
    }

    public void CompleteText() {
        text.maxVisibleCharacters = text.text.Length;
        textMoving = false;
    }
}

