using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
   
    public GameObject pauseMenu;
    public NavigationObject resumeButton, levelSelectbutton, editorButton, skipLevelButton, backToWorkshopItemButton;

    public void HideAll() {
        resumeButton.gameObject.SetActive(false);
        levelSelectbutton.gameObject.SetActive(false);
        editorButton.gameObject.SetActive(false);
        backToWorkshopItemButton.gameObject.SetActive(false);
    }

    public void Shrink() {

        gameObject.SetActive(false);
    
    }

    public void UnShrink() {
        gameObject.SetActive(true);
      
    }

    public void ShowScreen() {
       // Time.timeScale = 0;
        UnShrink();
        HideAll();

        if (GameMasterManager.currentGameMode == GameMode.EDITOR) {
            resumeButton.gameObject.SetActive(true);
            editorButton.gameObject.SetActive(true);
        } else if (GameMasterManager.currentGameMode == GameMode.WORKSHOPCOMPLETENESSCHECK) {
            resumeButton.gameObject.SetActive(true);
            backToWorkshopItemButton.gameObject.SetActive(true);

        } else { 
            resumeButton.gameObject.SetActive(true);
            levelSelectbutton.gameObject.SetActive(true);
        }

        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL &&
            !GameMasterManager.gameSaveFile.completedStoryLevels.Contains(GameMasterManager.instance.generator.levelName.text)) {
            skipLevelButton.gameObject.SetActive(true);
        } else {
            skipLevelButton.gameObject.SetActive(false);
        }
        NavigationCheck();
        GameMasterManager.instance.generator.timeController.recording = false;
    }

    public void NavigationCheck() {
        if (GameMasterManager.IsCurrentlyGamePad()) {
            NavigationManager.SetSelectedObject(resumeButton);
            Debug.Log("Here");
        } else {
            NavigationManager.StopNavigation();
        }
    }

    public void HideScreen() {
       // Time.timeScale = 1;
        GameMasterManager.paused = false;
        Shrink();
    }



    public void Resume() {

        GameMasterManager.paused = false;
        //Time.timeScale = 1;
        HideScreen();
        GameMasterManager.instance.generator.timeController.recording = true;
    }

    public void LevelSelect() {
    
        GameMasterManager.paused = false;
        //Time.timeScale = 1;
        HideScreen();
        GameMasterManager.instance.PlayMenuTrack();
        GameMasterManager.instance.GoToLevelSelect();
    }



    public void SkipLevel() {
        if (!GameMasterManager.gameSaveFile.skippedLevels.Contains(GameMasterManager.currentLevel.saveFile.GetSaveKey())) {
            GameMasterManager.gameSaveFile.skippedLevels.Add(GameMasterManager.currentLevel.saveFile.GetSaveKey());
        }
        LevelSelect();
    }

    public void BackToEditor() {
     
        GameMasterManager.paused = false;
        // Time.timeScale = 1;
        GameMasterManager.instance.PlayMenuTrack();
        HideScreen();
        GameMasterManager.instance.GoToCustomLevelCreator(BlockLevelCreator.levelType, BlockLevelCreator.is3D);
    }

    internal void EnsureClosed() {

        // Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void BackToWorkshopButton() {
        GameMasterManager.instance.PlayMenuTrack();
        
        GameMasterManager.instance.playerController.machine.ChangeState(new DeadState());
#if !DISABLESTEAMWORKS
        //GameMasterManager.instance.workShopManager.uploadUI.GoBackTestPuzzle();
#endif
    }
}
