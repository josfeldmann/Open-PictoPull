using TMPro;
using UnityEngine;

public class LocalizedTMPro : MonoBehaviour {


    //public LocalizedString lString;
   // private TextMeshProUGUI text;

    private void Awake() {
     //   if (!LanguageMenu.localizedTMPros.Contains(this)) {
     //       LanguageMenu.localizedTMPros.Add(this);
     //   }
     //   SetString();
    }

    public void SetString() {
      //  if (text == null) text = GetComponent<TextMeshProUGUI>();
      //  if (text != null) text.text = lString.GetLocalizedString();
    }

    private void OnDestroy() {
      //  if (LanguageMenu.localizedTMPros.Contains(this)) {
      //      LanguageMenu.localizedTMPros.Remove(this);
      //  }
    }


}
