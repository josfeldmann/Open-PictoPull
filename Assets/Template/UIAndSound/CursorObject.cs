using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class CursorObject : MonoBehaviour
{
    
    public static CursorObject instance;
    public Image spriteRenderer;
    public Image subImage;
    
    [Header("Sprites")]
    public Sprite cursorSprite;
    public Sprite pencilSprite;
    public Sprite eraseSprite;
    public Sprite pulloutSingle, pullOutFull;
    public Sprite portalSprite;
    public Sprite cannonSprite;
    public Sprite blockerSprite, unionSprite, timerSprite, cloudSprite;
    public GamepadCursor gameCursor;

    public Sprite UpSprite, DownSprite, RightSprite, LeftSprite;

    public Sprite BucketSprite;

    public static bool cursorShouldBeShowing = true;

    public void Setup() {
        instance = this;
        Cursor.visible = false;
        subImage.enabled = false;
    }


    public static void HideCursor() {
        // Debug.Log("Cursor Hidden " + Time.time);
      //  try {
            instance.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
      //  } catch { 
        
       // }
    }

    public static void ShowCursor() {
      //  Debug.Log("Show Cursor " + Time.time);
        if (instance == null) return;
        if (instance.gameObject.activeInHierarchy != true) {
          //  try {
                instance.gameObject.SetActive(true);
            //} catch {

            // }
            Cursor.lockState = CursorLockMode.None;
            instance.Update();

        }
    }

    public static void SetPencilSprite(Color c) {
        instance.NoRotation();
        colorSprite = true;
        instance.spriteRenderer.sprite = instance.pencilSprite;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    }

    public static void SetBucketSprite(Color c) {
        colorSprite = true;
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.BucketSprite;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    }

    public static bool colorSprite;

    public static void UpdateColorIfNeeded(Color c) {
        if (colorSprite) {
            instance.spriteRenderer.color = c;
            instance.subImage.color = c;
        }
    }

    public static void SetDefaultCursorSprite() {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.cursorSprite;
        instance.spriteRenderer.color = Color.white;
        colorSprite = false;
        instance.subImage.enabled = false;
    }

    public static void SetEraseSprite() {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.eraseSprite;
        instance.spriteRenderer.color = Color.white;
        colorSprite = false;
        instance.subImage.enabled = false;
    }

    public void DirectionRotation() {
        spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, BlockLevelCreator.DirectionToZRotation(GameMasterManager.instance.creator.selectedDirection));
    }

    public void NoRotation() {
        spriteRenderer.transform.localRotation = Quaternion.identity;
    }



    public static void SetPulloutSprite(Color c, bool single) {
        instance.DirectionRotation();
        if (single) {
            instance.spriteRenderer.sprite = instance.pulloutSingle;
        } else {
            instance.spriteRenderer.sprite = instance.pullOutFull;
        }
        colorSprite = true;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    }

    public static void SetLadderSprite(Color c, BlockDirection dir) {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.portalSprite;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = true;
        instance.subImage.color = c;
        colorSprite = true;
        switch (dir) {
            case BlockDirection.UP:
                instance.subImage.sprite = instance.UpSprite;
                break;
            case BlockDirection.DOWN:
                instance.subImage.sprite = instance.DownSprite;
                break;
            case BlockDirection.LEFT:
                instance.subImage.sprite = instance.LeftSprite;
                break;
            case BlockDirection.RIGHT:
                instance.subImage.sprite = instance.RightSprite;
                break;
        }
    }


    private void Update() {

       // Debug.Log(Event.current.mousePosition);

        if (!GameMasterManager.IsCurrentlyGamePad()) transform.position = Mouse.current.position.ReadValue();
        else {
           // Debug.Log(Event.current.mousePosition + " " + Time.time);
        }
    }

    public static void ResetCursorPositon() {
       //instance.transform.position = new Vector3(Screen.width/2, Screen.height/2);
    }

    public static void ShowCursorIfNeeded() {

        //  Debug.Log("Show Cursor If Needed " + Time.time);
        if (instance == null) instance = FindObjectOfType<CursorObject>();

        if (GameMasterManager.IsCurrentlyGamePad()) {
            if (GameMasterManager.showCursorWithGamePad && CursorObject.cursorShouldBeShowing) {
                CursorObject.ShowCursor();
                instance.gameCursor.enabled = true;
            } else {
                CursorObject.HideCursor();
                ResetCursorPositon();
                instance.gameCursor.enabled = false;
            }
        } else {
            if (CursorObject.cursorShouldBeShowing) {
                CursorObject.ShowCursor();
                instance.gameCursor.enabled = false;
            } else {
                CursorObject.HideCursor();
                ResetCursorPositon();
                instance.gameCursor.enabled = false;
            }
        }

        


    }


    public static void SetBlocker(Color c) {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.blockerSprite;
        colorSprite = true;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    }


    public static void SetTimer(Color c) {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.timerSprite;
        colorSprite = true;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    }


    public static void SetUnion(Color  c) {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.unionSprite;
        colorSprite = true;
        instance.spriteRenderer.color = c;
        instance.subImage.enabled = false;
    } 

    public static void SetCannonSprite(Color color) {
        instance.DirectionRotation();
        instance.spriteRenderer.sprite = instance.cannonSprite;
        colorSprite = true;
        instance.spriteRenderer.color = color;
        instance.subImage.enabled = false;
    }

    public static void SetOpposite(Sprite positiveSprite, Color color) {
        instance.NoRotation();
        instance.spriteRenderer.sprite = positiveSprite;
        colorSprite = true;
        instance.spriteRenderer.color = color;
        instance.subImage.enabled = false;
    }

    public bool IsShowing() {
        return gameObject.activeInHierarchy;
    }

    internal static void SetCloudSprite() {
        instance.NoRotation();
        instance.spriteRenderer.sprite = instance.cloudSprite;
        instance.subImage.enabled = false;
        instance.spriteRenderer.color = Color.white;
    }
}
