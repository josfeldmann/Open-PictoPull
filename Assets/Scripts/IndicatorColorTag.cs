using System.Collections.Generic;
using UnityEngine;

public class IndicatorColorTag : MonoBehaviour{

    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    public Color color;

    public void SetColor(Color c) {
        color = c;
        foreach (SpriteRenderer s in sprites) {
            s.color = c;
        }

    }
}
