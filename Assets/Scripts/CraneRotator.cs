using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneRotator : MonoBehaviour
{
    public Transform craneTransform;
    public Vector2 maxXRange = new Vector2(0, 90);
    public Vector2 maxZRange = new Vector2(0, 90);
    public Vector2 waitTimeRange = new Vector2(5, 10);
    public Vector2 speedRange = new Vector2(10, 30);


    public Quaternion targetRot;
    public Vector3 currentVect;
    float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        GetNewTarget();
    }

    public void GetNewTarget() {
        currentSpeed = Random.Range(speedRange.x, speedRange.y);
        targetRot = Quaternion.Euler( new Vector3(Random.Range(maxXRange.x, maxXRange.y), 0, Random.Range(maxZRange.x, maxZRange.y)));
    }

    float waitTime;

    // Update is called once per frame
    void Update()
    {
        //currentVect = craneTransform.localEulerAngles;
        if (waitTime > Time.time) return;
        if (craneTransform.localRotation != targetRot) {
            craneTransform.localRotation = Quaternion.RotateTowards(craneTransform.localRotation, targetRot , currentSpeed * Time.deltaTime);
        } else {
            GetNewTarget();
            waitTime = Time.time + Random.Range(waitTimeRange.x, waitTimeRange.y);
        }

    }
}
