using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorIcon : MonoBehaviour
{

    public Image image;
    public Image subImage;
    public Image up, down, left, right;
    public RectTransform rect;

    public bool isCoreImage;


    public void Awake() {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    public void SetColor(Color c) {
        if (image != null) {
            image.color = c;
            if (up) up.color = c;
            if (down) down.color = c;
            if (right) right.color = c;
            if (left) left.color = c;
        }
    }

    public void SetCoreColor(Color col1, Color col2) {
        image.color = col1;
        subImage.color = col2;
    }

    public void HideDirectionalIndicators() {
        if (up) up.enabled = false;
        if (down) down.enabled = false;
        if (right) right.enabled = false;
        if (left) left.enabled = false;
    }

    public void SetDirectionalIndicator(BlockDirection selectedDirection) {
        HideDirectionalIndicators();
        switch (selectedDirection) {
            case BlockDirection.UP:
                if (up) up.enabled = true;
                break;
            case BlockDirection.DOWN:
                if (down) down.enabled = true;
                break;
            case BlockDirection.LEFT:
                if (left) left.enabled = true;
                break;
            case BlockDirection.RIGHT:
                if (right) right.enabled = true;
                break;
        }
    }

    public void SetRotation(BlockDirection selectedDirection) {
        if (image != null)
        image.transform.localEulerAngles = new Vector3(0, 0, BlockLevelCreator.DirectionToZRotation(selectedDirection));
    }

    public void SetSize(Vector2 pos, Vector2 size) {

     
        rect.sizeDelta = new Vector2(size.x, size.y);
        rect.anchoredPosition = new Vector2(pos.x * size.x, pos.y * size.y);


    }
}
