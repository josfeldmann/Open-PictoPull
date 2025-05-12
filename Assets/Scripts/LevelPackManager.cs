using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPackManager : MonoBehaviour
{
    public Transform pickLevelPackSortingPoint;
    public Transform addLevelPackButton;
    public LevelPackButton packButtonPrefab;
    public List<LevelPackButton> levelPackButtons;
    
    public List<BuiltInLevelPack> builtInLevelPack;
    public List<LevelPack> allPacks = new List<LevelPack>();
    public LevelPackInfoDisplayer infoDisplayer;
    public Sprite defaultPackSprite;

    


    public void UpdateLevelPacks() {


        allPacks = new List<LevelPack>();

        foreach (BuiltInLevelPack b in builtInLevelPack) {
            allPacks.Add(b.GetLevelPack());
        }

        List<LevelPack> levelPack = SaveManager.LoadCustomLevelPacks();


        foreach (LevelPack p in levelPack) {
            allPacks.Add(p);
        }


        //We have all level packs now make the buttons
        int toAdd = allPacks.Count - levelPackButtons.Count;

        for (int i = 0; i < toAdd; i++) {
            LevelPackButton b = Instantiate(packButtonPrefab, pickLevelPackSortingPoint);
             levelPackButtons.Add(b);
        }

        for (int i = 0; i < levelPackButtons.Count; i++) {
            if (i < allPacks.Count) {
                levelPackButtons[i].gameObject.SetActive(true);
                levelPackButtons[i].SetPack(allPacks[i]);
            } else {
                levelPackButtons[i].gameObject.SetActive(false);
            }
        }

        Hover(levelPackButtons[0]);
        addLevelPackButton.SetAsLastSibling();

    }


    public void ShowLevelPacks() {
        if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT || GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
            addLevelPackButton.gameObject.SetActive(true);
        } else {
            addLevelPackButton.gameObject.SetActive(false);
        }
        
    }


    public void AddLevelPackButton() {
        GameMasterManager.instance.ShowLevelPackCreator(new LevelPack());
    }

    public void EditLevelPack(LevelPack p) {
        GameMasterManager.instance.ShowLevelPackCreator(p);
    }

    public void Hover(LevelPackButton pack) {
        infoDisplayer.SetPack(pack.levelPack);
    }
    public void Click(LevelPackButton levelPackButton) {
        if (LevelSelector.selectingForWorkshop) {
          
                #if !DISABLESTEAMWORKS
                GameMasterManager.instance.workShopManager.PickPuzzlePackForUpload(levelPackButton.levelPack);
                #endif
                LevelSelector.selectingForWorkshop = false;
                return;
            
        } else {
           GameMasterManager.instance.levelSelector.ShowSelectedCustomLevelPack(levelPackButton.levelPack);
        }
    }

}
 

