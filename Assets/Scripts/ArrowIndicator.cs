using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public Transform arrowTransform, bodyTransform;
    public Animator anim;

    private void Awake() {
        anim.Play("PushPullIdle");
        //SetPosition(BlockDirection.DOWN, BlockDirection.LEFT, new Vector3(2, 1, 2));
    }


    public void SetPosition(BlockDirection arrowDir, BlockDirection humanDir, Vector3 position, Block b) {
        gameObject.SetActive(true);
        anim.Play("PushPullIdle");
        arrowTransform.rotation = DirectionToEuler(arrowDir);
        bodyTransform.rotation = DirectionToEuler(humanDir);
        transform.SetParent(b.transform);
        transform.localPosition = position;

    }


    public Quaternion DirectionToEuler(BlockDirection d) {
        switch (d) {
            case BlockDirection.UP:   { return Quaternion.Euler(0, 0, 0); }
            case BlockDirection.DOWN: { return Quaternion.Euler(0, 180, 0); }
            case BlockDirection.LEFT: { return Quaternion.Euler(0, 270, 0); }
            case BlockDirection.RIGHT: { return Quaternion.Euler(0, 90, 0); }
        }
        return Quaternion.Euler(0, 0, 0);
    }

    internal void Show() {
        anim.Play("PushPullIdle");
        gameObject.SetActive(true);
    }

    public void HideCondition(bool v) {
        if (gameObject.activeInHierarchy == v) {
            gameObject.SetActive(!v);
            if (gameObject.activeInHierarchy) anim.Play("PushPullIdle");
        }
        
    }

    internal bool IsAttachedTo(Block block) {
        return transform.parent == block.transform;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
