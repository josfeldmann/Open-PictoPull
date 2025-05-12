using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {

    public Image image;
    public TextMeshProUGUI text;
    public LanguageObject languageObject;

    public void Set(LanguageObject l) {
        languageObject = l;
        image.sprite = l.languageSprite;
        text.text = l.languageName;
    }


    public void SetLanguage() {
        GameMasterManager.instance.mainMenu.languageMenu.SetLanguage(languageObject);
    }

}
