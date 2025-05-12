using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelPackButton : MonoBehaviour, IPointerEnterHandler {

    public LevelPack levelPack;
    public Image packImage;
    public TextMeshProUGUI packTitle;
    public GameObject editButton;
    public GameObject SteamButton;
    bool issteam = false;
    

    public void SetPack(LevelPack p) {
        levelPack = p;
        packImage.sprite = p.sprite;
        packTitle.text = p.packName;
        packImage.EnsureImageAspectRatio();
        issteam = p.isSteam();
        SteamButton.SetActive(issteam);
        editButton.SetActive(!issteam);
    }

    public void Click() {
        GameMasterManager.instance.levelPackManager.Click(this);
    }

    public void Hover() {
        GameMasterManager.instance.levelPackManager.Hover(this);
    }

    private void OnEnable() {
        if (levelPack == null || GameMasterManager.currentGameMode != GameMode.LEVELSELECT || levelPack.associatedLevelPack != null || issteam) {
            editButton.gameObject.SetActive(false);
        } else {
            editButton.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Hover();
    }

    public void EditButton() {
        GameMasterManager.instance.levelPackManager.EditLevelPack(levelPack);
    }

}
 

