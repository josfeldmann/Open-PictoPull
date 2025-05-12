using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image levelImage;

    public LevelInfo levelInfo;
    public GameObject tickMark;
    public GameObject lockIcon;
    public LevelGroup group;
    private LevelSelector selector;
    public int index = 0;
    public NavigationObject navObject;
    public float defaultWidth = 100f;
    public bool isCustom = false;
    public bool canplay = true;
    public Transform steamIndicator;
    public Image fourImage, fiveImage, sixImage, unionLevel, crashLevel, timerLevel, oppositeLevel;

    public void HideAllIcons() {
        fourImage.gameObject.SetActive(false);
        fiveImage.gameObject.SetActive(false);
        sixImage.gameObject.SetActive(false);
        unionLevel.gameObject.SetActive(false);
        crashLevel.gameObject.SetActive(false);
        timerLevel.gameObject.SetActive(false);
        oppositeLevel.gameObject.SetActive(false);
    }


    public void SetDefaultWidth(float f) {
        defaultWidth = f;
        SetLevelSize();
    }


    public void SetLevelSize() {
        if (levelInfo == null || levelInfo.sprite == null) {
            levelImage.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultWidth);
            return;
        }
        if (levelInfo.sprite.rect.width == levelInfo.sprite.rect.height) {
            levelImage.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultWidth);
        } else if (levelInfo.sprite.rect.width > levelInfo.sprite.rect.height) {
            levelImage.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultWidth * ((float)levelInfo.sprite.rect.height / (float)levelInfo.sprite.rect.width));
        } else {
            levelImage.rectTransform.sizeDelta = new Vector2(defaultWidth * ((float)levelInfo.sprite.rect.width / (float)levelInfo.sprite.rect.height), defaultWidth);
        }
    }

    public void SetLevelInfo(LevelInfo l, int i, LevelSelector s, LevelGroup g, bool isC) {
        levelInfo = l;
        isCustom = isC;
        levelImage.sprite = l.sprite;
        index = i;
        selector = s;
        group = g;
        name = l.GetName();
        

        steamIndicator.gameObject.SetActive(l.saveFile.isSteam);
        HideAllIcons();
       

        if (l.saveFile.levelType == LevelType.CRASH) {
            crashLevel.gameObject.SetActive(true);
        } else if (l.saveFile.is4Level()) {
            fourImage.gameObject.SetActive(true);
        } else if (l.saveFile.is5Level()) {
            fiveImage.gameObject.SetActive(true);
        } else if (l.saveFile.isSixLevel()) {
            sixImage.gameObject.SetActive(true);
        }
        if (l.saveFile.isAllUnion()) {
            unionLevel.gameObject.SetActive(true);
        }
        if (l.saveFile.isAllTimer()) {
            timerLevel.gameObject.SetActive(true);
        }
        if (l.saveFile.isOpposite()) {
            oppositeLevel.gameObject.SetActive(true);
        }


        SetLevelSize();
        
        canplay = true;
        
        if ( !isCustom && (GameMasterManager.gameSaveFile.skippedLevels.Contains(l.saveFile.GetSaveKey()) || GameMasterManager.gameSaveFile.completedStoryLevels.Contains(l.saveFile.GetSaveKey()))) {
            tickMark.SetActive(GameMasterManager.gameSaveFile.completedStoryLevels.Contains(l.saveFile.GetSaveKey()));
            
            lockIcon.gameObject.SetActive(false);
        } else {
            if ((GameMasterManager.currentGameMode == GameMode.LEVELSELECT && !isCustom)) {
                canplay = false;
                lockIcon.gameObject.SetActive(true);
                tickMark.SetActive(false);
            } else if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL &&
                       !GameMasterManager.gameSaveFile.skippedLevels.Contains(l.saveFile.GetSaveKey()) && 
                       !GameMasterManager.gameSaveFile.completedStoryLevels.Contains(l.saveFile.GetSaveKey()) &&
                       i > 0 &&
                       (!GameMasterManager.gameSaveFile.completedStoryLevels.Contains(g.levels[i-1].saveFile.GetSaveKey()) &&
                        !GameMasterManager.gameSaveFile.skippedLevels.Contains(g.levels[i - 1].saveFile.GetSaveKey()))) {

                canplay = false;
                lockIcon.gameObject.SetActive(true);
                tickMark.SetActive(false);

            } else {
                if (GameMasterManager.gameSaveFile.completedCustomLevels.Contains(l.saveFile.GetSaveKey())) {
                    tickMark.SetActive(true);
                } else if (GameMasterManager.currentGameMode == GameMode.LEVELPACK && GameMasterManager.currentLevelPackProgressFile.completedLevels.Contains(levelInfo.saveFile.levelName)) {
                    tickMark.SetActive(true);
                } else {
                    tickMark.SetActive(false);
                }
                lockIcon.gameObject.SetActive(false);
            }
            
        }
    }

    


   


    public void SelectLevel() {
       
        GameMasterManager.instance.levelSelector.SelectLevelButton(this);
    }

    public void OnSelected() {
        if (selector == null) selector = GameMasterManager.instance.levelSelector;
        selector.SetLastButton(this);
        selector.Hover(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        selector.Hover(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
      //  selector.HoverExit();
    }
}
