using System;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public Collider col;
    public Transform touchablePart;
    public GameObject crown;
    public Vector3 targetScale = Vector3.zero;
    public bool changingSize = false;
    public float scaleSpeed = 10f;
    public float rotateSpeed = 360f;
    public bool canPlayerInteract = false;
    public LayerMask mask;
    public Block attachedBlock;


    public void HideTouchablePart() {
        canPlayerInteract = false;
        if (touchablePart.transform.localScale != Vector3.zero) {
            targetScale = Vector3.zero;
            changingSize = true;
        }
    }


    private void Update() {
        crown.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        if (changingSize) {
            if (touchablePart.transform.localScale != targetScale) {
                touchablePart.transform.localScale = Vector3.MoveTowards(touchablePart.transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
            } else {
                changingSize = false;
            }
        }
    }

    internal void ShowTouchablePart() {
        canPlayerInteract = true;
        if (touchablePart.transform.localScale != Vector3.one) {
            targetScale = Vector3.one;
            changingSize = true;
        }
    }

    public List<GameObject> touching = new List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        touching.RemoveAll(t => t == null);
        if (other.gameObject.layer == Layers.Player && canPlayerInteract) {

            if (GameMasterManager.instance.generator.HasSubGoals() && !GameMasterManager.instance.generator.SubGoalCheck()) {
                return;
            }


            other.gameObject.GetComponent<PlayerController>().Win();
            return;
        }

        if (Layers.inLayer(other.gameObject.layer, mask) && !touching.Contains(other.gameObject)) {
            touching.Add(other.gameObject);
            HideTouchablePart();
        }
    }


    private void OnTriggerExit(Collider other) {
        touching.RemoveAll(t => t == null);
        if (Layers.inLayer(other.gameObject.layer, mask) && touching.Contains(other.gameObject)) {
            touching.Remove(other.gameObject);
            if (touching.Count == 0) ShowTouchablePart();
        }
    }

    internal void Check() {
        if (attachedBlock.currentPullLevel > 0 && touching.Count == 0) {
            ShowTouchablePart();
        } else if (attachedBlock.currentPullLevel == 0) {
            HideTouchablePart();
        }
    }

    
}







