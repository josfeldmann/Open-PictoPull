using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;

public class GamepadCursor : MonoBehaviour {

    public PlayerInput playerInput;
    private Mouse virtualMouse;
    public RectTransform cursorTransform;

    private bool prevMouse;
    
    public float speed = 400;
    public float switchSpeed = 750;

    bool addedYet = false;

    private void Awake()
    {
#if UNITY_SWITCH
        speed = switchSpeed;
#endif
    }

    private void OnEnable() {
        
        virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");

        Debug.Log("Disable " + Time.time + virtualMouse.position);
        
        InputSystem.AddDevice(virtualMouse);

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null) {
            Vector2 pos = cursorTransform.anchoredPosition;
            pos = new Vector2(Screen.width / 2, Screen.height / 2);
            InputState.Change(virtualMouse.position, pos);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        
        GamePadCheck();
        addedYet = true;
    }

    public static float completenessBuffer;

    private void OnDisable() {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        virtualMouse = null;
    }

    public void GamePadCheck() {
        if (virtualMouse == null) return;
        if (GameMasterManager.IsCurrentlyGamePad()) {
            InputState.Change(virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
        } else {
            InputState.Change(virtualMouse.position, new Vector2(-10000,-10000));
        }
    }
    
    bool aPressed;
    float aPressedBuffer;
    
    private void UpdateMotion() { 
        
        if (virtualMouse == null || Gamepad.current == null) return;
        Vector2 stickValue = Gamepad.current.leftStick.ReadValue();
        if (Gamepad.current.dpad != null)stickValue += Gamepad.current.dpad.ReadValue();
        stickValue = Vector2.ClampMagnitude(stickValue, 1);
        stickValue *= speed * Time.deltaTime;
        Vector2 curpos = virtualMouse.position.ReadValue();
        Vector2 newPos = curpos + stickValue;

        newPos.x = Mathf.Clamp(newPos.x, 0, Screen.width);

        newPos.y = Mathf.Clamp(newPos.y, 0, Screen.height);
        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, stickValue);

        aPressed = Gamepad.current.aButton.IsPressed();
        
        if (prevMouse != aPressed) {
            Debug.Log("APRESSSED " + Time.time);
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left,aPressed);
            InputState.Change(virtualMouse, mouseState);
            prevMouse = aPressed;
        }
        AnchorCursor(newPos);
    }

    public RectTransform canvasRect;
    public Camera mainCamera;



    private void AnchorCursor(Vector2 pos) {
        Vector2 anchoredPostion;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pos, null, out anchoredPostion);
        cursorTransform.anchoredPosition = anchoredPostion;
    }

}
