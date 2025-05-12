using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LevelPackInfoDisplayer : MonoBehaviour {

    public TextMeshProUGUI title, author, description;
    public Image levelImage;
    public string authorPreText;


    public void SetPack(LevelPack pack) {
        title.text = pack.packName;
        author.text = authorPreText + pack.author;
        description.text = pack.packDescription;
        levelImage.sprite = pack.sprite;
        levelImage.EnsureImageAspectRatio();
    }



}
 

