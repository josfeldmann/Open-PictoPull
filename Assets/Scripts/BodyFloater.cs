using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFloater : MonoBehaviour
{
    public Vector3 startPosition, target;
    public float floatDistance = 0.25f;
    public float floatSpeed = 0.25f;
    bool floating = true, goingUp = false;

    private void Awake() {
        target = startPosition;
    }

    public void StartFloating() {
        if (!floating) {
            floating = true;
            goingUp = Random.Range(0f, 1f) > 0.5f;
            SetFloatTarget();
        } else {

        }
    }

    public void SetFloatTarget() {
        if (goingUp) target = startPosition + new Vector3(0, floatDistance, 0);
        else target = startPosition - new Vector3(0, floatDistance, 0);
    }

    public void StopFloating() {
        floating = false;
        target = startPosition;
    }

    private void Update() {
        if (transform.localPosition != target) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, floatSpeed * Time.deltaTime);
        } else if (floating) {
            goingUp = !goingUp;
            SetFloatTarget();
        }
        
    }

}
