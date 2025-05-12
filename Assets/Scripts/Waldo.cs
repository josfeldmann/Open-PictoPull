using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waldo : MonoBehaviour
{
    public GameObject beet;
    public float offset = 0.2f;

    private void Awake() {
        beet.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Break();
      //  Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == Layers.Player) {
         //   Debug.Break();
            beet.gameObject.SetActive(true);
            beet.transform.position = transform.position + new Vector3(0,offset,0);
            gameObject.SetActive(false);
        }
    }
}
