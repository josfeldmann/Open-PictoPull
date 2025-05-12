using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTextBox : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public LevelTextBoxManager manager;


    public void Close() {
        manager.RemoveBox(this);
    }

}
