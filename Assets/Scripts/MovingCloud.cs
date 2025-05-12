using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    public float speed = 8f;
    public const float loopWidth = 40;
    public bool movePlayer;

    


    private void FixedUpdate() {
        if (GameMasterManager.instance.playerController.IsPaused()) return;
        transform.position += new Vector3(speed, 0, 0) * Time.fixedDeltaTime;
        if (transform.position.x > 40) {
            transform.position = new Vector3(-loopWidth, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == Layers.Player && GameMasterManager.instance.playerController.IsInGameplay()) {
         //   GameMasterManager.instance.playerController.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == Layers.Player && GameMasterManager.instance.playerController.transform.parent == transform) {
           //GameMasterManager.instance.playerController.ReturnToParent();
        }
    }

    private void OnTriggerStay(Collider other) {

        if (GameMasterManager.instance.playerController.IsPaused()) return;
        if (other.gameObject.layer == Layers.Player && GameMasterManager.instance.playerController.IsInGameplay()) GameMasterManager.instance.playerController.transform.position += new Vector3(speed, 0, 0) * Time.fixedDeltaTime;
    }

}
