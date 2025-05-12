using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ScreenShotListener : MonoBehaviour
{

    public float minCooldown = 2f;
    public static float lastTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator TakeScreenShot(bool hideui)
    {
        bool shouldshowText = false;
        if (GameMasterManager.instance != null) {
            shouldshowText = false;
            shouldshowText = GameMasterManager.instance.playerController.debugText.enabled && GameMasterManager.instance.playerController.debugText.gameObject.activeInHierarchy;
            if (hideui) {
                GameMasterManager.instance.gameplayUI.SetActive(false);
            }
            GameMasterManager.instance.playerController.debugText.gameObject.SetActive(false);
            GameMasterManager.instance.playerController.debugText.enabled = false;
        }

        yield return null;
        yield return null;


        lastTime = Time.time + minCooldown;
        string ssString = SaveManager.GetScreenShotPath() + "/" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "-" + DateTime.Now.Millisecond.ToString() + ".png";
        Debug.Log(ssString);
        ScreenCapture.CaptureScreenshot(ssString);


        yield return null;
        yield return null;
        if (GameMasterManager.instance != null) {
            if (hideui) {
                GameMasterManager.instance.gameplayUI.SetActive(true);
            }

            if (GameMasterManager.IsEditor() && shouldshowText) {
                GameMasterManager.instance.playerController.debugText.gameObject.SetActive(true);
                GameMasterManager.instance.playerController.debugText.enabled = true;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        if (Keyboard.current != null && GameMasterManager.IsEditor() && (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.oKey.wasPressedThisFrame)) {
            
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>() != null) {
                Debug.Log("InputBlocked " + Time.time);
                return;
            }
            if (Time.time > lastTime) {
               StartCoroutine(TakeScreenShot(Keyboard.current.oKey.wasPressedThisFrame));
            }
        }
    }
}
