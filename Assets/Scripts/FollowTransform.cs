using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public LayerMask targetLayer;
    public Transform runtimeTransform;
    public Transform followTransform;
    public float groundedDistance = 0.75f;
    public float maxAirDistance;
    public Transform castPoint;
    public Projector projector;
    public PlayerController controller;
    RaycastHit hit;

    private void Awake() {
        transform.SetParent(runtimeTransform);
    }

    private void Update() {
        transform.position = followTransform.position;
        if (GameMasterManager.inGameArea && controller.feetCheck.grounded) {
           // print("Ground " + Time.time);
            projector.farClipPlane = groundedDistance;
        } else {
            projector.farClipPlane = maxAirDistance;
            if (Physics.Raycast(castPoint.transform.position, Vector2.down, out hit, maxAirDistance, targetLayer)) {
             //   print(hit.collider.gameObject.name + Time.time.ToString());
                projector.farClipPlane = (transform.position.y - hit.point.y) + groundedDistance;
            }
        }

        if (followTransform.gameObject.activeInHierarchy) {
            projector.enabled = true;
        } else {
            projector.enabled = false;
        }

    }


}
