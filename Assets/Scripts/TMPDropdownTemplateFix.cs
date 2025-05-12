using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPDropdownTemplateFix : MonoBehaviour
{
    private void OnEnable() {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas) {
            print("canvas finded");
            //canvas.sortingOrder = 1;
            canvas.overrideSorting = false;

        }
    }
}
