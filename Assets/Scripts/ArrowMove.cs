using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float amt = 0.1f;
    public float speed = 1;
    public float target;

    private void OnEnable() {
        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        target = amt;
    }

    public void SetDistance() {
        transform.localPosition = new Vector3(transform.localPosition.x,  Mathf.MoveTowards(transform.localPosition.y, target, speed * Time.deltaTime), transform.localPosition.z);
        if (transform.localPosition.y == target) {
            if (target == amt) target = -amt;
            else target = amt;
        }
    }

    private void Update() {
        SetDistance();
    }


}
