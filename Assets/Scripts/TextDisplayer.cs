using TMPro;
using UnityEngine;

public class TextDisplayer : MonoBehaviour {

    public TextMeshProUGUI text;

    public void SetText(string s) {
        text.text = s;
    }


}
