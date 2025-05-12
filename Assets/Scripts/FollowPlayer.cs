using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform toFollow;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {


        transform.position = toFollow.position + Offset;
        transform.rotation = Quaternion.identity;
    }
}

