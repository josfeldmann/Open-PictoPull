using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentController : MonoBehaviour
{

    

   
    public Environment currentEnv;
    public EnvironmentGroup currentEnvironmentGroup = null;
    public bool working = false;

  

    public void SetEnvironment(EnvironmentGroup group) {
        if (currentEnvironmentGroup != null && currentEnvironmentGroup == group) {
            Debug.LogError("Same Index");
            return;
        }
        
        StartCoroutine(SetEnvironmentNum(group, true));
        //GameMasterManager.instance.fader.InstantlySetClear();
    }


    public IEnumerator SetEnvironmentNum(EnvironmentGroup group, bool isStart) {
        working = true;
        GameMasterManager.instance.playerController.ReturnToParent();
        CursorObject.cursorShouldBeShowing = false;
        if (currentEnvironmentGroup == null) {
            GameMasterManager.instance.fader.InstantlySetBlack();
        } else {
            
            GameMasterManager.instance.fader.FadeToBlack();
            while (GameMasterManager.instance.fader.moving) {
                yield return null;
            }
         // if (currentEnv != null) Destroy(currentEnv.gameObject);
        }
#if !UNITY_SWITCH
        SceneManager.LoadScene(group.scene);
        #else
        //TODO check this
        SceneManager.LoadScene(group.scene.BuildIndex)
        //SceneManager.LoadScene(currentEnvIndex + 1);
        #endif
        yield return null;
        yield return null;
        currentEnvironmentGroup = group;
        
        GameMasterManager.instance.generator.SetTileMats();

        yield return null;
        if (isStart) {
            GameMasterManager.instance.cameraManager.SetEnvCamera(true);
            GameMasterManager.currentGameMode = GameMode.MENUBACKGROUNDLOAD;
            GameMasterManager.instance.StartCoroutine(GameMasterManager.instance.generator.GenerateLevel(GameMasterManager.currentLevelGroup.levels[0].saveFile, false, false));
            GameMasterManager.instance.generator.SetPlayerAtStartPostion();
            
        }
       
        CursorObject.cursorShouldBeShowing = true;
        
        yield return null;
        working = false;
        GameMasterManager.gameSaveFile.lastEnv = group.envKey;
        GameMasterManager.instance.SaveGameFile();
        StartCoroutine(FadeOutHalf());

        if (isStart) GameMasterManager.instance.generator.timeController.recording = false;

    }


    IEnumerator FadeOutHalf() {
        GameMasterManager.instance.fader.FadeFromBlack();
        while (GameMasterManager.instance.fader.moving) {
            yield return null;
        }
    }


}
