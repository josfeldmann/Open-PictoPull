using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindEffect : MonoBehaviour
{
    public List<RewindBar> rewindBars = new List<RewindBar>();
    public List<Transform> targets = new List<Transform>();

    public void StartShowing() {
        gameObject.SetActive(true);
        foreach (RewindBar r in rewindBars) {
            r.StartShowing();
        }
    }

    public void Hide() {
        foreach (RewindBar r in rewindBars) {
            r.StopShowing();
        }
    }

    public void HideInstantly() {
        gameObject.SetActive(false);
    }


    public Transform GetTarget(Transform prevTarget) {
        int idx = UnityEngine.Random.Range(0, targets.Count);
        
        Transform t = targets[idx];
        if (t == prevTarget) idx++;
        if (idx >= targets.Count) idx = 0;
       // targets.RemoveAt(idx);
        return t;
    }

    public void ReAddTarget(Transform toFree) {
        if (!targets.Contains(toFree)) targets.Add(toFree);
    }
}
