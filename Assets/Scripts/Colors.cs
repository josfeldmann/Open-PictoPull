using UnityEngine;

public class Colors : MonoBehaviour {


    public static Color RED, GREEN, WHITE, BLACK, BLUE, YELLOW, TIMERGREEN, TIMERRED, TIMERYELLOW, DEFAULTSKYCOLOR;


    public Color red, green, black, white, yellow, blue, timerRed, timerGreen, timerYellow;
    public Color defaultSkyColor;
    private void Awake() {
        
    }

    public void SetupColors() {
        RED = red;
        GREEN = green;
        BLUE = blue;
        WHITE = white;
        YELLOW = yellow;
        BLACK = black;
        TIMERRED = timerRed;
        TIMERGREEN = timerGreen;
        TIMERYELLOW = timerYellow;
        DEFAULTSKYCOLOR = defaultSkyColor;
    }


}
