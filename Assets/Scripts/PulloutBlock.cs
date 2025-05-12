using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulloutBlock : MonoBehaviour
{
    public const float moveSpeed = 1f;
    public MeshRenderer mRendererBase;
    public MeshFilter mRendererBaseFilter, mRendererButtonFilter;
    public SpriteRenderer spriteIcon;
    public LayerMask playerLayer;
    public BlockLevelGenerator generator;
    public int index;
    private Block block;
    public Color color;
    public Transform upPosition;
    public Transform downPosition;
    public Transform buttonBaseParent;
    public Mesh singleButtonMesh;
    public Mesh singleButtonBaseMesh;
    Vector3Int upCoord;
    public Vector3Int position;

    public Material hideMat;
    public Material noHideMat;
    public List<MeshRenderer> renderers;

    public void SetColor(int i, Block b, Color c, BlockLevelGenerator gen, Vector3Int c1) {
        index = i;
        block = b;
        color = c;
        position = c1;
        upCoord = c1  + Vector3Int.up;
        spriteIcon.color = color;
        if (direction == BlockDirection.FORWARD) {
            direction = BlockDirection.DOWN;
        }
        if (direction == BlockDirection.BACKWARD) {
            direction = BlockDirection.UP;
        }
        if (single) {
            spriteIcon.sprite = CursorObject.instance.pulloutSingle;
            mRendererBaseFilter.mesh = singleButtonMesh;
            mRendererButtonFilter.mesh = singleButtonBaseMesh;
        } else {
            spriteIcon.sprite = CursorObject.instance.pullOutFull;
        }
        generator = gen;
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        mRendererBase.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color);
        mRendererBase.SetPropertyBlock(propBlock);

        if (direction == BlockDirection.UP) {
            buttonBaseParent.Rotate(0, 180, 0);
            spriteIcon.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (direction == BlockDirection.RIGHT) {
            buttonBaseParent.Rotate(0, 270, 0);
            spriteIcon.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == BlockDirection.LEFT) {
            buttonBaseParent.Rotate(0, 90, 0);
            spriteIcon.transform.rotation = Quaternion.Euler(0, 0, 270);
        }


        Material mat = hideMat;
        if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            mat = noHideMat;
        }
        foreach (MeshRenderer rend in renderers) {
            rend.material = mat;
        }



    }


    public bool canBePressed = true;
    public bool isCurrentlySteppedOn = false;
    internal bool single;
    public BlockDirection direction;

    private void Update() {




        if ((isCurrentlySteppedOn || !canBePressed) && mRendererBase.transform.position != downPosition.transform.position) {
            mRendererBase.transform.position = Vector3.MoveTowards(mRendererBase.transform.position, downPosition.position, moveSpeed * Time.deltaTime);
        } else if (!isCurrentlySteppedOn && canBePressed && mRendererBase.transform.position != upPosition.transform.position)  {
            mRendererBase.transform.position = Vector3.MoveTowards(mRendererBase.transform.position, upPosition.position, moveSpeed * Time.deltaTime);
        }
    }

    bool doneYet = false;
    public static float bufferTime;

    

    private void OnTriggerEnter(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, playerLayer) && canBePressed) {
            if (PlayerController.instance.machine.currentState is TimeRewindState || PlayerController.instance.machine.currentState is CrashRewindState) return;

            if (block.leveltype == LevelType.PULL) {
                isCurrentlySteppedOn = true;
                
                if ((block.colorIndex == index || (block.generator.coordToBlock.ContainsKey(upCoord) && block.generator.coordToBlock[upCoord].colorIndex == index && block.currentPullLevel == block.generator.coordToBlock[upCoord].currentPullLevel + 1)) && PlayerController.instance.machine.currentState is PlayerIdleState) {
                    //  Debug.Log("Here");
                    PlayerController.instance.SetIdleGroundedAnimation();
                    PlayerController.instance.MoveWithBlock(block, direction);
                }

                Block blockPlayerPulling = null;

                if (GameMasterManager.instance.playerController.machine.currentState is PlayerPushState) {
                    PlayerPushState p = (PlayerPushState)GameMasterManager.instance.playerController.machine.currentState;
                    print(index + " " + p.block.colorIndex + " " + Time.time);
                    if (p.block.colorIndex == index) {
                        blockPlayerPulling = p.block;
                    }
                }

                if (direction == BlockDirection.DOWN && (block.generator.coordToBlock.ContainsKey(upCoord) && block.generator.coordToBlock[upCoord].colorIndex == index)) {

                    Vector3Int downCoord = upCoord - new Vector3Int(0, 2,0);
                    bool abort = false;
                    if ((block.generator.coordToBlock.ContainsKey(downCoord) && (block.generator.coordToBlock[downCoord].colorIndex == index || block.generator.coordToBlock[downCoord].colorIndex == block.colorIndex))) {
                        abort = true;
                    }

                    if (!abort) {
                    //    Debug.Log("Pull Down " + Time.time);
                        PlayerController.instance.EnterPulloutFallState(block.generator.coordToBlock[upCoord]);
                    }
                }


                if (single) {
                    generator.MoveAllBlocksOfIndexInDirection(index, direction);
                    AddPulloutStat();
                } else {
                    generator.PullOutAllBlocksOfIndex(index, direction);
                    AddPulloutStat();
                }
                GameMasterManager.instance.playerController.StopGrabbingBlockIfHoldingBlockOfIndex(index);

            } else if (block.leveltype == LevelType.CRASH) {
                if (isCurrentlySteppedOn) return;
                
                isCurrentlySteppedOn = true;
                if (GameMasterManager.instance.playerController.InBlocksMoving()) {
                  //  Debug.LogError("In Crash Move??" + Time.time);
                    return;
                }
                Vector3Int vec = LadderBlock.DirectionToVector(direction);
                Vector3Int v3 = new Vector3Int(vec.x, 0, vec.y);
                List<Block> list = new List<Block>();

               // Debug.Log("Here???? " + Time.time);


                if (block.CanMoveCrashBlockInThisDirection(v3, list, true)) {
                 //   Debug.LogError("DO PUSH " + v3.ToString() + " " + Time.time);
                    bool playerMove = false;
                    if (list.Contains(block)) playerMove = true;
                    foreach (Block b in otherBlocks) {
                        if (!list.Contains(b)) {
                            list.Add(b);
                        }
                    }
                    PlayerController.instance.machine.ChangeState(new PushCrashState(list, v3, new PlayerIdleState(), playerMove, false));
                } else {
                   // Debug.LogError("COULDNT?? " + Time.time);
                }
            }
        }
    }


    public void AddPulloutStat() {
        GameMasterManager.gameSaveFile.trackerSaveFile.AddPullouts(1);
    }

    public static float pulloutcooldown;

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            isCurrentlySteppedOn = false;
           
        }
    }

    List<Block> otherBlocks = new List<Block>();
    bool blockedByOtherBlock = false;
    public Vector3Int GetPosition() {
        return new Vector3Int((int)transform.parent.localPosition.x, (int)transform.parent.localPosition.y, (int)transform.parent.localPosition.z)
            + position;
    }

    public void PressCheck() {
        canBePressed = false;
    
        if (block.leveltype == LevelType.PULL) {
            if (block.currentPullLevel == 0) return;
            foreach (Block b in generator.blocks) {
                if (b.colorIndex == index) {
                    if (direction == BlockDirection.DOWN) {
                        if (b.currentPullLevel < generator.pullLevelDepth) {
                            canBePressed = true;
                        }
                    } else {
                        if (b.currentPullLevel > 0) {
                            canBePressed = true;
                        }
                    }
                }
            }
        } else if (block.leveltype == LevelType.CRASH) {
            otherBlocks = new();
            blockedByOtherBlock = false;
            Vector3Int dir = new Vector3Int(LadderBlock.DirectionToVector(direction).x, 0, LadderBlock.DirectionToVector(direction).y);

            Vector3Int opos = GetPosition() + Vector3Int.up + dir;

            if (GameMasterManager.instance.generator.isVector3IntInCrashRange(opos)) {
                Block b = GameMasterManager.instance.generator.crashGrid[opos.x, opos.y, opos.z];
                if (b != null && b != block) {
                    otherBlocks.Add(b);
                    Debug.Log("Other Block Detected " + Time.time);
                    if (!b.CanMoveCrashBlockInThisDirection(dir, new List<Block>(), true)) {
                        blockedByOtherBlock = true;
                    }
                }
            }

            if (block.CanMoveCrashBlockInThisDirection(dir, new List<Block>(), true) && !blockedByOtherBlock) {
                canBePressed = true;
            }
        }
    }

}
