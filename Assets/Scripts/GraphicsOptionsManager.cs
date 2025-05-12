using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptionsManager : MonoBehaviour
{
    public const string VSYNC = "VSYNC", LIMITFRAMERATE = "LIMITFRAMERATE", FRAMERATE = "FRAMERATE";
    public int minFrameRate = 1;
    public Toggle vsyncToggle, limitFramRateToggle;
    public TMPro.TMP_InputField framrateInput;

    public void Update() {
        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }
    }


    private void Awake() {
        if (!PlayerPrefs.HasKey(VSYNC)) PlayerPrefs.SetInt(VSYNC, 1);
        if (!PlayerPrefs.HasKey(LIMITFRAMERATE)) PlayerPrefs.SetInt(LIMITFRAMERATE, 0);
        if (!PlayerPrefs.HasKey(FRAMERATE)) PlayerPrefs.SetInt(FRAMERATE, 60);

        framrateInput.SetTextWithoutNotify(PlayerPrefs.GetInt(FRAMERATE).ToString());
        vsyncToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(VSYNC) > 0);
        limitFramRateToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(LIMITFRAMERATE) > 0);
    }

    public void ToggleVSync() {
        if (vsyncToggle.isOn) {
            PlayerPrefs.SetInt(VSYNC, 1);
        } else {
            PlayerPrefs.SetInt(VSYNC, 0);
        }
        UpdateGraphics();
    }

    public void ToggleLimitFrameRate() {
        if (limitFramRateToggle.isOn) {
            PlayerPrefs.SetInt(LIMITFRAMERATE, 1);
        } else {
            PlayerPrefs.SetInt(LIMITFRAMERATE, 0);
        }
        UpdateGraphics();
    }

    public void SetFrameRate() {
        int amt = 0;
        if (int.TryParse(framrateInput.text, out amt)) {

        } else {
            amt = minFrameRate;
        }
        amt = Mathf.Max(minFrameRate, amt);
        PlayerPrefs.SetInt(FRAMERATE, amt);
        framrateInput.SetTextWithoutNotify(amt.ToString());

        UpdateGraphics();
    }
    
    public void UpdateGraphics() {
        if (PlayerPrefs.GetInt(LIMITFRAMERATE) == 1) {
            Application.targetFrameRate = PlayerPrefs.GetInt(FRAMERATE);
        } else {
            Application.targetFrameRate = -1;
        }
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(VSYNC);
    }
}
