using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {


    public bool moving = false;
    public Image fadeImage;
    public float currentAlpha = 0;
    public float targetAlpha;
    public float fadeSpeed = 1f;

    private void Update() {
        if (moving) {
            if (targetAlpha != currentAlpha) {
                currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
                return;
            } else {
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
                if (CursorObject.cursorShouldBeShowing) CursorObject.ShowCursorIfNeeded();
                moving = false;
            }
            
        }
    }

    public void FadeToBlack() {
        fadeImage.gameObject.SetActive(true);
        moving = true;
        currentAlpha = 0;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
        targetAlpha = 1;
        CursorObject.HideCursor();
    }


    public void FadeFromBlack() {
        fadeImage.gameObject.SetActive(true);
        moving = true;
        currentAlpha = 1;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
        targetAlpha = 0;
        CursorObject.HideCursor();
    }

    public void InstantlySetBlack() {
        fadeImage.gameObject.SetActive(true);
        moving = false;
        currentAlpha = 1;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
        targetAlpha = 1;
        CursorObject.HideCursor();
    }

    public void InstantlySetClear() {
        fadeImage.gameObject.SetActive(true);
        moving = false;
        currentAlpha = 0;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
        targetAlpha = 0;
        
    }



}
