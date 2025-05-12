using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public Vector3 rotAmount = new Vector3(0, 360, 0);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotAmount * Time.deltaTime);
    }
}

