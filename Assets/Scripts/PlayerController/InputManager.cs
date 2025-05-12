using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour {
    public float horizontal, vertical;
    public float camHorizontal, camVertical;

    public float threeDCamHorizontal, threeDCamVertical;

    public bool jumpDown;
    public bool cameraButtonHeld = false;
    public bool pullButtonHeld = false;
    public bool escKeyDown = false;

    public bool interact;
    public bool interactDown;

    public bool timeRewindButtonPressed = false;
    public bool timeRewindButtonDown = false;
    public bool cameraButtonDown;
    public bool cancelButtonDown;
    public bool hintDown;



    private void Update() {

       


        //  horizontal = Input.GetAxis("Horizontal");
        //    vertical = Input.GetAxis("Vertical");
        //   jumpDown = Input.GetKeyDown(KeyCode.Space);
        //   cameraButtonHeld = Input.GetKey(KeyCode.Q);
        //   interactButton = Input.GetKeyDown(KeyCode.E);
        //  pullButtonHeld = Input.GetKey(KeyCode.LeftShift);
        //  escKeyDown = Input.GetKeyDown(KeyCode.Escape);
        //   timeRewindButton = Input.GetKey(KeyCode.Z);

       // threeDCamHorizontal = Input.GetAxis("Mouse X");
        //threeDCamVertical = Input.GetAxis("Mouse Y");


     if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            cancelButtonDown = true;
            inputDetectedThisFrame = true;
        }
    }

    public bool inputDetectedThisFrame;
    internal bool jumpHeld;

    private void LateUpdate() {
        escKeyDown = false;
        jumpDown = false;
        timeRewindButtonDown = false;
        cameraButtonDown = false;
        cancelButtonDown = false;
        interactDown = false;
        hintDown = false;
        inputDetectedThisFrame = false;
    }



    public void OnMove(CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        horizontal = v.x;
        vertical = v.y;
        inputDetectedThisFrame = true;
    }

    public void OnMouse(CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        threeDCamHorizontal = v.x;
        threeDCamVertical = v.y;
        inputDetectedThisFrame = true;
    }


    public void OnCameraMove(CallbackContext context) {
        Vector2 v = context.ReadValue<Vector2>();
        camHorizontal = v.x;
        camVertical = v.y;
        inputDetectedThisFrame = true;
    }


    public void Jump(CallbackContext context) {
        //Debug.Break();
        if (context.phase == InputActionPhase.Started) {
            jumpDown = true;
            jumpHeld = true;
        } else if (context.performed) {
            jumpHeld = true;
        } else if (context.canceled) {
            jumpHeld = false;
        }
        inputDetectedThisFrame = true;
    }


    public void Camera(CallbackContext context) {
        if (context.performed) {
            cameraButtonHeld = true;
            if (context.action.WasPerformedThisFrame() && cameraButtonDown == false) {
                cameraButtonDown = true;
            } else {
                cameraButtonDown = false;
            }
        } else if (context.canceled) {
            cameraButtonHeld = false;
        }
        inputDetectedThisFrame = true;
    }

    public void Pull(CallbackContext context) {
        if (context.performed) {
            pullButtonHeld = true;
        } else if (context.canceled) {
            pullButtonHeld = false;
        }
        inputDetectedThisFrame = true;
    }


    public void Escape(CallbackContext context) {
        if (context.action.WasPerformedThisFrame() && escKeyDown == false) {
            escKeyDown = true;
        } else {
            escKeyDown = false;
        }
        inputDetectedThisFrame = true;
    }


    public void Rewind(CallbackContext context) {
        if (context.performed) {
            timeRewindButtonPressed = true;
            if (context.action.WasPerformedThisFrame() && timeRewindButtonDown == false) {
                timeRewindButtonDown = true;
            } else {
                timeRewindButtonDown = false;
            }
        } else if (context.canceled) {
            timeRewindButtonPressed = false;
        }
        inputDetectedThisFrame = true;
    }

    public void Interact(CallbackContext context) {
        if (context.performed) {
            interact = true;
            if (context.action.WasPerformedThisFrame() && interactDown == false) {
                interactDown = true;
            } else {
                interactDown = false;
            }
        } else if (context.canceled) {
            interact = false;
        }
        inputDetectedThisFrame = true;
    }


    public void CancelButton(CallbackContext context) {
        if (context.performed) {
            if (context.action.WasPerformedThisFrame() && timeRewindButtonDown == false) {
                cancelButtonDown = true;
            } else {
                cancelButtonDown = false;
            }
        } else if (context.canceled) {
            cancelButtonDown = false;
        }
        inputDetectedThisFrame = true;
    }


    public void HintButton(CallbackContext context) {
        if (context.performed) {
            if (context.action.WasPerformedThisFrame() ) {
                hintDown= true;
            } else {
               
            }
        } else if (context.canceled) {
            
        }
        inputDetectedThisFrame = true;
    }



    public void JumpButtonMobile() {
        jumpDown = true;
        NavigationManager.DeselectObject();
        inputDetectedThisFrame = true;
    }

    public void PullButtonMobile() {

    }

    public void BButtonMobile() {
        interactDown = true;
        NavigationManager.DeselectObject();
        inputDetectedThisFrame = true;
    }


    public void OnGamePadDisconnect() {

        if (GameMasterManager.instance.playerController.machine.currentState is PlayerIdleState) {
            GameMasterManager.instance.playerController.Pause();
        }


    }




}
