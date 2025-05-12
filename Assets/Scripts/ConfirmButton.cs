using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void VoidDelegate();

public class ConfirmButton : MonoBehaviour
{
    public VoidDelegate confirmDelegate;
    public TextMeshProUGUI text;
    public ConfirmScreen confirmScreen;
    public bool closeAfter;

    public void Set(VoidDelegateString voidDelegateString) {
        confirmDelegate = voidDelegateString.v;
        text.text = voidDelegateString.prompt;
        closeAfter = voidDelegateString.closeAfter;
    }

    public void Click() {
       
        confirmScreen.OptionClicked(this);
    }

   
}
