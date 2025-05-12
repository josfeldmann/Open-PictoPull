using System;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    [HideInInspector]public AudioSource source;
    public bool isMusic = false;
    [Range(0, 1)]
    public float volumeMultiplier = 1;

    private void Start() {
       if (source == null) source = GetComponent<AudioSource>();
       
        if (isMusic) {
           if (!AudioManager.musicSources.Contains(this)) AudioManager.musicSources.Add(this);
            SetVolume(AudioManager.musicVolume);
        } else {
            if (!AudioManager.sfxSources.Contains(this)) AudioManager.sfxSources.Add(this);
            SetVolume(AudioManager.sfxVolume);
        }
       
    }

    public void SetVolume(float val) {
        if (source == null) source = GetComponent<AudioSource>();
        source.volume = val * volumeMultiplier;
    }

    public void GetSource() {
        if (source == null) source = GetComponent<AudioSource>();
    }

    [ContextMenu("Play")]
    public void Play() {
        if (source != null)source.Play();
    }

    public void Stop() {
        if (source != null) source.Stop();
    }
} 
