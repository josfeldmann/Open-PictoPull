using UnityEngine;
using TMPro;

public class ColorPaletteSelectButton : MonoBehaviour {

    public ColorPalette palette;
    public BlockLevelCreator creator;
    public TextMeshProUGUI text;


    public void Set(ColorPalette p) {
        palette = p;
        text.text = p.paletteName;
    }

    public void Click() {
        creator.ApplyColorPalette(palette);
    }


}
