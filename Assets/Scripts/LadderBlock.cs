using System;
using System.Collections.Generic;
using UnityEngine;


public class LadderBlock : MonoBehaviour {

    public static Dictionary<BlockDirection, LadderBlockPosition> dirPos = new Dictionary<BlockDirection, LadderBlockPosition> {

        { BlockDirection.UP, new LadderBlockPosition(new Vector3(0,0.01f,0), Vector3.zero) },
        { BlockDirection.DOWN, new LadderBlockPosition(new Vector3(0,-1.01f,0), new Vector3(0,0,180)) },
        { BlockDirection.LEFT, new LadderBlockPosition(new Vector3(-0.51f,-0.5f,0), new Vector3(0,0,90))},
        { BlockDirection.RIGHT, new LadderBlockPosition(new Vector3(0.51f,-0.5f,0), new Vector3(0,0,-90))},
        { BlockDirection.FORWARD, new LadderBlockPosition(new Vector3(0,-0.5f,-0.55f), new Vector3(-90,0,0))},
        { BlockDirection.BACKWARD, new LadderBlockPosition(new Vector3(0,-0.5f,0.55f), new Vector3(90,0,0))}




    };


    public SpriteRenderer spriteIcon, leftIcon, rightIcon, upIcon, downIcon;
    public BoxCollider rightCol, leftCol, upCol, downCol, forwardCol, backCol;
    public SpriteRenderer[] sprites = new SpriteRenderer[0];
    public SpriteRenderer portalBase;
    public LayerMask playerLayer;
    public BlockLevelGenerator generator;
    private int index;
    private Block block;
    private Color color;
    public LadderBlock otherLadderBlock;
    private Vector2Int gridPos;
    public Transform portalTransform;
    
    public BlockDirection blockdir;
    public void SetColor(int i, Block b, Color c, BlockLevelGenerator gen, Vector2Int vec, BlockDirection dir) {
        index = i;
        block = b;
        color = c;
        spriteIcon.color = color;
        generator = gen;
        gridPos = vec;
        SetColor(c);

        LadderBlockPosition l = dirPos[dir];
        portalTransform.localPosition = l.portalPosition;
        portalTransform.localRotation = Quaternion.Euler(l.portalRotation);
        upIcon.color = color;
        downIcon.color = color;
        leftIcon.color = color;
        rightIcon.color = color;
        blockdir = dir;
        HideAll();
        switch (dir) {
            case BlockDirection.UP:
                upIcon.enabled = true;
                upCol.enabled = true;
                break;
            case BlockDirection.DOWN:
                downIcon.enabled = true;
                downCol.enabled = true;
                break;
            case BlockDirection.LEFT:
                leftIcon.enabled = true;
                leftCol.enabled = true;
                break;
            case BlockDirection.RIGHT:
                rightIcon.enabled = true;
                rightCol.enabled = true;
                break;
            case BlockDirection.FORWARD:
                forwardCol.enabled = true;
                break;
            case BlockDirection.BACKWARD:
                backCol.enabled = true;
                break;
        }


    }

    public void HideAll() {
        leftIcon.enabled = false;
        rightIcon.enabled = false;
        upIcon.enabled = false;
        downIcon.enabled = false;
        upCol.enabled = false;
        downCol.enabled = false;
        rightCol.enabled = false;
        leftCol.enabled = false;
        forwardCol.enabled = false;
        backCol.enabled = false;

    }

    public bool ShouldBeShowing;
    public float currentScale;
    public static Vector3 rotSpeed = new Vector3(0, 180f, 0);
    public static float HideSpeed = 5f;


    public void ImmediatelyRetract() {
        ShouldBeShowing = false;
        currentScale = 0;
        SetScale();
    }

    public void Show() {
        ShouldBeShowing = true;
    }

    public void Hide() {
        ShouldBeShowing = false;
    }

    public void SetScale() {
        foreach (SpriteRenderer s in sprites) {
            s.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
    }

    public void SetColor(Color c) {
        portalBase.color = c;
        foreach (SpriteRenderer sr in sprites) {
            sr.color = c;
        }
    }

    public void SetOtherLadderBlock() {
        foreach (LadderBlock b in generator.ladders) {
            if (b.index == index && b != this) {
                otherLadderBlock = b;
                return;
            }
        }
    }



    public void Update() {
        if (block.leveltype == LevelType.PULL && block.currentPullLevel <= 0 ) portalTransform.gameObject.SetActive(false);
        if (ShouldBeShowing && currentScale != 1) {
            currentScale = Mathf.MoveTowards(currentScale, 1, HideSpeed * Time.deltaTime);
            SetScale();
        }
        if (!ShouldBeShowing && currentScale != 0) {
            currentScale = Mathf.MoveTowards(currentScale, 0, HideSpeed * Time.deltaTime);
            SetScale();
        }
        if (ShouldBeShowing) {
            portalTransform.Rotate(rotSpeed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other) {
      //  Debug.Log(other.gameObject.name);
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
           if (!PlayerController.ladderBlocks.Contains(this)) {
                PlayerController.AddLadder(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            if (PlayerController.ladderBlocks.Contains(this)) {
                PlayerController.RemoveLadder(this);
            }
        }
    }

    public Vector3Int GetCrashPos() {
        Vector3Int v = new Vector3Int((int)block.transform.localPosition.x, (int)block.transform.localPosition.y, (int)block.transform.localPosition.z);
        Vector3Int v2 = new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y-1, (int)transform.localPosition.z);
        return v + v2;
    }

    public void LadderCheck() {

        if (block.leveltype == LevelType.CRASH) {
            
            portalTransform.gameObject.SetActive(true);
            Vector3Int dirr = DirectionToVector(blockdir);
            Vector3Int pos =  GetCrashPos() + dirr;
            
           // Debug.Log("Checking " + pos.ToString() + " " + Time.time);
            if ( pos.y >= 0 && GameMasterManager.instance.generator.GetCrashBlock(pos) != null) {
                Hide();
            } else {
                if (true
                    //(blockdir != BlockDirection.FORWARD && blockdir != BlockDirection.BACKWARD) 
                    //|| pos.y == 0 
                    //|| GameMasterManager.instance.generator.GetCrashBlock(pos - Vector3Int.up) != null 
                    ) {
                    Show();
                } else {
                    Hide();
                }
            }
            return;
        }

        if (block.currentPullLevel <= 0) {
            ImmediatelyRetract();
            return;
        } else if (portalTransform.gameObject.activeInHierarchy == false) {
            portalTransform.gameObject.SetActive(true);
        }

         

        Block b = generator.GetDirectionBlock(new Vector3Int(gridPos.x, gridPos.y), DirectionToVector(blockdir));
        if (b == null) {
            Show();
        } else if (b != null && b.currentPullLevel < block.currentPullLevel) {
            Show();
        } else if (ShouldBeShowing) {
            ImmediatelyRetract();
        }
    }

    public void OtherLadderCheck() {
        
        if (ShouldBeShowing && ( otherLadderBlock == null || otherLadderBlock.ShouldBeShowing == false)) {
            Hide();
        }
    }


    public static Vector3Int DirectionToVector(BlockDirection d) {
        switch (d) {
            case BlockDirection.UP:
                return Vector3Int.up;
             
            case BlockDirection.DOWN:
                return Vector3Int.down;
               
            case BlockDirection.LEFT:
                return Vector3Int.left;
               
            case BlockDirection.RIGHT:
                return Vector3Int.right;
               
            case BlockDirection.FORWARD:
                return Vector3Int.back;
            
            case BlockDirection.BACKWARD:
                return Vector3Int.forward;
             
        }
        return Vector3Int.up;
    }

    internal Vector3 GetWarpToPos() {
        switch (blockdir) {
            case BlockDirection.UP:
                return transform.position + new Vector3(0, 0.01f, 0);

            case BlockDirection.DOWN:
                return transform.position + new Vector3(0, -0.5f, 0);

            case BlockDirection.LEFT:
                return transform.position + new Vector3(-0.5f, -0.5f, 0);

            case BlockDirection.RIGHT:
                return transform.position + new Vector3(+0.5f, -0.5f, 0);
            case BlockDirection.FORWARD:
                return transform.position + new Vector3(0, -0.5f, -0.5f);
                
            case BlockDirection.BACKWARD:
                return transform.position + new Vector3(0, -0.5f, 0.5f);

        }
        return transform.position + new Vector3(0, 0.01f, 0);
    }


    public Vector3 GetWarpPlacementPosition() {
        switch (blockdir) {
            case BlockDirection.UP:
                return transform.position + new Vector3(0, 0.01f, 0);

            case BlockDirection.DOWN:
                return transform.position + new Vector3(0f, -2f, 0);

            case BlockDirection.LEFT:
                return transform.position + new Vector3(-1f, -0.5f, 0);

            case BlockDirection.RIGHT:
                return transform.position + new Vector3(1f, -0.5f, 0);
            case BlockDirection.FORWARD:
                return transform.position + new Vector3(0, -0.5f, -1f);
               
            case BlockDirection.BACKWARD:
                return transform.position + new Vector3(0, -0.5f, 1f);
        }
        return transform.position + new Vector3(0, 0.01f, 0);
    }
}
