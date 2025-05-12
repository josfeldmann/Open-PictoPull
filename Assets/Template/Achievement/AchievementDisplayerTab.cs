using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayerTab : MonoBehaviour {
    public AchievementObject currentAchivementObject;
    public RectTransform rect;
    public Image image;
    public TextMeshProUGUI nameText, descriptionText;

    public void SetAchievement(AchievementObject o) {
        currentAchivementObject = o;
        image.sprite = o.achievementSprite;
        
        if (o.IsFulfilled(GameMasterManager.instance.tracker)) {
            if (!GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime.ContainsKey(o.GetID())) {
                GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime.Add(o.GetID(), AchievementTracker.GetCurrentTime());
            }
            nameText.text = o.GetName();// + " " + GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime[o.GetID()].ToString();
        } else {
            nameText.text = o.GetName();
        }
        descriptionText.text = o.GetDescription();
    }

}