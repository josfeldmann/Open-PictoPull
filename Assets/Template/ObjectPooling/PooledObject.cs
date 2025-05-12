using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public IObjectPooler pooler;


    public void ReturnToPool() {
        pooler.ReturnPooledObject(this);
    }

    internal void ReturnToPoolInXSeconds(int v) {
        StartCoroutine(ReturnPoolTimerNumerator(v));
    }

    private IEnumerator ReturnPoolTimerNumerator(int v) {
        yield return new WaitForSeconds(v);
        ReturnToPool();
    }
}
