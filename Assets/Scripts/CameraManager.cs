using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{

    public CinemachineBrain brain;
    public CinemachineVirtualCamera playerControllerCamera;
    public CinemachineVirtualCamera levelViewerCamera;
    public CinemachineVirtualCamera environmentCamera;
    public CinemachineVirtualCamera characterCustomizationCamera;
    public CinemachineVirtualCamera currentCustomCamera;

    public int focusPriority = 10, notPriority = 1;


    public void HideAll() {
        if (currentCustomCamera != null) currentCustomCamera.Priority = notPriority;
        playerControllerCamera.Priority = notPriority;
        levelViewerCamera.Priority = notPriority;
        environmentCamera.Priority = notPriority;
        characterCustomizationCamera.Priority = notPriority;
    }


    public bool setyet;
    public Vector3 followOffset;

    internal void SetEnvCamera(bool instant) {
        HideAll();
        if (setyet == false) {
            var transposer = environmentCamera.GetCinemachineComponent<CinemachineTransposer>();
            followOffset = transposer.m_FollowOffset;
            setyet = true;
        }
        
        if (Environment.instance == null) {
            Environment.instance = FindObjectOfType<Environment>();
        }

        environmentCamera.Follow = Environment.instance.cameraSpot;
        environmentCamera.Priority = focusPriority;
        
        if (instant) {
            brain.transform.SetPositionAndRotation(Environment.instance.cameraSpot.position + followOffset, Environment.instance.cameraSpot.rotation);
            environmentCamera.ForceCameraPosition(Environment.instance.cameraSpot.position + followOffset, Environment.instance.cameraSpot.rotation);
           // print(followOffset.ToString());
        }
      
        //playerControllerCamera.Priority = notPriority;
        
        
    }

    public void ShowPlayerCamera(bool instant) {
        HideAll();
        playerControllerCamera.Priority = focusPriority;

        if (instant) {
            Vector3 v = playerControllerCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
            brain.transform.SetPositionAndRotation(GameMasterManager.instance.playerController.transform.position + v, playerControllerCamera.transform.rotation);
            playerControllerCamera.ForceCameraPosition(GameMasterManager.instance.playerController.transform.position + v, playerControllerCamera.transform.rotation);

        }


    }

    public void ShowCharacterCustomizationCamera() {
        HideAll();
        characterCustomizationCamera.Priority = focusPriority;
    }

    public void ShowLevelViewerCamera() {
        HideAll();
        levelViewerCamera.Priority = focusPriority;
    }


    public void ShowCustomCamera(CinemachineVirtualCamera cam) {
        HideAll();
        currentCustomCamera = cam;
        currentCustomCamera.Priority = focusPriority;
    }







}
