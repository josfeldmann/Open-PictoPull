using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;


public class LevelDisplayImage : MonoBehaviour {


    public Image image;
    public RectTransform goalIcon;
    public ColorIcon pulloutPrefab;
    public ColorIcon ladderPrefab;
    public ColorIcon singlePulloutPrefab;
    public ColorIcon blockerPrefab;
    public ColorIcon unionPrefab;
    public ColorIcon timerPrefab;
    public ColorIcon positiveOppositePrefab;
    public ColorIcon negativeOppositePrefab;
    public ColorIcon cannonPrefab;
    public ColorIcon cloudPrefab;
    public Transform iconTransform;

    public List<ColorIcon> ladders;
    public List<ColorIcon> fullpullouts;
    public List<ColorIcon> singlepullouts;
    public List<ColorIcon> cannons;
    public List<ColorIcon> blockers;
    public List<ColorIcon> unions;
    public List<ColorIcon> timers;
    public List<ColorIcon> negativeOpposites;
    public List<ColorIcon> positiveOpposites;
    public List<ColorIcon> clouds;


    public GameObject FourLevel, FiveLevel, SixLevel, IsUnion, IsTimer, IsOpposite, Crash;

    public Image coreCenterIcon, coreBorderIcon;

    public void HideAll() {
        FourLevel.gameObject.SetActive(false);
        FiveLevel.gameObject.SetActive(false);
        SixLevel.gameObject.SetActive(false);
        IsUnion.gameObject.SetActive(false);
        IsTimer.gameObject.SetActive(false);
        IsOpposite.gameObject.SetActive(false);
        Crash.gameObject.SetActive(false);
    }



    private float goalWidth;
    private LevelInfo level;
    public void SetLevel(LevelInfo g) {
        if (g == null || g.sprite == null) return;
        image.sprite = g.sprite;
        level = g;
        float ImageWidth = GameMasterManager.instance.levelSelector.ImageWidth;

        float division = Mathf.Max(g.saveFile.width, g.saveFile.height);

        goalWidth = ImageWidth / division;
        goalIcon.sizeDelta = new Vector2(goalWidth, goalWidth);
        goalIcon.anchoredPosition = new Vector2(goalWidth * g.saveFile.goalSpot.x, goalWidth * g.saveFile.goalSpot.y);

        if (g.sprite.rect.width == g.sprite.rect.height) {
            image.rectTransform.sizeDelta = new Vector2( ImageWidth, ImageWidth);
        } else if (g.sprite.rect.width > g.sprite.rect.height) {
            image.rectTransform.sizeDelta = new Vector2(ImageWidth, ImageWidth * ((float)g.sprite.rect.height / (float)g.sprite.rect.width));
        } else {
            image.rectTransform.sizeDelta = new Vector2(ImageWidth * ((float)g.sprite.rect.width / (float)g.sprite.rect.height), ImageWidth);
        }

        List<SpecialTile> ladderthings = new List<SpecialTile>();
        List<SpecialTile> singlepullthings = new List<SpecialTile>();
        List<SpecialTile> fullpullthings = new List<SpecialTile>();
        List<SpecialTile> cannonThings = new List<SpecialTile>();
        List<SpecialTile> timerThings = new List<SpecialTile>();
        List<SpecialTile> blockerThings = new List<SpecialTile>();
        List<SpecialTile> unionThings = new List<SpecialTile>();
        List<SpecialTile> positiveOppositeThings = new List<SpecialTile>();
        List<SpecialTile> negativeOppositeThings = new List<SpecialTile>();
        List<SpecialTile> cloudThings = new List<SpecialTile>();



        foreach (SpecialTile ss in g.saveFile.specialTile) {
            if (ss.isLadder()) {
                ladderthings.Add(ss);
            } else if (ss.isFullPullout()) {
                fullpullthings.Add(ss);
            } else if (ss.isSinglePullout()) {
                singlepullthings.Add(ss);
            } else if (ss.isCannon()) {
                cannonThings.Add(ss);
            } else if (ss.isTimer()) {
                timerThings.Add(ss);
            } else if (ss.isBlocker()) {
                blockerThings.Add(ss);
            } else if (ss.isOpposite()) {
                if (ss.isPositiveOpposite()) {
                    positiveOppositeThings.Add(ss);
                } else {
                    negativeOppositeThings.Add(ss);
                }
            } else if (ss.isUnion()) {
                unionThings.Add(ss);
            } else if (ss.isCloud()) {
                cloudThings.Add(ss);
            }
        }

        SetColorIcons(ladderthings, ladderPrefab, ladders);
        SetColorIcons(singlepullthings, singlePulloutPrefab, singlepullouts);
        SetColorIcons(fullpullthings, pulloutPrefab, fullpullouts);
        SetColorIcons(cannonThings, cannonPrefab, cannons);
        SetColorIcons(timerThings, timerPrefab, timers);
        SetColorIcons(blockerThings, blockerPrefab, blockers);
        SetColorIcons(unionThings, unionPrefab, unions);
        SetColorIcons(positiveOppositeThings, positiveOppositePrefab, positiveOpposites);
        SetColorIcons(negativeOppositeThings, negativeOppositePrefab, negativeOpposites);
        SetColorIcons(cloudThings, cloudPrefab, clouds);
        HideAll();

        if (g.saveFile.is4Level()) FourLevel.gameObject.SetActive(true);
        if (g.saveFile.is5Level()) FiveLevel.gameObject.SetActive(true);
        if (g.saveFile.isAllTimer()) IsTimer.gameObject.SetActive(true);
        if (g.saveFile.isAllUnion()) IsUnion.gameObject.SetActive(true);
        if (g.saveFile.isOpposite()) IsOpposite.gameObject.SetActive(true);
        if (g.saveFile.isSixLevel()) SixLevel.gameObject.SetActive(true);
        if (g.saveFile.levelType == LevelType.CRASH) Crash.gameObject.SetActive(true);

        EnvironmentGroup group = null;
        if (GameMasterManager.instance.levelSelector.currentLevelGroupButton != null && GameMasterManager.instance.levelSelector.currentLevelGroupButton.group != null) {
            group = GameMasterManager.instance.levelSelector.currentLevelGroupButton.group.envGroup;
        }

        if (group == null) {
            LevelGroup l = GameMasterManager.instance.GetLevelGroupFromString(g.saveFile.environmentName);
            if (l != null) group = l.envGroup;
        }

        if (group == null) {
            group = GameMasterManager.envLevelGroup.envGroup;
        }

        if (group != null) {
            coreCenterIcon.color = group.coreCenterColor;
            coreBorderIcon.color = group.coreBorderColor;
        }



    }

 

    public void SetColorIcons(List<SpecialTile> s, ColorIcon iconPrefab, List<ColorIcon> existing) {

        int toAdd = s.Count - existing.Count;

        for (int i = 0; i < toAdd; i++) {
            ColorIcon c = Instantiate(iconPrefab, iconTransform);
            existing.Add(c);
        }

        foreach (ColorIcon c in existing) {
            c.gameObject.SetActive(false);
            c.Awake();
        }

        List<Color> colors = BlockLevelCreator.ConvertVectorListToColor(level.saveFile.colors);

        for (int i = 0; i < s.Count; i++) {
            ColorIcon cc = existing[i];
            SpecialTile ss = s[i];
            cc.gameObject.SetActive(true);
            cc.SetColor(colors[ss.colorIndex]);
            cc.SetSize( ss.position, new Vector2(goalWidth, goalWidth));

            if (ss.id == SpecialTile.LADDERID) {
                cc.SetDirectionalIndicator(ss.direction);
            } else {
                cc.SetRotation(ss.direction);
            }
        }





    }




}
