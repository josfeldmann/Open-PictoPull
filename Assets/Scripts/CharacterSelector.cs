using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public List<CharacterObject> characters = new List<CharacterObject>();
    public CharacterButton characterButtonPrefab;
    public List<CharacterButton> charButtons = new List<CharacterButton>();
    public Transform charButtonParent;
    public InputManager inputManager;

    public void Activate() {
        gameObject.SetActive(true);
        int toAdd = 0;
        toAdd = characters.Count - charButtons.Count;
        for (int i = 0; i < toAdd; i++) {
            CharacterButton button = Instantiate(characterButtonPrefab, charButtonParent);
            charButtons.Add(button);
            button.charSelector = this;
        }

        foreach (CharacterButton c in charButtons) {
            c.gameObject.SetActive(false);
        }

        for (int i = 0; i < characters.Count; i++) {
            charButtons[i].gameObject.SetActive(true);
            charButtons[i].Set(characters[i]);
        }

    }

    public void SelectCharacter(CharacterObject o) {
        GameMasterManager.SelectCharacter(o, true);
    }

    internal void SelectCharacter(CharacterButton characterButton) {
        SelectCharacter(characterButton.characterObject);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public CharacterObject GetCharacterFromKey(string avatarID) {
        foreach (CharacterObject c in characters) {
            if (c.GetKey() == avatarID) {
                return c;
            }
        }
        return null;
    }

    private void Update() {
        if (inputManager.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }
        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }
    }
}

