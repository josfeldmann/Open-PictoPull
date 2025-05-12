using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageMenu : MonoBehaviour {

    public static List<LocalizedTMPro> localizedTMPros = new List<LocalizedTMPro>();


    public List<LanguageObject> languageList = new List<LanguageObject>();
    public Transform languageGroupingTransform;
    public LanguageButton langaugeButtonPrefab;
    public List<LanguageButton> languageButtons = new List<LanguageButton>();
    public Image MainMenuLanguageImage;
    public LanguageObject currentObject;

    public void Setup() {
        
        foreach (LanguageButton b in languageButtons) {
            if (b != null) {
                Destroy(b.gameObject);
            }
        }
        languageButtons = new List<LanguageButton>();
        foreach (LanguageObject l in languageList) {
            LanguageButton but = Instantiate(langaugeButtonPrefab, languageGroupingTransform);
            but.Set(l);
            languageButtons.Add(but);
        }
        SetLanguage(currentObject);
    }

    public void SetLanguage(LanguageObject l) {
        currentObject = l;
        //LocalizationSettings.SelectedLocale = l.locale;
        foreach (LocalizedTMPro p in localizedTMPros) {
            p.SetString();
        }
        MainMenuLanguageImage.sprite = l.languageSprite;
    }

}
