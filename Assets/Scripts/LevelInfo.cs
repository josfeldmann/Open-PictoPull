using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

[System.Serializable]
public partial class LevelInfo {
    public BlockSaveFile saveFile;
    public Sprite sprite;
    public string storyOverideName = "";    


    
    public LevelInfo(TextAsset t) {
        saveFile = JsonTool.StringToObject<BlockSaveFile>(t.text);
        MakeSprite();
    }

    public LevelInfo(BlockSaveFile s) {
        saveFile = s;
        MakeSprite();
    }

    public string GetName() {
        if (storyOverideName != null && storyOverideName.Length > 0 && GameMasterManager.currentGameMode == GameMode.STORYLEVEL) {
            return storyOverideName;
        }
        return saveFile.levelName;
    }

    public void MakeSprite() {

        
        List<Color> colors = BlockLevelCreator.ConvertVectorListToColor(saveFile.colors);
        Texture2D tex = new Texture2D(saveFile.width, saveFile.height);
        for (int x = 0; x < saveFile.width; x++)
            for (int y = 0; y < saveFile.height; y++) {
                Color c = new Color(1f, 1f, 1f, 0f);
                if (saveFile.is3DLevel) {

                } else {
                    if (saveFile.colorGrid[x, y] > 0 && (saveFile.colorGrid[x, y] - 1) < colors.Count) {
                        c = colors[saveFile.colorGrid[x, y] - 1];
                    }
                    tex.SetPixel(x, y, c);
                }
            }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        Sprite s = Sprite.Create(tex, new Rect(new Vector2(), new Vector2(saveFile.width, saveFile.height)), new Vector2());
        sprite = s;
    }
}
