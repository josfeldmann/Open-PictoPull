using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewAchievementScreen : MonoBehaviour
{

    public AchievementProgressViewer viewerprefab;
    public List<AchievementProgressViewer> viewers;
    public Transform viewerRootTransform;
    public Material grayscale;

    public static Material grayscaleMat;

    public TextMeshProUGUI currentTimeText, levelsCompletedText, jumpText, pullText, portalsUsed, pulloutsUsed, cannonsUsed, resetsUsed;

    public void SetAchievements(List<AchievementObject> objects) {

        grayscaleMat = grayscale;

        int toAdd = objects.Count - viewers.Count; 

        for (int i = 0; i < toAdd; i++) {
            AchievementProgressViewer viewer = Instantiate(viewerprefab, viewerRootTransform);
            viewers.Add(viewer);
        }

        for (int i = 0; i < objects.Count; i++) {
            viewers[i].gameObject.SetActive(true);
            viewers[i].Set(objects[i]);
        }

        SetCurrentTimeText();

        LoadStats();


    }

    public void LoadStats() {

        levelsCompletedText.text = "Levels Completed: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetCompletedLevels().ToString();
        cannonsUsed.text = "Cannons Used: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetCannons().ToString();
        pullText.text = "Total Pulls: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetPushes();
        portalsUsed.text = "Portals Used: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetPortals().ToString();
        pulloutsUsed.text = "Pullouts Used: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetPullouts().ToString();
        jumpText.text = "Total Jumps: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetJumps();
        resetsUsed.text = "Total Resets: " + GameMasterManager.gameSaveFile.trackerSaveFile.GetResets();


    }



    private void Update() {
        SetCurrentTimeText();
        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }
    }


    public void SetCurrentTimeText() {
        TimeSpan time = TimeSpan.FromSeconds(AchievementTracker.GetCurrentTime());

        //here backslash is must to tell that colon is
        //not the part of format, it just a character that we want in output
        string str = time.ToString(@"hh\:mm\:ss");

        currentTimeText.text = "Time: " +  str;
    }

}
