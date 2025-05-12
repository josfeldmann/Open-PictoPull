using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Environment : MonoBehaviour
{

    public static float minYDistance = -2;

    public static Environment instance;

    public float yMinDis = -2;
    public AudioClip audioTrack;
    public Transform cameraSpot;
    public Transform respawnSpot;
    public List<LevelGroup> associatedLevelGroups;
    public bool overrideSkyColor = false;
    public Color skyColor;


    public StoryCutScene startCutscene;
    public StoryCutScene endCutscen;
    public List<StoryCutScene> otherCustscenes = new List<StoryCutScene>();





    private void Awake() {
        minYDistance = yMinDis;
        GameMasterManager.envLevelGroup = associatedLevelGroups[0];
        if (GameMasterManager.instance) {
            if (overrideSkyColor) {
                GameMasterManager.instance.SetSkyColor(skyColor);
            } else {
                GameMasterManager.instance.SetDefaultSkyColor();
            }
        }
        if (startCutscene) {
            startCutscene.HideCutSceneProps();
        }
        if (endCutscen) {
            endCutscen.HideCutSceneProps();
        }
    }


    private void Start() {
        
        if (GameMasterManager.instance == null) {
            #if !UNITY_SWITCH
            SceneManager.LoadScene(0);
            #else
            SceneManager.LoadScene(1);
            #endif
            return;
        } else {
           
            //GameMasterManager.instance.cameraManager.SetEnvCamera(true);
           // AudioManager.PlayTrack(audioTrack);
        }

        instance = this;

    }

    private void OnDrawGizmos() {
        if (cameraSpot != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cameraSpot.position, 0.2f);
         
        }

        if (respawnSpot != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(respawnSpot.position, 0.2f);
        }
    }
}
