using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetInWorldObject : MonoBehaviour
{
    public BoolAchievementObject beetAchievement;

    public bool shrinking = false;
    public float shrinkTime = 1f;

    private void Start() {
        
        if (beetAchievement == null) {
            gameObject.SetActive(false);
            return;
        }

        #if UNITY_SWITCH
        gameObject.SetActive(false);
        return;
        #endif
        
        if (GameMasterManager.instance == null || beetAchievement.IsFulfilled(GameMasterManager.instance.tracker))
        {
            gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (shrinking) return;
        if (other.gameObject.layer == Layers.Player) {
            beetAchievement.CompleteAchievement();
            StartCoroutine(ShrinkNum());
        }
    }

    public IEnumerator ShrinkNum() {

        float speed = transform.localScale.magnitude;

        while (transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, speed * Time.deltaTime / shrinkTime);
            yield return null;
        }

        gameObject.SetActive(false);


    }





}
