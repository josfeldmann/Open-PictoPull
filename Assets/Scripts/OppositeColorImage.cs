using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OppositeColorImage : MonoBehaviour
{
    public OppositeColorSetter setter;
    public int index;
    public Color color;
    public Image image;
    public bool isNegative = false;

    public void CloseButton() { 
        if (isNegative) {
            setter.RemoveNegative(index);
        } else {
            setter.RemovePositive(index);
        }
    
    }

    public void Set(int i, Color c) {
        index = i;
        color = c;
        image.color = c;
    }


}
