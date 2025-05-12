using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementProgressViewer : MonoBehaviour {

    public GameObject ProgressBar;
    public TextMeshProUGUI DescriptionText, titleText, progressTextBar;
    public Image achImage, progressBarFill;


    public void Set(AchievementObject achievement) {
        achImage.sprite = achievement.achievementSprite;
        gameObject.name = achievement.GetName() + " Achievement Viewer";

        if (achievement.IsFulfilled(GameMasterManager.instance.tracker)) {
            if (!GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime.ContainsKey(achievement.GetID())) {
                GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime.Add(achievement.GetID(), AchievementTracker.GetCurrentTime());
            }
            titleText.text = achievement.GetName();// + " " +  GameMasterManager.gameSaveFile.trackerSaveFile.achievementCompletionTime[achievement.GetID()].ToString(@"hh\:mm\:ss");
        } else {
            titleText.text = achievement.GetName();
        }


        bool achieved = achievement.IsFulfilled(GameMasterManager.instance.tracker);
        if (achievement.useProgressBar) {
            float amtmax = achievement.GetProgressMax();
            float amt = Mathf.Min(achievement.GetCurrentProgress(), amtmax);
            DescriptionText.text = achievement.GetDescription();
           
            progressBarFill.fillAmount = amt/amtmax;
            progressTextBar.text = ((int)amt).ToString() + " / " + ((int)amtmax);
            ProgressBar.SetActive(true);

            if (achieved) {
                progressBarFill.color = Colors.GREEN;
            } else {
                progressBarFill.color = Colors.RED;
            }

        } else {
            DescriptionText.text = achievement.GetDescription();
           
            ProgressBar.SetActive(false);
        }
        if (achieved) {
            achImage.material = null;
        } else {
            achImage.material = ViewAchievementScreen.grayscaleMat;
        }
    }


}
