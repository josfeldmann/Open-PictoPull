using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrailerCutscene : MonoBehaviour
{
    public List<MeshRenderer> frontMeshes = new List<MeshRenderer>();
    
    public List<Color> c = new List<Color>();
    public Transform blocks;
    public float raiseSpeed = 11f;
    public float blockMoveSpeed = 3f;
    public float waitTime = 0.5f;
    public Animator player;
    public Transform playerStart, playerPull, playerPause;
    public float playerMoveSpeed = 4f;
    public float RotateSpeed = 360f;

    void Awake() {
        Cursor.visible = false;
        // blocks.transform.position = new Vector3(0, -11, 0);

        for (int i = 0; i < frontMeshes.Count; i++) {
            MeshRenderer frontMeshRenderer = frontMeshes[i];
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            frontMeshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", c[i]);
            propBlock.SetColor("_CrossColor", c[i]);
            frontMeshRenderer.SetPropertyBlock(propBlock);
        }

    }

    public void Update() {
        if (Keyboard.current.kKey.wasPressedThisFrame) {
            StartCoroutine(Trailer());
        }

        if (Keyboard.current.hKey.wasPressedThisFrame) {
            player.gameObject.SetActive(false);
        }

        if (Keyboard.current.jKey.wasPressedThisFrame) {
            blocks.gameObject.SetActive(false);
        }


    }

    public AudioSource bgMusic;





    IEnumerator Trailer() {

        bgMusic.Play();
        player.Play("Idle");
        
        blocks.transform.position = new Vector3(0, 0, 0);

        player.transform.position = playerPause.transform.position;
        player.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(.5f);

        player.SetBool("Walking", true);
        //  while (player.transform.position != playerPause.transform.position) {
        //  player.transform.position = Vector3.MoveTowards(player.transform.position, playerPause.transform.position, playerMoveSpeed * Time.deltaTime);
        //  yield return null;
        // }
        // player.SetBool("Walking", false);

        player.transform.rotation = Quaternion.identity;
        Quaternion target = Quaternion.Euler(0, 180, 0);

        while (player.transform.rotation != target) {
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, target, RotateSpeed * Time.deltaTime);
            yield return null;
        }


        yield return null;

      //  while (blocks.transform.position != Vector3.zero) {
          //  blocks.transform.position = Vector3.MoveTowards(blocks.transform.position, Vector3.zero, raiseSpeed * Time.deltaTime);
         //   yield return null;
     //   }

       

        player.SetBool("Walking", true);
        while (player.transform.position != playerPull.transform.position) {
          
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerPull.transform.position, playerMoveSpeed * Time.deltaTime);

            yield return null;
        }
        player.SetBool("Walking", false);

        player.SetTrigger("Pushing");
        yield return new WaitForSeconds(waitTime);

        float z = 0;
        float ztarget = -3;
       
        while ( z != ztarget) {

            //foreach (MeshRenderer r in frontMeshes) {
            //  r.transform.parent.transform.localPosition = new Vector3(r.transform.parent.transform.localPosition.x, r.transform.parent.transform.localPosition.y, z);
            //}
            blocks.transform.position = new Vector3(0, 0, z);

            z = Mathf.MoveTowards(z, ztarget, blockMoveSpeed * Time.deltaTime);
            player.transform.position += new Vector3(0, 0, -blockMoveSpeed * Time.deltaTime);
            yield return null;

        }
        blocks.transform.position = new Vector3(0, 0, z);

        player.SetTrigger("ReturnIdle");

        target = player.transform.rotation * Quaternion.Euler(0, 180, 0);

        while (player.transform.rotation != target) {
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, target, RotateSpeed * Time.deltaTime);
           // player.transform.position = Vector3.MoveTowards(player.transform.position, playerPull.transform.position, playerMoveSpeed * Time.deltaTime);

            yield return null;
        }

        player.Play("Dance");

        yield return new WaitForSeconds(3f);

        player.Play("Idle");



    }


}
