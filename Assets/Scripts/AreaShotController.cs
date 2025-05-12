using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AreaShotController : MonoBehaviour
{
    public Transform cameraRot;
    public float camSpeed;
    public float timeInBetweenTransitions = 1;

    public List<Transform> transitions;


    public void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !spinning) {
            StartCoroutine(CameraSpin());
        }
    }

    bool spinning = false;

    IEnumerator CameraSpin() {
        spinning = true;
        int index = 0;
        foreach (Transform t in transitions) {
            t.gameObject.SetActive(false);
        }
        cameraRot.rotation = Quaternion.identity;
        transitions[0].gameObject.SetActive(true);
        float timer = 0;
        while (index < transitions.Count) {

            cameraRot.transform.Rotate(0, camSpeed * Time.deltaTime, 0);


            if (timer < timeInBetweenTransitions) {

            } else {
                timer = 0;
                transitions[index].gameObject.SetActive(false);
                index++;
                if (index < transitions.Count) {
                    transitions[index].gameObject.SetActive(true);
                } else {
                    transitions[0].gameObject.SetActive(true);
                }
            }

            yield return null;
            timer += Time.deltaTime;

        }
        spinning = false;



    }


}
