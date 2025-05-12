using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class CustomLevelSelector : MonoBehaviour
{
    public NavigationObject pushLevel, crashLevel, stretchlevel;
    public RectTransform cursorObject;
    
    public void Show() {
        CursorObject.cursorShouldBeShowing = true;
        InputCheck();
    }


    private void Update() {
        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        } else if (GameMasterManager.instance.inputManger.jumpDown) {
            if (NavigationManager.instance.currentNavigationObject != null) {
                NavigationManager.instance.currentNavigationObject.button.onClick.Invoke();
            }
        }

    }

    public void HoverNavigationObject(NavigationObject g) {
        StartCoroutine(CursorNum(g));
    }

    IEnumerator CursorNum(NavigationObject g) {
        cursorObject.gameObject.SetActive(false);
        yield return null;
        cursorObject.gameObject.SetActive(true);
        cursorObject.transform.position = g.transform.position;

    }

    public void InputCheck() {
        if (GameMasterManager.IsCurrentlyGamePad()) {
            CursorObject.ShowCursorIfNeeded();
            NavigationManager.SetSelectedObject(pushLevel);
            
        } else {
            CursorObject.ShowCursorIfNeeded();
            cursorObject.gameObject.SetActive(false);
            
        }
    }
}
