using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelPack")]
public class BuiltInLevelPack : ScriptableObject {

    public string packName;
    public string packAuthor;
    [TextArea]
    public string packDescription;
    public Sprite sprite;
    public List<TextAsset> assets;



    public LevelPack GetLevelPack() {

        LevelPack p = new LevelPack();

        p.packName = packName;
        p.packDescription = packDescription;
        p.sprite = sprite;
        p.associatedLevelPack = this;
        p.author = packAuthor;
        p.numberOfLevels = assets.Count;
        return p;

    }






}

