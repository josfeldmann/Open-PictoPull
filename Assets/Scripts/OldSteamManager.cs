using UnityEngine;
using System.Collections;
#if !UNITY_WEBGL && !DISABLESTEAMWORKS
using Steamworks;
#endif
/*

public class SteamManager : MonoBehaviour {
    public const int gameID = 1820790;
    public static SteamManager instance;
    [SerializeField] private bool isSteam = false;
    public static bool isSteamBuild = false;

#if !UNITY_WEBGL

   
    public void Awake() {
        if (!isSteam) return;
        if (instance == this) return;
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        try {
            
            SteamClient.Init(gameID);
            isSteamBuild = isSteam;
            instance = this;
        } catch (System.Exception e) {
            Debug.LogError(e + " Error launchingSteam");
        }
    }

    private void Update() {
        if (!isSteam) return;
        Steamworks.SteamClient.RunCallbacks();
    }


    private void OnApplicationQuit() {
        if (!isSteam) return;
        Steamworks.SteamClient.Shutdown();
    }
#endif

}
*/