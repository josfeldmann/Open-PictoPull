using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddLevelPackScreen : MonoBehaviour
{
    public TMPro.TMP_InputField title, author, description;
    public Transform levelTransform;
    public Transform addButtonTransform;

    public LevelPackLevelButton levelpacklevelbutton;
    public List<LevelPackLevelButton> levelPackLevelButtons;
    public List<LevelInfo> levelInfos;
    public Image image;
    public Sprite defaultSprite;

    public Button deleteButton, saveButton;

    string pathToDelete = null;

    private void OnEnable() {
        CursorObject.cursorShouldBeShowing = true;
        GameMasterManager.showCursorWithGamePad = true;
        CursorObject.ShowCursorIfNeeded();
    }

    private void OnDisable() {
        GameMasterManager.showCursorWithGamePad = false;
        CursorObject.ShowCursorIfNeeded();
    }




    public void LoadLevelPack(LevelPack levelPack) {

        levelInfos = levelPack.GetSaveFiles();
        title.SetTextWithoutNotify(levelPack.packName);
        author.SetTextWithoutNotify(levelPack.author);
        description.SetTextWithoutNotify(levelPack.packDescription);
        image.sprite = levelPack.sprite;
        if (image.sprite == null) image.sprite = defaultSprite;
        if (levelPack.associatedLevelPack == null && levelPack.levelPath != null && levelPack.levelPath.Length > 0) {
            pathToDelete = levelPack.levelPath;
            deleteButton.gameObject.SetActive(true);
        } else {
            pathToDelete = null;
            deleteButton.gameObject.SetActive(false);
        }
        LoadImages();
        NavigationManager.DeselectObject();
        CompleteCheck();

    }

    public void LoadImages() {

        int toAdd = levelInfos.Count - levelPackLevelButtons.Count;

        for (int i =0; i < toAdd; i++) {
            LevelPackLevelButton buttons = Instantiate(levelpacklevelbutton, levelTransform);
            levelPackLevelButtons.Add(buttons);
        }

        for (int i = 0; i < levelPackLevelButtons.Count; i++) {
            if (i < levelInfos.Count) {
                levelPackLevelButtons[i].SetLevel(levelInfos[i], i);
            } else {
                levelPackLevelButtons[i].DeActivate();
            }
        }

        addButtonTransform.SetAsLastSibling();
    }

    public void SetSprite(Sprite s) {
        image.sprite = s;
    }

    bool completeCheck = false;
    public TextMeshProUGUI errorText;
    public GameObject errorWindow;


    public void CompleteCheck() {
        string errorString = "";
        completeCheck = true;

        if (title.text == null || title.text.Length == 0 || title.text.Length > 30) {
            completeCheck = false;
            errorString += "- Title must be between 1 and 30 characters long\n";
        }

        if (description.text == null || description.text.Length == 0 || description.text.Length > 100) {
            completeCheck = false;
            errorString += "- Description must be between 1 and 100 characters long\n";
        }

        if (author.text == null || author.text.Length == 0 || author.text.Length > 30) {
            completeCheck = false;
            errorString += "- Author must be between 1 and 100 characters long\n";
        }

        if (levelInfos.Count == 0) {
            completeCheck = false;
            errorString += "Must have at least one level+\n";
        }

        if (errorString.Length > 0) {
            errorString = "Errors:\n" + errorString;
            errorText.color = Colors.RED;
        } else {
            errorText.color = Colors.GREEN;
        }

        
        errorWindow.gameObject.SetActive(!completeCheck);
        errorText.text = errorString;
        saveButton.interactable = completeCheck;



    }



    public void SaveButton() {
        LevelPack s = new LevelPack();
        s.author = author.text;
        s.packName = title.text;
        s.packDescription = description.text;
        SaveManager.SaveCustomLevelPack(s, levelInfos, image.sprite);
        GameMasterManager.instance.levelPackManager.UpdateLevelPacks();
        BackButton();
    }

    public void RemoveButton(LevelInfo l) {
        levelInfos.Remove(l);
        LoadImages();
        CompleteCheck();
    }

    public void AddLevelButton() {
        GameMasterManager.instance.SelectLevelForLevelPack();
    }

    public void BackButton() {
        GameMasterManager.instance.GoToLevelSelectButton();
    }

    public void AddLevelToCurrentPack(LevelInfo levelInfo) {
        if (levelInfos.Count == 0) {
            Texture2D tex = GameMasterManager.MakeWorkshopSprite(levelInfo.sprite);
            SetSprite(Sprite.Create(tex, new Rect(0,0, tex.width, tex.height), new Vector2()));
        }
        levelInfos.Add(levelInfo);
        
        LoadImages();
        CompleteCheck();
    }

    public void DeletePack() {

        GameMasterManager.instance.confirmScreen.SetConfirmScreen("Delete " + title.text, new List<VoidDelegateString>() {
        new VoidDelegateString(DeleteDelete, "Yes"),
        new VoidDelegateString(Empty, "No", true)
        });
        GameMasterManager.instance.confirmScreen.deleteScreen = true;
    }
    public void DeleteDelete() {
        SaveManager.DeleteLevelPack(pathToDelete);
        GameMasterManager.instance.levelPackManager.UpdateLevelPacks();
        BackButton();
    }

    public void Empty() {

    }


    public void ShowAuthorInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(author, "Author");
    }

    public void ShowPackDescriptionInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(description, "Pack Description");
    }

    public void ShowNameInput() {
        GameMasterManager.instance.keyboardScreen.SetKeyboard(title, "Pack Title");
    }
}
