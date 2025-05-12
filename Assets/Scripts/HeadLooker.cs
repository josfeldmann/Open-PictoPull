using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLooker : MonoBehaviour
{
   
    public Transform head;
    public float headSpeed = 180f;
    public float range = 10;

    Quaternion targetRotation;
    Quaternion defaultRot;
    private void Awake() {
        defaultRot = head.transform.rotation;
    }


    private void Update() {
        if (PlayerController.instance == null) return;
        if (Vector3.Distance(PlayerController.instance.npcLookAtSpot.transform.position, head.transform.position) < range) {
            targetRotation = Quaternion.LookRotation(-(PlayerController.instance.npcLookAtSpot.transform.position - head.transform.position), Vector3.up);
        } else {
            targetRotation = defaultRot;
        }

        if (head.rotation != targetRotation) head.rotation = Quaternion.RotateTowards(head.rotation, targetRotation, headSpeed * Time.deltaTime);
        
    }


}
