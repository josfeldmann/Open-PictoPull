using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class CameraPosition {

    public Vector3 offset = new Vector3(0,0,-9);
    public float yEulersAngle = 180;

    public CameraPosition(Vector3 offset, float yEulersAngle) {
        this.offset = offset;
        this.yEulersAngle = yEulersAngle;
    }
}


public class CameraPositionSetter : MonoBehaviour {

    public Cinemachine.CinemachineVirtualCamera vcam;
    public CinemachineTransposer transposer;
    public PlayerController player;


    public List<Vector3> vector3s = new List<Vector3>() { new Vector3(0, 0, -9), new Vector3(9, 0, 0), new Vector3(0, 0, 9), new Vector3(-9, 0, 0) };

    public Quaternion targetPosition;
    public Vector3 targetOffset;
    public float timeToCameraTransition = 1f;
    public float minYSpeed = 180f;
    public float camSpeed;
    public int camIndex = 0;
    public float camMoveSpeed = 13f;
    public bool rotating;

    Vector2 twitchOffset = new Vector2();
    public float maxXTwitch, maxYTwitch;
    public float twitchSpeed = 5f;

    private void Update() {
        //  if ( targetPosition != vcam.transform.rotation) vcam.transform.rotation = Quaternion.RotateTowards(vcam.transform.rotation, targetPosition, camSpeed * Time.deltaTime);
        //  if (transposer != null && transposer.m_FollowOffset != targetOffset) transposer.m_FollowOffset = Vector3.MoveTowards(transposer.m_FollowOffset, targetOffset, camMoveSpeed * Time.deltaTime);

        if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {

            if (player != null && player.machine != null && player.machine.currentState is PlayerIdleState && (player.inputManager.camHorizontal != 0 || player.inputManager.camVertical != 0)) {

                twitchOffset = new Vector2(player.inputManager.camHorizontal * maxXTwitch, player.inputManager.camVertical * maxYTwitch);
                if (twitchOffset.y < 0) twitchOffset.y = 0;
                if (transposer == null) transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                transposer.m_FollowOffset = Vector3.MoveTowards(transposer.m_FollowOffset, new Vector3(twitchOffset.x, twitchOffset.y, transposer.m_FollowOffset.z), twitchSpeed * Time.deltaTime);
                transposer.m_FollowOffset = new Vector3(Mathf.Clamp(transposer.m_FollowOffset.x, -maxXTwitch, maxXTwitch), Mathf.Clamp(transposer.m_FollowOffset.y, -maxYTwitch, maxYTwitch), transposer.m_FollowOffset.z);

            } else if (transposer != null && (transposer.m_FollowOffset.x != 0 || transposer.m_FollowOffset.y != 0)) {
                if (transposer == null) {
                    Debug.Log("Trigger " + Time.time);
                    transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                }
                twitchOffset = new Vector3(0, 0, transposer.m_FollowOffset.z);
                transposer.m_FollowOffset = Vector3.MoveTowards(transposer.m_FollowOffset, new Vector3(twitchOffset.x, twitchOffset.y, transposer.m_FollowOffset.z), twitchSpeed * Time.deltaTime);
                transposer.m_FollowOffset = new Vector3(Mathf.Clamp(transposer.m_FollowOffset.x, -maxXTwitch, maxXTwitch), Mathf.Clamp(transposer.m_FollowOffset.y, -maxYTwitch, maxYTwitch), transposer.m_FollowOffset.z);

            }
        } else if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            
            if (player.inputManager.camHorizontal != 0 && Time.time > lastTime) {
                lastTime = Time.time + 0.25f;

                if (player.inputManager.camHorizontal > 0) {
                    SetCrashIndex(camIndex + 1);
                } else {
                    SetCrashIndex(camIndex - 1);
                }

            }


        }

    }


    public void SetCrashIndex(int i) {
        camIndex = i;

        if (camIndex < 0) {
            camIndex = vector3s.Count - 1;
        }
        if (camIndex >= vector3s.Count) {
            camIndex = 0;
        }

        if (transposer == null) {
            transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        }
        transposer.m_FollowOffset = vector3s[camIndex];
    }

    float lastTime;


    public void SetDefaultIndex() {
        camIndex = 0;
        //SetCameraPosition(pullPosition,true, 0);
        //Debug.Log("RESET CAMERA " + Time.time);
        if (transposer == null) transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        twitchOffset = new Vector3(0, 0, transposer.m_FollowOffset.z);
        transposer.m_FollowOffset = new Vector3(0, 0, -9);
        vcam.transform.rotation = Quaternion.identity;
        vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = 0;
    }


    public void MoveIndex(int direction) {
     //   camIndex += direction;
      //  while (camIndex < 0) {
           // camIndex += 4;
       // }
       // while (camIndex >= positions.Length) {
        //    camIndex -= positions.Length;
       // }
       // SetCameraPosition(positions[camIndex], false, direction);
    }

    private void SetCameraPosition(CameraPosition p, bool instant, float direction) {


      //  transposer = vcam.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        
        
        //if (instant) {
            //vcam.transform.rotation = targetPosition;
           // transposer.m_FollowOffset = p.offset;
      //  } else {
            // camSpeed = Mathf.Max(minYSpeed, Mathf.Abs(p.yEulersAngle - vcam.transform.eulerAngles.y));
            //  targetPosition = Quaternion.Euler(vcam.transform.localEulerAngles.x, p.yEulersAngle, vcam.transform.localEulerAngles.z);
            // targetOffset = p.offset;
         //  rotating = true;
           // StartCoroutine(RotateCamera(p, direction));
       // }
        

    }

    IEnumerator RotateCamera(CameraPosition p, float direction) {

        rotating = true;

        float stopTime = Time.time + 0.25f;
        transposer.enabled = false;
        float rotamount = 90;
        if (direction == 1) {
            rotamount = -90f;
        } else if (direction == -1) {
            rotamount = 90f;
        } else if (direction == -2) {
            rotamount = -180;
        } else if (direction == 2) {
            rotamount = 180;
        }

        while (stopTime > Time.time) {
            yield return null;
            vcam.transform.RotateAround(player.transform.position, Vector3.up, (rotamount * Time.deltaTime) / 0.25f);
        }

        transposer.m_FollowOffset = p.offset;
        vcam.transform.localEulerAngles = new Vector3(vcam.transform.eulerAngles.x, p.yEulersAngle, vcam.transform.eulerAngles.z);
        transposer.enabled = true;

        //transposer.enabled = true;

        rotating = false;

    }


}
