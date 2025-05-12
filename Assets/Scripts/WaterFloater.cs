using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloater : MonoBehaviour
{

    public float distance = 1f;
    public float speed = 2f;

    Vector3 startPosition;
    Vector3 target;

    private void Awake() {
        startPosition = transform.position;
        target = startPosition + (Vector3.down * distance);
    }

    private void Update() {
        if (target != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        } else {
            if (target == (startPosition + (Vector3.up *  distance))) {
                target = startPosition + (Vector3.down * distance);
            } else {
                target = startPosition + (Vector3.up * distance);
            }
        }
    }

}
