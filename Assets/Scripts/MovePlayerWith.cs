using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerWith : MonoBehaviour
{
    Vector3 lastPosition;
    bool tracking = false;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == Layers.Player) {
            tracking = true;
            lastPosition = transform.position;
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == Layers.Player) {
            tracking = false;
        }
    }


    private void FixedUpdate() {
        if (!tracking) return;
        Vector3 difference = transform.position - lastPosition;
        lastPosition = transform.position;
        GameMasterManager.instance.playerController.transform.position += difference;
    }

    

}
