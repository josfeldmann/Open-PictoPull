using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameMode {
    STORYLEVEL = 0, LEVELSELECT = 1, EDITOR = 2, LEVELPACK = 3, ADDLEVELPACKBUTTON = 4, WORKSHOPCOMPLETENESSCHECK = 5,
    MENUBACKGROUNDLOAD = 6,
    MAINMENU = 7
}

public class WinScreen : MonoBehaviour
{
    public RectTransform outPosition, inPosition;
    public RectTransform menu;

    public Vector2 targetPos;

    public NavigationObject NextLevelButton;
    public NavigationObject ReplayButton, EditorButton, LevelSelectButton;
    public float dropDownSpeed = 1000f;
    public TextMeshProUGUI nextLevelText;
    public string nextLevelString;

    private void Awake() {
        ImmediatelyHideWinScreen();
    }

    public void HideAllButtons() {
        NextLevelButton.gameObject.SetActive(false);
        ReplayButton.gameObject.SetActive(false);
        EditorButton.gameObject.SetActive(false);
        LevelSelectButton.gameObject.SetActive(false);
    }

    bool moving = false;

    public void ShowWinScreen() {
        HideAllButtons();
        menu.gameObject.SetActive(true);
        menu.anchoredPosition = outPosition.anchoredPosition;
        targetPos = inPosition.anchoredPosition;
        moving = true;
        GameMasterManager.instance.generator.timeController.recording = false;

        SetText();

        switch (GameMasterManager.currentGameMode) {
            case GameMode.LEVELSELECT:
                if (GameMasterManager.nextLevel != null) NextLevelButton.gameObject.SetActive(true);
                ReplayButton.gameObject.SetActive(true);
                LevelSelectButton.gameObject.SetActive(true);
                break;
            case GameMode.STORYLEVEL:
                if (GameMasterManager.nextLevel != null) NextLevelButton.gameObject.SetActive(true);
                ReplayButton.gameObject.SetActive(true);
                LevelSelectButton.gameObject.SetActive(true);
                break;
            case GameMode.EDITOR:
                ReplayButton.gameObject.SetActive(true);
                EditorButton.gameObject.SetActive(true);
                break;
            case GameMode.LEVELPACK:
                NextLevelButton.gameObject.SetActive((GameMasterManager.currentLevelPackIndex + 1) < GameMasterManager.currentLevelPackLevels.Count);
                ReplayButton.gameObject.SetActive(true);
                LevelSelectButton.gameObject.SetActive(true);
                break;
            case GameMode.ADDLEVELPACKBUTTON:
                Debug.LogError("Don't get here");
                break;
        }

        

    }

    public void SetText() {
        nextLevelText.text = GameMasterManager.ReplacePrompts("Next Level");
    }


    public void InputCheck() {
        if (moving) return;
        if (GameMasterManager.IsCurrentlyGamePad()) {
            
            if (GameMasterManager.currentGameMode == GameMode.EDITOR) {
                NavigationManager.SetSelectedObject(EditorButton);
            } else {
                if (NextLevelButton.gameObject.activeInHierarchy) {
                    NavigationManager.SetSelectedObject(NextLevelButton);
                } else {
                    NavigationManager.SetSelectedObject(LevelSelectButton);
                }
            }
            CursorObject.HideCursor();
        } else {
            CursorObject.SetDefaultCursorSprite();
            CursorObject.ShowCursor();
            NavigationManager.StopNavigation();
        }
    }

    public void ImmediatelyHideWinScreen() {
        menu.anchoredPosition = outPosition.anchoredPosition;
        menu.gameObject.SetActive(false);
    }

    public void BackToEditor() {
        if (!gameObject.activeInHierarchy || !menu.gameObject.activeInHierarchy) return;
        GameMasterManager.instance.PlayMenuTrack();
        GameMasterManager.instance.GoToCustomLevelCreator(BlockLevelCreator.levelType, BlockLevelCreator.is3D);
    }

    public void ReplayLevel() {
        if (!gameObject.activeInHierarchy || !menu.gameObject.activeInHierarchy) return;

        if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT || GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
            BlockSaveFile b = GameMasterManager.currentLevel.saveFile;
            GameMasterManager.instance.StartLevel(b, GameMasterManager.currentLevel, GameMasterManager.currentIndex, false);
        } else {
            GameMasterManager.instance.PlayCustomLevelTest();
        }
    }

    public void LevelSelect() {
        if (!gameObject.activeInHierarchy || !menu.gameObject.activeInHierarchy) return;
        GameMasterManager.instance.PlayMenuTrack();
        GameMasterManager.instance.GoToLevelSelect();
    }

    public void NextLevel() {
        if (!gameObject.activeInHierarchy || !menu.gameObject.activeInHierarchy) return;
        if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
            GameMasterManager.currentLevelPackIndex++;
            BlockSaveFile b = GameMasterManager.currentLevelPackLevels[GameMasterManager.currentLevelPackIndex].saveFile;
            GameMasterManager.instance.StartLevel(b, GameMasterManager.nextLevel, GameMasterManager.currentIndex + 1, false);
            PlayerController.cantJumpTime = Time.time + 0.25f;
        } else {
            BlockSaveFile b = GameMasterManager.nextLevel.saveFile;
            if (GameMasterManager.nextLevelGroup != null) {
                GameMasterManager.currentIndex = -1;
                GameMasterManager.currentLevelGroup = GameMasterManager.nextLevelGroup;
                GameMasterManager.nextLevelGroup = null; 
            }
            GameMasterManager.instance.StartLevel(b, GameMasterManager.nextLevel, GameMasterManager.currentIndex + 1, false);
            PlayerController.cantJumpTime = Time.time + 0.25f;
        }
    }


    private void Update() {
         if (moving) {
            if (menu.anchoredPosition != targetPos) {
                menu.anchoredPosition = Vector2.MoveTowards(menu.anchoredPosition, targetPos, dropDownSpeed * Time.deltaTime);

            } else {
                
                //CursorObject.ShowCursor();
                moving = false;
                if (menu.anchoredPosition == inPosition.anchoredPosition) {
                    InputCheck();
                }
                PlayerController.instance.SetIdleGroundedAnimation();
            }
        } else if (menu.gameObject.activeInHierarchy) {
           if (GameMasterManager.instance.inputManger.jumpDown && NextLevelButton.gameObject.activeInHierarchy) {
                NextLevel();
            }
        }
    }


}


