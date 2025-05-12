using UnityEngine;
using UnityEngine.UI;

public class LevelPackLevelButton : MonoBehaviour {

    public LevelInfo levelInfo;
    public int levelIndex;
    public Image levelImage;

    public bool active;
    public void Activate() {
        active = true;
        gameObject.SetActive(true);
    }

    public void DeActivate() {
        active = false;
        gameObject.SetActive(false);
    }

    public void SetLevel(LevelInfo info, int levelindex) {
        levelInfo = info;
        levelImage.sprite = info.sprite;
        levelIndex = levelindex;
        Activate();
        levelImage.EnsureImageAspectRatio();
    }


    public static float lastDelete;

    public void Delete() {
        if (Time.time < lastDelete) return;
        GameMasterManager.instance.addLevelPackScreen.RemoveButton(levelInfo);
        lastDelete = Time.time + 0.1f;
    }


} 
