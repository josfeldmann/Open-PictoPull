using UnityEngine;
using UnityEngine.InputSystem;
using UnityLibrary;

public class ThreeDCameraController : MonoBehaviour {

    public ThreeDLevelCreator creator;
    public InputManager inputManager;

    public float moveSpeed = 5.0f;
    public float rotateSpeed = 180f;
    public Camera cam;

    public StateMachine<ThreeDCameraController> stateMachine;

  

    public bool onSpot = false;
    public Vector3Int spot;
    public Vector3 realSpot;
    public GameObject debugCursor;

    public GameObject UIMaster;


    public UnitySmoothMouseLook mouseLook;


    public LayerMask builderLayer;


    public void StartLevelEditor() {
        stateMachine = new StateMachine<ThreeDCameraController>(new CameraMoving(), this);
    }
    
    

    private void LateUpdate() {
        if (stateMachine != null)stateMachine.Update();
    }

    public void SetMoving() {
        stateMachine.ChangeState(new CameraMoving());
    }

    public void SetUI() {
        stateMachine.ChangeState(new UIState());
    }

    RaycastHit hit;
    public void RayCheck() {

        if (Physics.Raycast(cam.transform.position, (cam.transform.forward * 20f), out hit, 20f, builderLayer)) {
            
            debugCursor.transform.position = hit.point;

            if (debugCursor.transform.localPosition.x < 0 ||
                debugCursor.transform.localPosition.y < 0 ||
                debugCursor.transform.localPosition.z < 0 ||
                debugCursor.transform.localPosition.x >= creator.size.x ||
                debugCursor.transform.localPosition.y >= creator.size.y ||
                debugCursor.transform.localPosition.z >= creator.size.z) {

                onSpot = false;
                debugCursor.gameObject.SetActive(false);
                spot = -Vector3Int.one;

            } else {

                onSpot = true;
                debugCursor.gameObject.SetActive(true);


                spot = new Vector3Int((int)debugCursor.transform.localPosition.x, (int)debugCursor.transform.localPosition.y, (int)debugCursor.transform.localPosition.z);
                
            
            }

        } else {
            onSpot = false;
            debugCursor.gameObject.SetActive(false);
            spot = -Vector3Int.one;

        }
        realSpot = debugCursor.transform.position;


    }




    public class UIState : State<ThreeDCameraController> {


        public override void Enter(StateMachine<ThreeDCameraController> obj) {
            obj.target.UIMaster.gameObject.SetActive(true);
            obj.target.mouseLook.enabled = false;
            CursorObject.ShowCursor();
        }


        public override void Update(StateMachine<ThreeDCameraController> obj) {
            if (Keyboard.current.tabKey.wasPressedThisFrame) {
                obj.ChangeState(new CameraMoving());
                return;
            }
        }


    }

    public class CameraMoving : State<ThreeDCameraController> {

        public override void Enter(StateMachine<ThreeDCameraController> obj) {
            obj.target.UIMaster.gameObject.SetActive(false);
            obj.target.mouseLook.enabled = true;
            CursorObject.HideCursor();
        }

        private static Vector2 rotation = Vector2.zero;

        public override void Update(StateMachine<ThreeDCameraController> obj) {


            if (Keyboard.current.tabKey.wasPressedThisFrame) {
                obj.ChangeState(new UIState());
                return;
            }

            


            obj.target.RayCheck();

            if (Mouse.current.leftButton.wasPressedThisFrame) {
                if (obj.target.onSpot) {
                    obj.target.creator.SpawnMarkerAtMouseClick(obj.target.spot, obj.target.realSpot, obj.target.hit.normal);
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame) {
                if (obj.target.onSpot) {

                    ThreeDBlockMarker marker = obj.target.hit.collider.transform.parent.GetComponent<ThreeDBlockMarker>();
                    if (marker != null) {
                        obj.target.creator.RemoveMarker(marker);
                    }
                }
            }

            if (Mouse.current.middleButton.wasPressedThisFrame) {

                if (obj.target.onSpot) {

                    Vector3Int spot = obj.target.spot;

                    if (obj.target.hit.normal == Vector3.up && obj.target.spot.y > 0) {
                        spot -= Vector3Int.up;
                    }

                    if (obj.target.hit.normal == Vector3.right) {
                        spot -= Vector3Int.right;
                    }

                    if (obj.target.hit.normal == Vector3.forward) {
                        spot -= Vector3Int.forward;
                    }


                    obj.target.creator.SetGoalSpot(spot);
                }


            }



            if (Keyboard.current.qKey.wasPressedThisFrame) {
                obj.target.creator.MoveColor(-1);
            }


            if (Keyboard.current.eKey.wasPressedThisFrame) {
                obj.target.creator.MoveColor(1);

            }




            float vertical = 0;
            if (obj.target.inputManager.jumpHeld) {
                vertical += 1;
            }
            if (obj.target.inputManager.pullButtonHeld) {
                vertical -= 1;
            }




            obj.target.transform.position += (((obj.target.cam.transform.forward * obj.target.inputManager.vertical) + 
                                             (obj.target.cam.transform.right * obj.target.inputManager.horizontal) +
                                             (obj.target.cam.transform.up * vertical)) * obj.target.moveSpeed * Time.deltaTime);
           // obj.target.transform.eulerAngles += (new Vector3(-obj.target.inputManager.threeDCamVertical, obj.target.inputManager.threeDCamHorizontal, 0 ) * obj.target.rotateSpeed * Time.deltaTime);
        }



    }


}
