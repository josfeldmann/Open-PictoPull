using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Blocker : MonoBehaviour
{
  
    public Transform leftMostPoint;
    public bool blockersEnabled;
    public BlockerMesh leftDiagonal, rightDiagonal, farRight, farleft, leftForwardBlock, rightForwardBlock, twoLeft, twoRight, frontRightTwo, frontLeftTwo;
    public Material enabledMaterial, disabledMaterial;
    public BlockLevelGenerator generator;

    public bool showMeshes = true;


    private void Awake() {
        if (showMeshes && GameMasterManager.IsEditor()) {
            SetMeshes(true);
        } else {
            SetMeshes(false);
        }
        DisableBlockers();
    }

    public void SetMeshes(bool b) {
        leftDiagonal.SetVisuals(b);
        rightDiagonal.SetVisuals(b);
        farRight.SetVisuals(b);
        farleft.SetVisuals(b);
        leftForwardBlock.SetVisuals(b);
        rightForwardBlock.SetVisuals(b);
        twoLeft.SetVisuals(b);
        twoRight.SetVisuals(b);
        frontRightTwo.SetVisuals(b);
        frontLeftTwo.SetVisuals(b);
    }


    public void DisableBlockers() {
        leftDiagonal.Disable();
        rightDiagonal.Disable();
        farRight.Disable();
        farleft.Disable();
        leftForwardBlock.Disable();
        rightForwardBlock.Disable();
        twoLeft.Disable();
        twoRight.Disable();
        frontRightTwo.Disable();
        frontLeftTwo.Disable();
    }
    public static float lastEnableTime = 0;


    bool upCheckBool, leftCheckBool, rightCheckBool; bool farRightBool, farLeftBool;

    public void EnableBlockers() {
        if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            DisableBlockers();
            return;
        }
        if (generator.player.lastvalidGridPosition.y <= 0) return;
        upCheckBool = generator.player.CielingCheck();
        leftCheckBool = generator.player.LeftCheck();
        rightCheckBool = generator.player.RightCheck();
        farLeftBool = generator.player.FarLeftCheck();
        farRightBool = generator.player.FarRightCheck();

        if (upCheckBool) {
            if (leftCheckBool)leftDiagonal.Enable();
            if (rightCheckBool) rightDiagonal.Enable();
        }

        if (leftCheckBool) farleft.Enable();
        if (rightCheckBool) farRight.Enable();

        leftForwardBlock.Enable();
        rightForwardBlock.Enable();
        if (farLeftBool)twoLeft.Enable();
        if (farRightBool)twoRight.Enable();
        frontRightTwo.Enable();
        frontLeftTwo.Enable();


        blockersEnabled = true;
        lastEnableTime = Time.time + 0.1f;
    }

    public void MoveToPos(Vector3Int v) {
        transform.position = PlayerController.PositionToVector3(v);
        //transform.position = generator.leftSide.transform.position + v - new Vector3(-2.5f,-1.5f,0.5f);
       // transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, -2.5f, -0.5f));
    }

    




}
