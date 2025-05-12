using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class LevelGroupButton : MonoBehaviour {
    [HideInInspector]public LevelGroup group;
    public int index = 0;
    public Image groupImage;
    public TextMeshProUGUI title;
    public NavigationObject navObject;
    public LevelSelector selector;
    public bool isCustom = false;
    public bool isLevelPack;
    internal void Set(LevelGroup g, int i) {
        group = g;
        title.text = g.GetLevelName();
        groupImage.sprite = g.iconSprite;
        index = i;
        gameObject.name = g.GetLevelName();
    }


    

    public void Click() {
        if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
            return;
        }
        if (isCustom) {
            selector.ShowCustomLevels();
        } else if (isLevelPack) {
            selector.ShowCustomLevelPacks();
        } else {
            selector.SetLevelGroup(this);
        }
    }
}
