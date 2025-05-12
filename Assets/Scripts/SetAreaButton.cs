using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetAreaButton : MonoBehaviour
{
    public Image image;
    public LevelGroup levelGroup;
    public TextMeshProUGUI areaName;


    private void Awake() {
        image.sprite = levelGroup.iconSprite;
        areaName.text = levelGroup.envGroup.GetName();
    }

    public void Click() {
        GameMasterManager.instance.creator.SetArea(levelGroup);
    }

    public void UnlockOrNot() {
        foreach (StoryChapters c in GameMasterManager.gameSaveFile.unlockedStoryChapters) {
            if (c == levelGroup.StoryChapter && GameMasterManager.instance.availableLevels.Contains(levelGroup)) {
                gameObject.SetActive(true);
                return;
            }
        }
        gameObject.SetActive(false);
    }


}
