using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{

    public Vector3 Euler;
    // Start is called before the first frame update
    private void Update() {
        transform.rotation = Quaternion.Euler(Euler);
    }
}
