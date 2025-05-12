using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class StoryController : MonoBehaviour
{
    public List<StoryLevelButton> currentButtons = new List<StoryLevelButton>();

    public GameObject mainPanel, bonusPanel;

    public RectTransform currentCursor;

    public void SetStoryButtons() {
        foreach (StoryLevelButton s in mainButtons) {
            s.Set(s.levelGroup);
        }
        foreach (StoryLevelButton s in bonusButtons) {
            s.Set(s.levelGroup);
        }
    }

    public void HideAllPanels() {
        mainPanel.SetActive(false);
        bonusPanel.SetActive(false);
    }

    public int currentLevelIndex;

    public void MoveLevelTypeCursor(int dir) {
        currentLevelIndex += dir;
       // Debug.Log(currentLevelIndex.ToString());
        int max = 0;

        if (MainLevelButton.gameObject.activeInHierarchy) {
            max++;
        }

        if (BonusLevelButton.gameObject.activeInHierarchy) {
            max++;
        }

       

        if (max == 0) return;

        if (currentLevelIndex < 0) {
            currentLevelIndex = max - 1;
        }

        if (currentLevelIndex >= max) {
            currentLevelIndex = 0;
        }

        if (currentLevelIndex == 0) {
            SetMainLevels();
        }

        if (currentLevelIndex == 1) {
            SetBonusLevels();
        }
       



    }


    public void SetLevelTypeCursor(RectTransform t) {
        levelTypeCursor.SetParent(t);
        levelTypeCursor.transform.localPosition = Vector3.zero;
    }

    public void SetMainLevels() {
        currentLevelIndex = 0;
        HideAllPanels();
        currentButtons = mainButtons;
        currentCursor = mainCursor;
        mainPanel.SetActive(true);
        Hover(currentButtons[0]);
        SetLevelTypeCursor(MainLevelButton);
    }

    public void SetBonusLevels() {
        currentLevelIndex = 1;
        HideAllPanels();
        currentButtons = bonusButtons;
        currentCursor = bonusCursor;
        bonusPanel.SetActive(true);
        Hover(currentButtons[0]);
        SetLevelTypeCursor(BonusLevelButton);
    }

  

    public RectTransform MainLevelButton, BonusLevelButton, levelTypeCursor;

    public RectTransform mainCursor, bonusCursor;
    public List<StoryLevelButton> mainButtons, bonusButtons;

    internal void SetUp() {
        SetStoryButtons();
    }

    private void Update() {

        if (GameMasterManager.IsCurrentlyGamePad() && EventSystem.current.currentSelectedGameObject == null) {
            Debug.LogError("Steam Error " + Time.time);
            SelectCurrentfButton();
        }

        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }

        if (GameMasterManager.instance.inputManger.timeRewindButtonDown) {
            MoveLevelTypeCursor(-1);
        }

        if (GameMasterManager.instance.inputManger.cameraButtonDown) {
            MoveLevelTypeCursor(1);
        }

        
    }

    public void Hover(StoryLevelButton b) {
        //GameMasterManager.instance.StartCoroutine(MoveCursor(b));
        //storyCursor.gameObject.SetActive(true);
        //storyCursor.transform.position = b.transform.position;
        GameMasterManager.instance.StartCoroutine(MoveNum(currentCursor, b));
        
    }

    IEnumerator MoveNum(RectTransform cursor, StoryLevelButton b) {
        cursor.transform.localScale = Vector3.zero;
        yield return null;
        cursor.transform.localScale = Vector3.one;
        cursor.gameObject.SetActive(true);
        cursor.transform.position = b.transform.position;
        if (GameMasterManager.IsCurrentlyGamePad() && NavigationManager.instance.currentNavigationObject != b.navObject) {
            NavigationManager.SetSelectedObject(b.navObject);
        }
    }

    IEnumerator MoveCursor(RectTransform cursor, StoryLevelButton b) {
        // storyCursor.gameObject.SetActive(false);
        //storyCursor.transform.localScale = Vector3.zero;
        yield return null;
        cursor.transform.localScale = Vector3.one;
        
        //NavigationManager.SetSelectedObject(b.GetNavObject());
    }

    public void SelectCurrentfButton() {
        foreach (StoryLevelButton story in currentButtons) {
            if (story.levelGroup.envGroup == GameMasterManager.currentLevelGroup.envGroup) {
                Hover(story);
                NavigationManager.SetSelectedObject(story.navObject);
                return;
            }
        }
        Hover(currentButtons[0]);
        NavigationManager.SetSelectedObject(currentButtons[0].navObject);

    }

    internal void DeSelectCursor() {
        Debug.Log("HEreee " + Time.time);
       // storyCursor.gameObject.SetActive(false);
    }
}
