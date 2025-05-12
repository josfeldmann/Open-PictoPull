using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonImagePrompt : MonoBehaviour {
    public string prompt = "Confirm";
    private string inputName = "";
    public string inputToCheck = "key";
    public Image displayImage;
    public TextMeshProUGUI description, keyboardText;
    public ButtonInfo info;
    public bool isGamePadOnly = false;
    float defaultWidth = 0, defaultHeight = 0;

    private void Awake() {
        defaultWidth = displayImage.rectTransform.rect.width;
        defaultHeight = displayImage.rectTransform.rect.height;
        ButtonPromptManager.AddListener(this);
        InputUpdate();
    }

    public void InputUpdate() {
        
        info = ButtonPromptManager.GetSprite(inputToCheck);
        if (info == null) {
           
        }


        if (isGamePadOnly) {
            if (GameMasterManager.IsCurrentlyGamePad()) {
                gameObject.SetActive(true);
            } else {
                gameObject.SetActive(false);
            }
        }
        if (info == null || info.ButtonSprite == null) return;
        displayImage.sprite = info.ButtonSprite;

        if (info.ButtonSprite.rect.width != info.ButtonSprite.rect.height) {
            if (info.ButtonSprite.rect.width > info.ButtonSprite.rect.height) {
                displayImage.rectTransform.sizeDelta = new Vector2(defaultWidth, info.ButtonSprite.rect.height / info.ButtonSprite.rect.width * defaultHeight);
            } else {
                displayImage.rectTransform.sizeDelta = new Vector2(info.ButtonSprite.rect.width / info.ButtonSprite.rect.height * defaultWidth, defaultHeight);
            }
        } else {
            displayImage.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultHeight);
        }
        if (keyboardText)keyboardText.enabled = false;
        if (description)description.SetText(prompt);
    }

    public void OnControlsChange(PlayerInput p) {
        Debug.Log("ListenChange");
        InputUpdate();
    }


}
