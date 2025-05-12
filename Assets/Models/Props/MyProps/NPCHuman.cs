using TMPro;
using UnityEngine;

public class NPCHuman : MonoBehaviour {

    public NPCDialogue dialogue;
    public TextMeshProUGUI text;
    public GameObject textObject;

    private void Awake() {
      //  HideText();
    }

    public void HideText() {
        textObject.SetActive(false);
    }

    public void Show() {
        textObject.SetActive(true);
    }

    public void SetText(string s) {
        text.SetText(s);
    }

    public void PlaySound() {

    }


}
