using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "ColorPalette")]
public class ColorPalette : ScriptableObject
{

    public Texture2D text;

    public string paletteName;
    public List<Color> colors;

    [ContextMenu("LoadPaletteFromImage")]
    public void LoadColorsFromTexture() {
        colors = new List<Color>();
        for (int x = 0; x < text.width; x++) {
            for (int y = 0; y < text.width; y++) {
                if (!colors.Contains(text.GetPixel(x, y))) {
                    colors.Add(text.GetPixel(x, y));
                }
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
