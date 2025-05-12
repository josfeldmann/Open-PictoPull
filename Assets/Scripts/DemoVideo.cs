using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class DemoVideo : MonoBehaviour
{
    public VideoPlayer player;
    public string filename;
    public RawImage rawImage;
    public bool playOnAwake = false;
    public InputManager inputManager;
    public GameObject textPrompt;


    [Header("Idle Logic")]
    public bool showAfterTimeIdle;
    public float idleTime = 5f;
    float idleTimer;


    public bool canVideoBePlayed;
    float lastPlayedTime;
    private void Awake() {
        try {
            string file = Application.dataPath + "/" + filename;
            if (!File.Exists(file)) {
                Debug.LogError("File " + file + " does not exist!");
                canVideoBePlayed = false;
                HideVideo();
                gameObject.SetActive(false);
            }
            canVideoBePlayed = true;
            player.url = file;
            if (playOnAwake) player.Play();
            else HideVideo();
        } catch (Exception e) {
            gameObject.SetActive(false);
        }
    }

    float lastHideTime;

    public void HideVideo() {
        rawImage.enabled = false;
        player.enabled = false;
        lastHideTime = Time.time;
        textPrompt.gameObject.SetActive(false);
    }

    public void PlayVideo() {
        rawImage.enabled = true;
        player.enabled = true;
        player.Play();
        lastPlayedTime = Time.time + 0.5f;

        textPrompt.gameObject.SetActive(true);
    }


    private void Update() {
        if (Keyboard.current != null && Keyboard.current.f7Key.wasPressedThisFrame && canVideoBePlayed) {
            if (canVideoBePlayed) PlayVideo();
        }
        if (player.enabled && inputManager.inputDetectedThisFrame && Time.time > lastPlayedTime) {
            HideVideo();
        }

        if (showAfterTimeIdle && player.enabled == false) {
            if (inputManager.inputDetectedThisFrame) {
                lastHideTime = Time.time;
            } else {
                if ((lastHideTime + idleTime) < Time.time) {
                    PlayVideo();
                }
            }
        }
    }

}
