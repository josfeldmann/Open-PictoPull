using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class VoidDelegateString {

    public VoidDelegate v;
    public String prompt;
    public bool closeAfter;

    public VoidDelegateString(VoidDelegate v, string prompt) {
        this.v = v;
        this.prompt = prompt;
        closeAfter = true;
    }
    public VoidDelegateString(VoidDelegate v, string prompt, bool close) {
        this.v = v;
        this.prompt = prompt;
        closeAfter = close;
    }
}


public class ConfirmScreen : MonoBehaviour
{

    public List<ConfirmButton> confirmButtons = new List<ConfirmButton>();
    public TextMeshProUGUI prompt;
    public bool deleteScreen;
    public void SetConfirmScreen(string p, List<VoidDelegateString> options) {

        prompt.text = p;
        gameObject.SetActive(true);
        deleteScreen = false;
        for (int i = 0; i < confirmButtons.Count; i++) {
            if (i < options.Count) {
                confirmButtons[i].confirmScreen = this;
                confirmButtons[i].gameObject.SetActive(true);
                confirmButtons[i].Set(options[i]);
            } else {
                confirmButtons[i].gameObject.SetActive(false);
            }
        }



    }


    private void Update() {
        if (deleteScreen) {
            if (Keyboard.current.deleteKey.wasPressedThisFrame) {
                OptionClicked(confirmButtons[0]);
            }
        }
    }


    public void OptionClicked(ConfirmButton confirmButton) {

        confirmButton.confirmDelegate?.Invoke();
        if (confirmButton.closeAfter) gameObject.SetActive(false);
    }

    
}
