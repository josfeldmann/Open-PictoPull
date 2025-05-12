using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldProxy : MonoBehaviour
{
    public void SetFileNameInput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "File Name");
    }
    public void SetPathNameInput(TMP_InputField input) {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(input, "Path Name");
    }
}
