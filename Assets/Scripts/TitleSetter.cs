using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TitleSetter : MonoBehaviour
{
    public ColorPalette palette;
    public List<Image> images = new List<Image>();


    private static System.Random rnd = new System.Random();

   public void SetRandomTitleColors() {
        images = new List<Image>(images.Shuffle());
        for (int i =0; i < images.Count; i++) {
            images[i].color = palette.colors[i];
        }
    }


    private void Awake() {
        rnd = new System.Random();
        SetRandomTitleColors();
    }

}
