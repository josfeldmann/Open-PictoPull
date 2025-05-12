using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour {

    public CharacterObject characterObject;
    public Image characterImage;
    public TextMeshProUGUI text;
    [HideInInspector] public CharacterSelector charSelector;

    public void Set(CharacterObject characterObject) {
        text.text = characterObject.GetName();
        characterImage.sprite = characterObject.characterIcon;
        this.characterObject = characterObject;
        
    }

    public void SelectCharacter() {
        charSelector.SelectCharacter(this);
    }
}

