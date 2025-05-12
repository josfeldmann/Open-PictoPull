using UnityEngine;



[CreateAssetMenu(menuName = "EnvironmentGroup")]
public class EnvironmentGroup : ScriptableObject {


    public string envKey;
    public string envNameString;
    public string scene;
   // public AudioClip envTrack;
    public Material blockMaterial, gridMaterial, coreMaterial;
    public Vector2Int maxLevelSize;

    public Color coreBorderColor, coreCenterColor;

    public string GetName() {
        return envNameString;
    }

    public bool CanFitLevel(Vector2Int v) {
        if (v.x <= maxLevelSize.x && v.y <= maxLevelSize.y) {
            return true;
        } else {
            return false;
        }
    }


   


}
