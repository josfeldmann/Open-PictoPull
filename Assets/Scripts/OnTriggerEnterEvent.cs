using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{

    public LayerMask layermask;
    public UnityEvent<Collider> colliderEnterTriggerEvent;
    public UnityEvent<Collider> colliderExitTriggerEvent;
    
    private void OnTriggerEnter(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, layermask)) {
          
            colliderEnterTriggerEvent.Invoke(other);
        }    
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, layermask)) {
             colliderExitTriggerEvent.Invoke(other);
        }
    }

    public void ShowStoryLevelSelectPrompt() {
        PlayerController.instance.ShowSelectectStoryLevelText();
    }

    public void HideStoryLevelSelectPrompt() {
        PlayerController.instance.HideStorySelectText();
    }

    public void ShowTalkPrompt(NPCHuman human) {
        PlayerController.instance.ShowTalkPrompt(human);
    }

    public void HideTalkPrompt() {
        PlayerController.instance.HideTalkPrompt();
    }


}
