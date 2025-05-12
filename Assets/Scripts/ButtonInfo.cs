using UnityEngine;

public class ButtonInfo {
    public ButtonType bType;
    public string buttonName;
    public Sprite ButtonSprite;

    public ButtonInfo(ButtonType bType, string buttonName, Sprite buttonSprite) {
        this.bType = bType;
        this.buttonName = buttonName;
        ButtonSprite = buttonSprite;
    }
}
