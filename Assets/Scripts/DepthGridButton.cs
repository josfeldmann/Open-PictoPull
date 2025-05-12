using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class DepthGridButton : MonoBehaviour, IPointerClickHandler
{
    public int depthValue;
    public Vector2Int coord;
    public TextMeshProUGUI text;

    public static int currentMaxDepth = 3;

    public void SetDepthValue(int i) {
        depthValue = i;
        text.text = i.ToString();
    }

    public void SetCoord(Vector2Int v) {
        coord = v;
        gameObject.name = "(" + v.x.ToString() + "," + v.y.ToString() + ")";
    }

    

    public void Click() {
        depthValue++;
        if (depthValue > currentMaxDepth) {
            depthValue = 0;
        }
        SetDepthValue(depthValue);
        EventSystem.current.SetSelectedGameObject(null);
        NavigationManager.SetSelectedObject(null);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            depthValue--;
            if (depthValue < 0) {
                depthValue = currentMaxDepth;
            }
            SetDepthValue(depthValue);
            EventSystem.current.SetSelectedGameObject(null);
            NavigationManager.SetSelectedObject(null);
        }
    }
}
