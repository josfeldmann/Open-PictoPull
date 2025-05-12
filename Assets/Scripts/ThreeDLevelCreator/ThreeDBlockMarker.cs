using System;
using UnityEngine;

public class ThreeDBlockMarker : MonoBehaviour {

    public Vector3Int position;
    public Color color;
    public int colorIndex;
    public MeshRenderer meshRenderer;
    internal ThreeDLevelCreator creator;

    internal void SetColor(int currentColorPaletteIndex) {
        Color c = creator.spawnedColorButtons[currentColorPaletteIndex].color;
        meshRenderer.material.SetColor("_Color", c);
        color = c;
        colorIndex = currentColorPaletteIndex;
    }
}
