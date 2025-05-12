using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{

    public GameObject screen;
    public Vector3 lookUpOffset = new Vector3(0, 0.5f, 0);
    public Transform lookAt;



    private void Update() {

                 

        Vector3 lookPoint = lookAt.position + lookUpOffset;

        Vector3 diff = transform.position - lookPoint;
        diff.y = 0;

        Vector3 screenLook = screen.transform.position - lookPoint;
        transform.forward = diff;

        screen.transform.forward = screenLook;

       


    }
}
