using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewindBar : MonoBehaviour
{
    public RewindEffect rewindEffect;
    public Vector2 timeToFadeInRange = new Vector2(0.5f, 1);
    public Vector2 speedRange = new Vector2(400,700);
    public float currentSpeed = 0;
    public float currentFadeInAmount = 0;
    public Image image;
    public Transform currentTargetTransform;

    float targetAlpha;

    private void Update() {

        if (currentTargetTransform == null) {
            SetTarget();
        }

        if (currentTargetTransform.position != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, currentTargetTransform.position, currentSpeed * Time.deltaTime);
        } else {
            SetTarget();
        }

        if (image.color.a != targetAlpha) {
            SetAlpha(Mathf.MoveTowards(image.color.a, targetAlpha, currentFadeInAmount * Time.deltaTime));
            if (image.color.a == targetAlpha && targetAlpha == 0) {
                gameObject.SetActive(false);
            }
        }
    }


    private void OnDisable() {
        if (currentTargetTransform != null) {
            rewindEffect.ReAddTarget(currentTargetTransform);
            currentTargetTransform = null;
        }
    }

    public void StartShowing() {
        StopAllCoroutines();
        SetAlpha(0);
        SetTarget();
        gameObject.SetActive(true);
        currentFadeInAmount = Random.Range(timeToFadeInRange.x, timeToFadeInRange.y);
        targetAlpha = 1;
    }

    public void SetAlpha(float f) {
        image.color = new Color(image.color.r, image.color.g, image.color.b, f);
    }



    





    public void SetTarget() {
        currentSpeed = Random.Range(speedRange.x, speedRange.y);
        if (currentSpeed <= 1) currentSpeed = 500;
        Transform toFree = currentTargetTransform;
        currentTargetTransform = rewindEffect.GetTarget(currentTargetTransform);
        if (toFree != null) rewindEffect.ReAddTarget(toFree);
    }

    public void StopShowing() {
        currentFadeInAmount = timeToFadeInRange.y * 2;
        targetAlpha = 0;
    }


}
