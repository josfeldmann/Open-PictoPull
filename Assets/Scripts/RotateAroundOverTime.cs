using UnityEngine;

public class RotateAroundOverTime : MonoBehaviour {
    public float rotAmount = 30;
    public Vector3 rotaxis = new Vector3(0, 1, 0);
    public Transform point;
    
    void Update() {
        transform.RotateAround(point.transform.position, rotaxis, rotAmount * Time.deltaTime);
    }
}

