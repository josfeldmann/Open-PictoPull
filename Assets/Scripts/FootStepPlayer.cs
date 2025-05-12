using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepPlayer : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();
    public AudioSource source;
    public float minCooldown = 0.25f;
    public Animator anim;
    float time = 0;
    int index = 0;


    public void PlaySound() {

        if (Time.time < time) return;
        if (anim.IsInTransition(0)) return;
        if ( PlayerController.instance != null && !PlayerController.instance.feetCheck.grounded) return;
        source.clip = clips[index];
        index++;
        if (index >= clips.Count) index = 0;
        source.Play();
        time = Time.time + minCooldown;

    }

}
