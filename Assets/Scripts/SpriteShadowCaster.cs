using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadowCaster : MonoBehaviour
{
    public LayerMask castLayer;
    public Transform shadow;
    public RaycastHit shadowHit;
    public float shadowOffset = 0.01f;
    public Transform followTransform;
    public Transform RunTimeParent;

    private void Awake() {
        transform.SetParent(RunTimeParent);
    }

    private void Update() {
        transform.position = followTransform.position;
        if (Physics.Raycast(transform.position, Vector3.down, out shadowHit, 100, castLayer)) {
            shadow.transform.localPosition = new Vector3(0, shadowHit.point.y - transform.position.y + shadowOffset, 0);
        }
    }


}
