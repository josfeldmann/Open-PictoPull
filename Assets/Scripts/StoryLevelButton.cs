using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryLevelButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{

    [Header("Navigation")]
    public StoryLevelButton up;
    public StoryLevelButton down, left, right;
    public NavigationObject navObject;


    public Image storyImage;
    public TextMeshProUGUI titleText, levelText, areaTitle;
    public LevelGroup levelGroup;
    public GameObject unlockIcon;
    private bool unlocked = false;
    public bool isVisible = false;
    public Image fillImage;

    private void Awake() {
        if (up) navObject.up = up.navObject;
        if (down) navObject.down = down.navObject;
        if (right) navObject.right = right.navObject;
        if (left) navObject.left = left.navObject;
    }

    

    public void OnHover() {
       // cursorOver = true;
        GameMasterManager.instance.storyController.Hover(this);

    }

    private void OnEnable() {
        //cursorOver = false;
    }

    public void Set(LevelGroup level) {

        VisibilityCheck();
        if (isVisible) {
            unlocked = levelGroup.isLevelUnlocked();

            if (unlocked && !GameMasterManager.gameSaveFile.unlockedStoryChapters.Contains(levelGroup.StoryChapter)) {
                GameMasterManager.AddUnlockedStoryLevel(level);
            }
        } else {
            unlocked = false;
        }
        if (unlocked) {
            unlockIcon.gameObject.SetActive(false);
            int completedAmount = levelGroup.GetCompletedLevelNumber();
            
            levelText.text = completedAmount.ToString() + "/" + level.jsonLevels.Count;
        } else {
            unlockIcon.gameObject.SetActive(true);
            levelText.SetText(levelGroup.GetUnlockString());
        }

        storyImage.sprite = level.iconSprite;
        titleText.text = levelGroup.GetLevelName();
        areaTitle.text = levelGroup.envGroup.GetName();
        fillImage.fillAmount = (float)levelGroup.GetCompletedLevelNumber() / (float)level.jsonLevels.Count;
       // cursorOver = false;
    }

   

    public void VisibilityCheck() {
        isVisible = levelGroup.IsVisible();

        if (isVisible) {
            if (GameMasterManager.isDemo) {
                if (!GameMasterManager.instance.demoLevels.Contains(levelGroup)) {
                    gameObject.SetActive(false);
                } else {
                    gameObject.SetActive(true);
                }
            } else {
                gameObject.SetActive(true);
            }
        } else {
            gameObject.SetActive(false);
        }
    }

    public static StoryLevelButton currentLevelButton;

    public void SetLevelGroup() {
       // if (!GameMasterManager.IsCurrentlyGamePad() && !cursorOver) return;
        // if (!GameMasterManager.IsCurrentlyGamePad() && EventSystem.current.IsPointerOverGameObject() && !GameMasterManager.GetComponentsInHoveredObjects<StoryLevelButton>().Contains(this)) return;
        if (!unlocked) return;
        if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
            return;
        }
        currentLevelButton = this;
        GameMasterManager.instance.SetStoryLevelGroup(levelGroup);
       // cursorOver = false;
    }
   // bool cursorOver = false;

    public void OnPointerExit(PointerEventData eventData) {
        //cursorOver = false;
        if (NavigationManager.instance.currentNavigationObject == navObject) {
          //  GameMasterManager.instance.storyController.DeSelectCursor();
          //  NavigationManager.DeselectObject();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnHover();
    }
}
