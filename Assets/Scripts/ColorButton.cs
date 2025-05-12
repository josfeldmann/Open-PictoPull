using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour {

   
    public BlockLevelCreator creator;
    public ThreeDLevelCreator threeDLevelCreator;
    public Image colorImage;
    public int index = 0;
    public Color color;
    public TextMeshProUGUI text;

    public void Select() {

    }

    public void DeSelect() {

    }

    public void ChooseIndex() {
        if (creator != null) creator.SelectIndex(index, false);
        if (threeDLevelCreator != null) threeDLevelCreator.SetIndex(index);
    }

    public void SetColorIndex(Color c, int i) {
        index = i;
        color = c;
        colorImage.color = c;
        text.text = i.ToString();
    }


}
