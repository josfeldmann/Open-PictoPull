using CanvasKeyboard;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenKeyBoardSceen : MonoBehaviour
{
    public TextMeshProUGUI prompt;
    public TMPro.TMP_InputField inputField, targetInput;

    public CanvasKeyboard.CanvasKeyboard canvasKeyboard;

    public VoidDelegate onDone;
    public VoidDelegate onCancel;




    public void SetKeyboard(TMP_InputField  input, string Prompt) {
        if (!GameMasterManager.IsCurrentlyGamePad()) return;
        transform.SetAsLastSibling();
        GameMasterManager.instance.cursor.transform.SetAsLastSibling();
        gameObject.SetActive(true);
        prompt.text = Prompt;
        inputField.SetTextWithoutNotify(input.text);
        canvasKeyboard.SetBuildString(input.text);
        targetInput = input;
        canvasKeyboard.isDone = false;
        canvasKeyboard.isCancelled = false;
    }

    public void Finish() {
        gameObject.SetActive(false);
        targetInput.text = inputField.text;
    }

    public void Update() {
        if (EventSystem.current.currentSelectedGameObject != null) EventSystem.current.SetSelectedGameObject(null);
        if (canvasKeyboard.isDone || canvasKeyboard.isDone) {
            Finish();
            canvasKeyboard.isDone = false;
            canvasKeyboard.isCancelled = false;
        }

    }



}
