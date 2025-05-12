using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetButton : MonoBehaviour
{
    public Transform downPos, upPos;
    public Transform buttonUpMesh;
    public float speed = 5f;
    public LayerMask layerMask;
    public Transform targetTransform;
    public List<Transform> touching = new List<Transform>();
    public UnityEvent onPress;
    public Collider blocker;
    private void Update() {

        if (blocker.transform.position != transform.position) blocker.transform.position = transform.position;
        if (PlayerController.instance.feetCheck.grounded != blocker.gameObject.activeInHierarchy) {
            if (touching.Count > 0) {
                blocker.gameObject.SetActive(false);
            } else {
                blocker.gameObject.SetActive(PlayerController.instance.feetCheck.grounded);
            }
        }


         if (targetTransform != null && buttonUpMesh.position != targetTransform.position) {
            buttonUpMesh.transform.position = Vector3.MoveTowards(buttonUpMesh.transform.position, targetTransform.position, speed * Time.deltaTime);
         }
    }


    public void Setup() {
        targetTransform = upPos;
    }

    public void AddResetButtonStat() {
        GameMasterManager.gameSaveFile.trackerSaveFile.AddResets(1);
    }


    private void OnTriggerEnter(Collider other) {
        
        if (Layers.inLayer(other.gameObject.layer, layerMask)) {
            if (PlayerController.instance.feetCheck.grounded) return;
            if (!touching.Contains(other.transform)) {
                touching.Add(other.transform);
            }
            if (touching.Count > 0) {
                targetTransform = downPos;
                onPress.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, layerMask)) {
            if (touching.Contains(other.transform)) {
                touching.Remove(other.transform);
            }
            if (touching.Count == 0) {
                targetTransform = upPos;
            }
        }
    }



}
