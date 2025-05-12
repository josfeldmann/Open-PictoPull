using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable]
public enum LevelType { PULL = 0, CRASH = 1, STRETCH = 2 }


[System.Serializable]
public enum BlockDirection {  UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, FORWARD = 4, BACKWARD = 5}


public class BlockLevelGenerator : MonoBehaviour {
    public BlockSaveFile save;
    public Block blockPrefab;
    public Material pullevelmaterial, pullBackMaterial, crossColorMat, crashlevelmaterial, allUnionMaterial, positiveOppoMat, negativeOppoMat, timerMat, cloudMaterial;
    public Transform platform;
    public Goal goal;

   

    public static bool isCustomLevel = false;

    public static BlockSaveFile lastSelected;

    public static LevelType currentLevelType = LevelType.PULL;
    public static float evenoffset = 0.5f;

    [Header("Player")]
    public PlayerController player;
    public ResetButton resetButton;
    public Transform playerStartPostion;
    public TextMeshProUGUI levelName;

    

    [Header("Objects")]
    public Block goalBlock;
    public SpriteRenderer coreCenterSprite, coreBorderSprite;
    public MeshRenderer coreRenderer;
    public List<Block> blocks = new List<Block>();
    public Block[,] blockGrid = new Block[0, 0];
    public PulloutBlock pullOutBlock;
    public List<PulloutBlock> pullOuts = new List<PulloutBlock>();
    public LadderBlock ladderBlock;
    public List<LadderBlock> ladders = new List<LadderBlock>();
    public SubGoalBlock subGoalPrefab;
    public List<SubGoalBlock> subGoals = new List<SubGoalBlock>();
    public DirectionalCannon directionalCannon;
    public List<DirectionalCannon> cannons = new List<DirectionalCannon>();
    public TimerBlock timerBlock;
    public UnionTag unionTag;
    public BlockerTag blockerTag;
    public List<IndicatorColorTag> tags = new List<IndicatorColorTag>();
    public GameObject cameraViewAnchor;
    public CinemachineVirtualCamera levelViewCam;
    public bool firstBlockPulledOutYet = false;

    public bool HasSubGoals() {
        return subGoals.Count > 0;
    }


    [Header("Crash Block ")]
    public Transform CrashBlockParent, CrashBlockParentGoHere;
    public Block[,,] crashGrid; 

    [Header("Low Poly Stage")]
    public SideTileHolder rightSide;
    public SideTileHolder leftSide;
    public Transform blockParent;
    public TileMesh blockTilePrefab;
    public ArrowIndicator arrowIndicator;
    public MeshRenderer blockNode;
    public MeshRenderer gridNode; 
    
    //TileMesh[,] blockTiles = new TileMesh[0, 0];
  //  public static List<TileMesh> frontWallMeshes = new List<TileMesh>();
   // public static List<TileMesh> backWallMeshes = new List<TileMesh>();
  //  public static List<TileMesh> wallMeshes = new List<TileMesh>();
 //   public static List<TileMesh> topMeshes = new List<TileMesh>();
  //  public TileMesh wallTileprefab;
    public BoxCollider inGameBoxCollider;


    [Header("TopMesh")]
    public Mesh normalTopMesh;
    public Mesh grid1Mesh, grid2Mesh;

    public GameObject DebugCrashPrefab;
    public List<GameObject> Debuggers = new List<GameObject>();
    public Vector3Int index;
    public Material backBoardMaterial;
    public Block backBoardBlock;
    public List<Sprite> boxSprites = new List<Sprite>();

    int xmax, ymax, zmax;
    Vector3 spawnSpot;
    int sideAmount;
    int sideOffset = 5;

    public int zDepthStage;
    public int pullLevelDepth = 3;


    //OppositeBlocks
    public int oppositePullLevel = 0;
    public List<Block> positiveBlocks = new List<Block>();
    public List<Block> negativeBlocks = new List<Block>();

    public StringCutscene stringCutscene;
    public GameObject hintIcon;





    private void Update() {
#if !UNITY_SWITCH
        if (Keyboard.current.kKey.wasPressedThisFrame) {
            ShowDebugThings(this);
        }
#endif
    }

    [Header("Time Manager")]
    public TimeController timeController;

    public void Setup() { 
        Block.guidingSprites = boxSprites;
        arrowIndicator.Hide();
        GenerateInitialBlocks();
    }

    public void SetTileMats() {
        //foreach (TileMesh t in wallMeshes) {
        //    t.SetMaterial(GameMasterManager.envLevelGroup.envGroup.mat) ;
      //  }
               
       
        //foreach (TileMesh t in topMeshes) {
        //    t.SetMaterial(GameMasterManager.currentLevelGroup.topMat);
       // }
      //  foreach (TileMesh t in topMeshes) {
       //     t.SetMaterial(GameMasterManager.envLevelGroup.envGroup.mat);
      //  }
       
    }

   

    public void GenerateInitialBlocks() {
       /* blockTiles = new TileMesh[40, 9];
        frontWallMeshes = new List<TileMesh>();
        backWallMeshes = new  List<TileMesh>();
        for (int x = 0; x < 40; x++) {
            for (int y = 0; y < 9; y++) {
                TileMesh g = Instantiate(blockTilePrefab, blockParent);
                g.transform.localPosition = new Vector3(x + 1, 0, y);
                blockTiles[x, y] = g;
                g.gameObject.SetActive(false);
                g.SetIndex(new Vector2Int(x, y));
                
            }
            TileMesh wall = Instantiate(wallTileprefab, blockParent);
            wall.transform.eulerAngles = new Vector3(0, 180, 0);
            wall.transform.localScale = new Vector3(1f, 1f, 1f);
            wall.transform.localPosition = new Vector3(x + 0.5f,-0.5f, 0);
            wall.gameObject.SetActive(false);
            frontWallMeshes.Add(wall);

            TileMesh backWall = Instantiate(wallTileprefab, blockParent);
            backWall.transform.eulerAngles = new Vector3();
            backWall.transform.localScale = Vector3.one;
            backWall.transform.localPosition = new Vector3(x + 0.5f, -0.5f, 9);
            backWall.gameObject.SetActive(false);
            backWallMeshes.Add(backWall);

        
        }*/
    }

    public Block GetFirstBlockOfIndex(int v) {
        foreach (Block b in blocks) {
            if (b.colorIndex == v) {
                return b;
            }
        }
        return null;
    }

    public Block GetBlockNumber(int i) {
        return blocks[i];
    }
    public LadderBlock GetLadder(int v) {
        return ladders[v];
    }

    public void SetLowPolyStage() {
        float offset = 0;



        if (save.width % 2 == 0) offset = evenoffset;


         rightSide.transform.position = new Vector3(((float)save.width + sideAmount)/2 + offset - 1, rightSide.transform.position.y, rightSide.transform.position.z);
         leftSide.transform.position = new Vector3(-((float)save.width + sideAmount)/2 + offset, leftSide.transform.position.y, leftSide.transform.position.z);
        /*
        for (int x = 0; x < blockTiles.GetLength(0); x++) {
            for (int y = 0; y < blockTiles.GetLength(1); y++) {
                if (x < save.width + (sideOffset * 2) && y < zDepthStage) {
                    blockTiles[x, y].gameObject.SetActive(true);

                    if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
                        if (y > 2) {
                            if (((x + y) % 2) == 1) {
                                blockTiles[x, y].SetMesh(grid1Mesh);
                            } else {
                                blockTiles[x, y].SetMesh(grid2Mesh);
                            }
                        } else {
                            blockTiles[x, y].SetMesh(normalTopMesh);
                        }
                    } else {
                        if (y > 0 && y < (zDepthStage-1)) {
                            if (((x + y) % 2) == 1) {
                                blockTiles[x, y].SetMesh(grid1Mesh);
                            } else {
                                blockTiles[x, y].SetMesh(grid2Mesh);
                            }
                        } else {
                            blockTiles[x, y].SetMesh(normalTopMesh);
                        }
                    }


                } else {
                    blockTiles[x, y].gameObject.SetActive(false);
                }
            }
             
            if (x < save.width + (sideOffset * 2)) {
                frontWallMeshes[x].gameObject.SetActive(true);
                backWallMeshes[x].gameObject.SetActive(true);
                backWallMeshes[x].gameObject.transform.localPosition = new Vector3(backWallMeshes[x].transform.localPosition.x, backWallMeshes[x].transform.localPosition.y, zDepthStage);
            } else {
                frontWallMeshes[x].gameObject.SetActive(false);
                backWallMeshes[x].gameObject.SetActive(false);
            }
        }
        */
        if (save.levelType == LevelType.PULL) {
            blockNode.transform.localScale = new Vector3(save.width + 4, 1, zDepthStage);
            blockNode.transform.position = Vector3.zero + (Vector3.right * offset) + (-Vector3.forward * blockNode.transform.localScale.z / 2) + (Vector3.up * -0.5f);
            blockNode.material = GameMasterManager.envLevelGroup.envGroup.blockMaterial;

            gridNode.transform.localScale = new Vector3(save.width + 2, 1, pullLevelDepth);
            gridNode.transform.position = new Vector3(0, 0.01f, gridNode.transform.localScale.z / -2) + (Vector3.right * offset);
            gridNode.material = GameMasterManager.envLevelGroup.envGroup.gridMaterial;
            rightSide.SetRange(zDepthStage);
            leftSide.SetRange(zDepthStage);


        } else {

            if (save.is3DLevel) {
                blockNode.transform.localScale = new Vector3(save.width + 14, 1, 10 + save.threeDMap.GetLength(2));
                blockNode.transform.position = (Vector3.right * offset);
                blockNode.transform.position = new Vector3(blockNode.transform.position.x, -0.5f, (blockNode.transform.localScale.z/2) - 7);
            } else {
                blockNode.transform.localScale = new Vector3(save.width + 14, 1, 11);
                blockNode.transform.position = Vector3.zero + (Vector3.right * offset) + (-Vector3.forward * blockNode.transform.localScale.z / 2) + (Vector3.up * -0.5f);
                blockNode.transform.position = new Vector3(blockNode.transform.position.x, blockNode.transform.position.y, -1);
            }


            
            blockNode.material = GameMasterManager.envLevelGroup.envGroup.blockMaterial;

            if (save.is3DLevel) {
                gridNode.transform.localScale = new Vector3(save.width + 10, 1, 6 + save.threeDMap.GetLength(2));
                gridNode.transform.position = new Vector3(blockNode.transform.position.x, gridNode.transform.position.y, blockNode.transform.position.z);
            } else {
                gridNode.transform.localScale = new Vector3(save.width + 10, 1, 7);
                gridNode.transform.position = new Vector3(0, 0.01f, gridNode.transform.localScale.z / -2) + (Vector3.right * offset);
                gridNode.transform.position = new Vector3(gridNode.transform.position.x, gridNode.transform.position.y, -1.5f);
            }

            
            gridNode.material = GameMasterManager.envLevelGroup.envGroup.gridMaterial;

        }



        offset = 0;
        if (save.width % 2 == 0) offset = evenoffset;
        blockParent.transform.position = new Vector3((((float)save.width + (sideOffset * 2)) / -2) + offset, blockParent.transform.position.y, blockParent.transform.position.z);
        //  cameraViewAnchor.transform.position = new Vector3(cameraViewAnchor.transform.position.x, (((float)save.height) / 2f), ((float) -save.height/2) - 2.5f);
          cameraViewAnchor.transform.position = new Vector3(cameraViewAnchor.transform.position.x, (((float)save.height) / 2f), 0);

        levelViewCam.m_Lens.OrthographicSize = (((float)save.height) / 2f) + 0.5f;

        if (save.width % 2 == 0) {
            Environment.instance.transform.position = new Vector3(0.5f, 0, -pullLevelDepth + 3);
        } else {
            Environment.instance.transform.position = new Vector3(0, 0, -pullLevelDepth+3);
        }
    }

  

    public void ResetAllBlocks() {
        if (TimeController.isReversing) return;
        oppositePullLevel = 0;
        if (currentLevelType == LevelType.PULL) {
            foreach (Block b in blocks) {
                b.ResetButtonPullLevel();
            }
            player.pushPullSound.Play();
            firstBlockPulledOutYet = false;
        } else if (currentLevelType == LevelType.CRASH) {
            foreach (Block b in blocks) {
                b.transform.localPosition = new Vector3(sideOffset, 0, 3 - b.crashDepthOffset);
            }
            FullySetCrashBlocks();
            foreach (PulloutBlock p in pullOuts) {
                p.PressCheck();
            }
        }

       
    }

    public void GoDownLadder(LadderBlock ladderBlock, LadderBlock otherLadderBlock) {
        Debug.LogError("HERE");
    }

    public Dictionary<Vector3Int, Block> coordToBlock = new Dictionary<Vector3Int, Block>();

    public void PullOutAllBlocksOfIndex(int i, BlockDirection direction) {
        foreach (Block b in blocks) {
            if (b.isOpposite) {

            } else if (b.colorIndex == i) {
                TimeController.AddBlockMove(b, b.currentPullLevel);
                if (direction == BlockDirection.DOWN) {
                    b.SetPullLevel(pullLevelDepth);
                    b.ImmediatelyUpdateHiddenMesh();
                } else {
                    b.SetPullLevel(0);
                }
            }
        }
        player.pushPullSound.Play();
    }

    public void MoveAllBlocksOfIndexInDirection(int index, BlockDirection direction) {
        int addamount = 1;

        if (direction == BlockDirection.UP) {
            addamount = -1;
        }
        foreach (Block b in blocks) {
            if (b.isOpposite) {

            } else if (b.colorIndex == index) {
                TimeController.AddBlockMove(b, b.currentPullLevel);
                b.SetPullLevel(Math.Clamp(b.currentPullLevel + addamount, 0, 3));
                if (direction == BlockDirection.DOWN) b.ImmediatelyUpdateHiddenMesh();
            }
        }
        player.pushPullSound.Play();

    }


    public Block GetDirectionBlock(Vector3Int start, Vector3Int direction) {
        start = start + direction;
        if (coordToBlock.ContainsKey(new Vector3Int(start.x, start.y,0))) {
            return coordToBlock[new Vector3Int(start.x, start.y,0)];
        } else {
            return null;
        }
    }

    public TutorialMaster textDisplayer;
    public GameObject tutorialNPC;
    public bool timerPresent;

    public void GeneratePullLevel() {
        List<Vector3Int> visited = new List<Vector3Int>();
      //  print(save.levelName + " " + save.colorGrid.Length);

        bool isOpposite = save.isOpposite();
        bool allUnion = save.isAllUnion();
        bool allTimer = save.isAllTimer();

        negativeBlocks = new List<Block>();
        positiveBlocks = new List<Block>();

        for (int x = 0; x < save.width; x++) {
            for (int y = 0; y < save.height; y++) {
                if (save.colorGrid[x, y] != 0 && !visited.Contains(new Vector3Int(x, y,0))) {
                    Block block = Instantiate(blockPrefab, spawnSpot, Quaternion.identity);
                    block.generator = this;
                    block.material = pullevelmaterial;
                    block.blockColor = save.GetColor(save.colorGrid[x, y] - 1);
                    block.VecList = new List<Vector3Int>();
                    block.VecList.Add(new Vector3Int(x, y,0));
                    int index = save.colorGrid[x, y];
                    block.colorIndex = index;
                    Queue<Vector3Int> bQueue = new Queue<Vector3Int>();
                    bQueue.Enqueue(new Vector3Int(x, y,0));
                    visited.Add(new Vector3Int(x, y,0));
                    while (bQueue.Count > 0) {

                        Vector3Int v = bQueue.Dequeue();
                        if (!coordToBlock.ContainsKey(v)) {
                            coordToBlock.Add(v, block);

                        }

                        if (v.x == save.goalSpot.x && v.y == save.goalSpot.y) {
                            block.hasGoal = true;
                            goalBlock = block;
                        }
                        if (v.x > 0 && !visited.Contains(v - Vector3Int.right) && save.colorGrid[v.x - 1, v.y] == index) {
                            visited.Add(v - Vector3Int.right);
                            block.VecList.Add(v - Vector3Int.right);
                            bQueue.Enqueue(v - Vector3Int.right);
                        }
                        if (v.x < xmax && !visited.Contains(v + Vector3Int.right) && save.colorGrid[v.x + 1, v.y] == index) {
                            visited.Add(v + Vector3Int.right);
                            block.VecList.Add(v + Vector3Int.right);
                            bQueue.Enqueue(v + Vector3Int.right);
                        }
                        if (v.y < ymax && !visited.Contains(v + Vector3Int.up) && save.colorGrid[v.x, v.y + 1] == index) {
                            visited.Add(v + Vector3Int.up);
                            block.VecList.Add(v + Vector3Int.up);
                            bQueue.Enqueue(v + Vector3Int.up);
                        }
                        if (v.y > 0 && !visited.Contains(v - Vector3Int.up) && save.colorGrid[v.x, v.y - 1] == index) {
                            visited.Add(v - Vector3Int.up);
                            block.VecList.Add(v - Vector3Int.up);
                            bQueue.Enqueue(v - Vector3Int.up);
                        }
                    }

                    

                    if (isOpposite && (save.oppositeInfo.positiveIndexes.Contains(block.colorIndex - 1) || save.oppositeInfo.negativeIndexes.Contains(block.colorIndex - 1))) {
                       
                        block.isOpposite = true;
                        if (save.oppositeInfo.positiveIndexes.Contains(block.colorIndex - 1)) {
                            //Debug.Break();
                            block.isPositiveOpposite = true;
                            positiveBlocks.Add(block);
                        } else {
                           // Debug.Break();
                            block.isPositiveOpposite = false;
                            negativeBlocks.Add(block);
                        }
                    }

                    if (allUnion) {
                        if (!block.isOpposite) {
                            block.isUnion = true;
                            block.unionIndexes.Add(block.colorIndex);
                            block.material = allUnionMaterial;
                        }
                    }

                    if (allTimer) {
                        if (!block.isOpposite && !block.isUnion) {
                            block.isTimer = true;
                            timerPresent = true;
                        }
                    }


                    blocks.Add(block);
                }
            }
        }

        if (goalBlock) {
            goal.transform.SetParent(goalBlock.transform);
            goal.transform.localPosition = new Vector3(save.goalSpot.x + 0.5f, save.goalSpot.y + 1, 0.5f);
            goal.attachedBlock = goalBlock;
            goal.touchablePart.transform.localScale = Vector3.zero;
        } else {
           // Debug.LogError("EEEEE");
        }

        for (int i = 0; i < blocks.Count; i++) {
            blocks[i].BuildChunk(blocks[i].VecList, save.width, save.height, save.depthGrid, save);
            blocks[i].SetPullLevelInstantly(0);
        }
     
        pullOuts = new List<PulloutBlock>();
        ladders = new List<LadderBlock>();
        subGoals = new List<SubGoalBlock>();
        cannons = new List<DirectionalCannon>();
        tags = new List<IndicatorColorTag>();
        
        if (save.specialTile != null && save.specialTile.Count > 0) {
            foreach (SpecialTile s in save.specialTile) {
                //  Debug.Log("SpecialTile");

                Vector3Int vv = new Vector3Int(s.position.x, s.position.y, 0);

                if (coordToBlock.ContainsKey(vv)) {

                    if (s.id == SpecialTile.PULLOUTBUTTON) {
                        //  Debug.Log("PullOutButton");
                        PulloutBlock p = Instantiate(pullOutBlock, coordToBlock[vv].transform);
                        if (s.subid == 0) {
                            p.single = false;
                        } else {
                            p.single = true;
                        }
                        p.direction = s.direction;
                        p.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, new Vector3Int(s.position.x, s.position.y,0));
                        p.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        pullOuts.Add(p);
                    } else if (s.id == SpecialTile.LADDERID) {
                        // Debug.Log("LadderButton");

                        LadderBlock l = Instantiate(ladderBlock, coordToBlock[vv].transform);
                        l.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, s.position, s.direction);
                        l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        ladders.Add(l);
                        l.ImmediatelyRetract();
                    } else if (s.id == SpecialTile.CANNONID) {
                        //  Debug.Log("CannonButton");
                        DirectionalCannon l = Instantiate(directionalCannon, coordToBlock[vv].transform);
                        l.block = coordToBlock[vv];
                        l.SetDirection(s.direction);
                        l.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, s.position, s.direction);
                        l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        cannons.Add(l);
                    } else if (s.id == SpecialTile.BLOCKERID) {
                        //  Debug.Log("BlockerButton");
                        coordToBlock[vv].isBlocked = true;
                        BlockerTag l = Instantiate(blockerTag, coordToBlock[vv].transform);
                        l.SetColor(save.GetColor(s.colorIndex));
                        l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        tags.Add(l);
                        coordToBlock[vv].material = crossColorMat;
                        coordToBlock[vv].ResetMaterial();
                    } else if (s.id == SpecialTile.UNIONID) {
                        coordToBlock[vv].isUnion = true;
                        coordToBlock[vv].unionIndexes.Add(s.colorIndex + 1);
                        // Debug.Log("UnionButton");
                        UnionTag l = Instantiate(unionTag, coordToBlock[vv].transform);
                        l.SetColor(save.GetColor(s.colorIndex));
                        l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        tags.Add(l);
                        coordToBlock[vv].material = allUnionMaterial;
                        coordToBlock[vv].ResetMaterial();
                    } else if (s.id == SpecialTile.TIMERID) {
                        coordToBlock[vv].isTimer = true;
                        //  Debug.Log("TimerButton");
                        TimerBlock l = Instantiate(timerBlock, coordToBlock[vv].transform);
                        l.SetColor(save.GetColor(s.colorIndex));
                        l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, 0.5f);
                        tags.Add(l);
                        timerPresent = true;
                    } else if (s.id == SpecialTile.OPPOSITEID) {


                        Block b = coordToBlock[vv];

                        if (positiveBlocks.Contains(b) || negativeBlocks.Contains(b)) {

                        } else {
                            b.isOpposite = true;
                            if (s.isPositiveOpposite()) {
                                b.isPositiveOpposite = true;
                                positiveBlocks.Add(b);
                            } else {
                                b.isPositiveOpposite = false;
                                negativeBlocks.Add(b);
                            }
                        }


                    } else if (s.id == SpecialTile.SUBGOALID) {
                        SubGoalBlock l = Instantiate(subGoalPrefab, coordToBlock[vv].transform);
                        subGoals.Add(l);
                        l.SetColor(coordToBlock[vv], this, new Vector2Int(vv.x, vv.y));
                    }
                }
            }
        }



        List<Vector3Int> backboard = new List<Vector3Int>();
        int ij = 0;
        foreach (Block b in blocks) {
            b.HideFrontOverLay();
            b.transform.SetParent(transform);
            b.name = "Block " + ij + " Size:" + b.VecList.Count + " Color: " + b.colorIndex.ToString();
            foreach (Vector3Int v in b.VecList) {
                backboard.Add(v);
            }
            ij++;
            if (b.isOpposite) {
                b.isTimer = false;
                b.SetOppositeOverlay();
            } else if (b.isTimer) {
                b.PrepTimerOverlay();
            } else if (b.isCloud) {
                b.SetCloudMesh();
            } else { 
                b.frontOverFilter.gameObject.SetActive(false);
            }
        }


        SetBackBoard(backboard, spawnSpot);
    }

    

    public void SetEmptyCrashGrid() {
        crashGrid = new Block[save.width + (sideOffset * 2), save.height, zDepthStage];
    }

    public bool isVector3IntInCrashRange(Vector3Int input) {
        if (input.x < 0 || input.y < 0 || input.z < 0) return false;
        if (input.y >= crashGrid.GetLength(1) ||
            input.x >= crashGrid.GetLength(0) ||
            input.z >= crashGrid.GetLength(2)) return false;
        return true;
    }


    public Block MakeBlock(int x, int y, int z) {
        Block block = Instantiate(blockPrefab, spawnSpot, Quaternion.identity, CrashBlockParent);
        block.gameObject.name = blocks.Count + " - Crash Block";
        block.generator = this;
        block.leveltype = LevelType.CRASH;
        block.material = crashlevelmaterial;


        if (save.is3DLevel) {
            block.blockColor = save.GetColor(save.threeDMap[x, y, z] - 1);
        } else {
            block.blockColor = save.GetColor(save.colorGrid[x, y] - 1);
        }
        block.VecList = new List<Vector3Int>();
        return block;
    }

    public bool CheckCrashIndex(Vector3Int v, BlockSaveFile s, int index) {
        return CheckCrashIndex(v.x, v.y, v.z, s, index);
    }

    public bool CheckCrashIndex(int x, int y, int z, BlockSaveFile s, int index) {
        if (s.is3DLevel) {
            Debug.Log(x.ToString() + "," + y.ToString() + ","+ z.ToString() + " looking for index " + index.ToString() + " found index " + s.threeDMap[x, y, z].ToString() + " " + (s.threeDMap[x, y, z] == index).ToString());
            return s.threeDMap[x, y, z] == index;
        } else {
            return s.colorGrid[x, y] == index;
        }
    }

    private void GenerateCrashLevel() {
        List<Vector3Int> visited = new List<Vector3Int>();
        blocks = new List<Block>();
        //    Debug.Log("1 - ?????");
        float offset = -0.5f;
        if (save.width % 2 == 1) offset = 0.5f;
        CrashBlockParent.transform.localPosition = new Vector3(-(((save.width + 10)/2) + offset), 0, -5 );
        save.EnsureDepthGridIsNotNull();
        SetEmptyCrashGrid();
        spawnSpot += new Vector3(0, 0, 1);

        /// GENERATE LEVELS NORMALLY
        if (!save.is3DLevel) {
            for (int x = 0; x < save.width; x++) {
                for (int y = 0; y < save.height; y++) {
                    if (save.colorGrid[x, y] != 0 && !visited.Contains(new Vector3Int(x, y, 0))) {

                        Block block = MakeBlock(x, y, 0);

                        block.VecList.Add(new Vector3Int(x, y, 0));
                        int index = save.colorGrid[x, y];
                        block.colorIndex = index;
                        Queue<Vector3Int> bQueue = new Queue<Vector3Int>();
                        bQueue.Enqueue(new Vector3Int(x, y, 0));
                        visited.Add(new Vector3Int(x, y, 0));
                        while (bQueue.Count > 0) {

                            Vector3Int v = bQueue.Dequeue();
                            if (!coordToBlock.ContainsKey(v)) {
                                coordToBlock.Add(v, block);

                            }

                            if (v.x == save.goalSpot.x && v.y == save.goalSpot.y) {
                                block.hasGoal = true;
                                goalBlock = block;
                            }

                            //save.colorGrid[v.x - 1, v.y] == index

                            if (v.x > 0 && !visited.Contains(v - Vector3Int.right) && CheckCrashIndex(v.x-1, v.y, 0, save, index) ) {
                                visited.Add(v - Vector3Int.right);
                                block.VecList.Add(v - Vector3Int.right);
                                bQueue.Enqueue(v - Vector3Int.right);
                            }
                            if (v.x < xmax && !visited.Contains(v + Vector3Int.right) && CheckCrashIndex(v.x + 1, v.y, 0, save, index)) {
                                visited.Add(v + Vector3Int.right);
                                block.VecList.Add(v + Vector3Int.right);
                                bQueue.Enqueue(v + Vector3Int.right);
                            }
                            if (v.y < ymax && !visited.Contains(v + Vector3Int.up) && CheckCrashIndex(v.x, v.y + 1, 0, save, index)) {
                                visited.Add(v + Vector3Int.up);
                                block.VecList.Add(v + Vector3Int.up);
                                bQueue.Enqueue(v + Vector3Int.up);
                            }
                            if (v.y > 0 && !visited.Contains(v - Vector3Int.up) && CheckCrashIndex(v.x, v.y - 1, 0, save, index)) {
                                visited.Add(v - Vector3Int.up);
                                block.VecList.Add(v - Vector3Int.up);
                                bQueue.Enqueue(v - Vector3Int.up);
                            }
                        }

                        List<Vector3Int> toAdd = new List<Vector3Int>();

                        foreach (Vector3Int vvv in block.VecList) {
                            int Cdepth = save.depthGrid[vvv.x, vvv.y];
                            //  Debug.Log(vvv.ToString() + Cdepth.ToString());
                            if (Cdepth > 0) {
                                //Debug.LogError("ADDING Z SHIT " + Time.time);
                                for (int i = 0; i < Cdepth; i++) {
                                    toAdd.Add(new Vector3Int(vvv.x, vvv.y, 1 + i));
                                    toAdd.Add(new Vector3Int(vvv.x, vvv.y, -(1 + i)));
                                }
                            }
                        }
                        foreach (Vector3Int vadd in toAdd) {
                            block.VecList.Add(vadd);
                        }


                        blocks.Add(block);
                    }
                }
            }
        } else {
            for (int x = 0; x < save.threeDMap.GetLength(0); x++) {
                for (int y = 0; y < save.threeDMap.GetLength(1); y++) {
                    for (int z = 0; z < save.threeDMap.GetLength(2); z++) {
                        if (save.threeDMap[x, y, z] != 0 && !visited.Contains(new Vector3Int(x, y, z))) {

                            Block block = MakeBlock(x, y, z);
                            block.blockColor = save.GetColor(save.threeDMap[x, y,z] - 1);

                            block.VecList.Add(new Vector3Int(x, y, z));
                            int index = save.threeDMap[x, y,z];
                            block.colorIndex = index;
                            Queue<Vector3Int> bQueue = new Queue<Vector3Int>();
                            bQueue.Enqueue(new Vector3Int(x, y, z));
                            visited.Add(new Vector3Int(x, y, z));
                            while (bQueue.Count > 0) {

                                Vector3Int v = bQueue.Dequeue();
                                if (!coordToBlock.ContainsKey(v)) {
                                    coordToBlock.Add(v, block);

                                }

                                if (v == save.threeDGoalSpot) {
                                    block.hasGoal = true;
                                    goalBlock = block;
                                }

                                //save.colorGrid[v.x - 1, v.y] == index

                                Vector3Int vCheck = v - Vector3Int.right;


                                if (v.x > 0 && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }
                                vCheck = v + Vector3Int.right;
                                if (v.x < xmax && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }


                                vCheck = v + Vector3Int.up;
                                if (v.y < ymax && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }
                                vCheck = v - Vector3Int.up;

                                if (v.y > 0 && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }

                                vCheck = v + Vector3Int.forward;
                                if (v.z < zmax && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }

                                vCheck = v - Vector3Int.forward;
                                if (v.z > 0 && !visited.Contains(vCheck) && CheckCrashIndex(vCheck, save, index)) {
                                    visited.Add(vCheck);
                                    block.VecList.Add(vCheck);
                                    bQueue.Enqueue(vCheck);
                                }


                            }

                           
                            blocks.Add(block);
                        }
                    }
                }
            }

        }

        /// GENERATE 3D CRASH LEVELS



        for (int i = 0; i < blocks.Count; i++) {
            blocks[i].leveltype = save.levelType;
            blocks[i].BuildChunk(blocks[i].VecList, save.width, save.height, save.depthGrid, save);
            // blocks[i].SetPullLevelInstantly(3);
            blocks[i].transform.localPosition = new Vector3(sideOffset, 0, 3 - blocks[i].crashDepthOffset);
            
        }

        pullOuts = new List<PulloutBlock>();
        ladders = new List<LadderBlock>();
        subGoals = new List<SubGoalBlock>();
        cannons = new List<DirectionalCannon>();


        if (save.specialTile != null && save.specialTile.Count > 0) {
            foreach (SpecialTile s in save.specialTile) {
                Vector3Int vv = new Vector3Int(s.position.x, s.position.y, 0);
               // Debug.Log("SpecialTile");
                if (s.id == SpecialTile.PULLOUTBUTTON) {
                //    Debug.Log("PullOutButton");
                    PulloutBlock p = Instantiate(pullOutBlock, coordToBlock[vv].transform);
                    if (s.subid == 0) {
                        p.single = false;
                    } else {
                        p.single = true;
                    }
                    p.direction = s.direction;
                    p.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, new Vector3Int(s.position.x, s.position.y, 0));
                    p.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, GetCrashTileOffsetForSpot(coordToBlock[vv], s.position.x, s.position.y));
                    pullOuts.Add(p);
                } else if (s.id == SpecialTile.LADDERID) {
                 //   Debug.Log("LadderButton");
                    LadderBlock l = Instantiate(ladderBlock, coordToBlock[vv].transform);
                    l.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, s.position, s.direction);
                    l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, GetCrashTileOffsetForSpot(coordToBlock[vv], s.position.x, s.position.y));
                    ladders.Add(l);
                    l.ImmediatelyRetract();
                } else if (s.id == SpecialTile.CANNONID) {
                    Debug.Log("CannonButton");
                    DirectionalCannon l = Instantiate(directionalCannon, coordToBlock[vv].transform);
                    l.SetColor(s.colorIndex + 1, coordToBlock[vv], save.GetColor(s.colorIndex), this, s.position, s.direction);
                    l.transform.localPosition = new Vector3(s.position.x + 0.5f, s.position.y + 1, GetCrashTileOffsetForSpot(coordToBlock[vv], s.position.x, s.position.y));
                    l.block = coordToBlock[vv];
                    l.SetDirection(s.direction);
                    cannons.Add(l);
                } else if (s.id == SpecialTile.CLOUDID && currentLevelType == LevelType.CRASH) {
                    if (coordToBlock.ContainsKey(vv)) {
                        Block b = coordToBlock[vv];
                        b.isCloud = true;
                    }
                } else if (s.id == SpecialTile.SUBGOALID) {
                    SubGoalBlock l = Instantiate(subGoalPrefab, coordToBlock[vv].transform);
                    subGoals.Add(l);
                    l.SetColor(coordToBlock[vv], this, new Vector2Int(vv.x, vv.y));
                }


            }
        }

      

        FullySetCrashBlocks();



        if (!save.is3DLevel) {
            goalBlock = crashGrid[sideOffset + save.goalSpot.x, save.goalSpot.y, 3];
        } else {
            if (goalBlock == null) {
                goalBlock = blocks[0];
            }
        }
        goal.transform.SetParent(goalBlock.transform);


        if (save.is3DLevel) { 
        goal.transform.localPosition = new Vector3(save.threeDGoalSpot.x + 0.5f, save.threeDGoalSpot.y + 1, save.threeDGoalSpot.z + 0.5f);


        } else {
            goal.transform.localPosition = new Vector3(save.goalSpot.x + 0.5f, save.goalSpot.y + 1, GetCrashTileOffsetForSpot(goalBlock, save.goalSpot.x, save.goalSpot.y));

        }
        goal.ShowTouchablePart();

        LadderCheck();

        foreach (PulloutBlock b in pullOuts) {
            b.PressCheck();
        }

        foreach (Block b in blocks) {
            if (b.isCloud) {
                b.SetCloudMesh();
            }
        }


    }

    public float GetCrashTileOffsetForSpot(Block b, int x, int y) {

        int depth = save.depthGrid[x, y];
        
        if (b.crashdepthWidth == 1) {
            return 0.5f;
        }

        return (b.crashdepthWidth/2f) - depth;
    }

    



    public void FullySetCrashBlocks() {
        SetEmptyCrashGrid();
        foreach (Block b in blocks) {
            b.SetCrashGrid(this);

            Vector3 oldTransform = b.transform.localPosition;

            b.transform.localPosition = new Vector3(Mathf.Round(b.transform.localPosition.x), Mathf.Round(b.transform.localPosition.y), Mathf.Round(b.transform.localPosition.z));

            if (oldTransform != b.transform.transform.localPosition) {
                Debug.LogError("Had to round for some reason on block: " + b.name + " " + Time.time);
            }

            b.crashStartPosition = b.transform.localPosition;
            b.crashMovePosition = b.transform.localPosition;
        }
    }


    public void ShowDebugThings(BlockLevelGenerator blockLevelGenerator) {
        foreach (GameObject g in Debuggers) {
            if (g != null) {
                Destroy(g);
            }
        }
        Debuggers = new List<GameObject>();
        if (crashGrid == null) return;
        for (int x = 0; x < crashGrid.GetLength(0); x++)
            for (int y = 0; y < crashGrid.GetLength(1); y++)
                for (int z = 0; z < crashGrid.GetLength(2); z++) {
                    if (crashGrid[x,y,z] != null) {
                        GameObject g =Instantiate(DebugCrashPrefab, CrashBlockParent);
                        g.transform.localPosition = new Vector3(x, y, z);
                        Debuggers.Add(g);
                        
                    }
                }
    }


    private void GenerateStretchLevel() {
        
    


        List<Vector3Int> visited = new List<Vector3Int>();
        blocks = new List<Block>();
        for (int x = 0; x < save.width; x++) {
            for (int y = 0; y < save.height; y++) {
                if (save.colorGrid[x, y] != 0 && !visited.Contains(new Vector3Int(x, y,0))) {
                    Block block = Instantiate(blockPrefab, spawnSpot, Quaternion.identity);
                    block.generator = this;
                    block.material = crashlevelmaterial;
                    block.blockColor = save.GetColor(save.colorGrid[x, y] - 1);
                    block.VecList = new List<Vector3Int>();
                    block.VecList.Add(new Vector3Int(x, y,0));
                    int index = save.colorGrid[x, y];
                    block.colorIndex = index;
                    Queue<Vector3Int> bQueue = new Queue<Vector3Int>();
                    bQueue.Enqueue(new Vector3Int(x, y,0));
                    visited.Add(new Vector3Int(x, y));
                    while (bQueue.Count > 0) {

                        Vector3Int v = bQueue.Dequeue();
                        if (!coordToBlock.ContainsKey(v)) {
                            coordToBlock.Add(v, block);

                        }

                        if (v.x == save.goalSpot.x && v.y == save.goalSpot.y) {
                            block.hasGoal = true;
                            goalBlock = block;
                        }
                        if (v.x > 0 && !visited.Contains(v - Vector3Int.right) && save.colorGrid[v.x - 1, v.y] == index) {
                            visited.Add(v - Vector3Int.right);
                            block.VecList.Add(v - Vector3Int.right);
                            bQueue.Enqueue(v - Vector3Int.right);
                        }
                        if (v.x < xmax && !visited.Contains(v + Vector3Int.right) && save.colorGrid[v.x + 1, v.y] == index) {
                            visited.Add(v + Vector3Int.right);
                            block.VecList.Add(v + Vector3Int.right);
                            bQueue.Enqueue(v + Vector3Int.right);
                        }
                        if (v.y < ymax && !visited.Contains(v + Vector3Int.up) && save.colorGrid[v.x, v.y + 1] == index) {
                            visited.Add(v + Vector3Int.up);
                            block.VecList.Add(v + Vector3Int.up);
                            bQueue.Enqueue(v + Vector3Int.up);
                        }
                        if (v.y > 0 && !visited.Contains(v - Vector3Int.up) && save.colorGrid[v.x, v.y - 1] == index) {
                            visited.Add(v - Vector3Int.up);
                            block.VecList.Add(v - Vector3Int.up);
                            bQueue.Enqueue(v - Vector3Int.up);
                        }
                    }
                    blocks.Add(block);
                }
            }
        



    }

    }

    public void ClearLevelObjects() {
        CleanUp();
        foreach (PulloutBlock b in pullOuts) {
            if (b != null) {
                Destroy(b.gameObject);
            }
        }
        pullOuts = new List<PulloutBlock>();
        foreach (LadderBlock l in ladders) {
            if (l != null) {
                Destroy(l.gameObject);
            }
        }
        ladders = new List<LadderBlock>();
        foreach (DirectionalCannon c in cannons) {
            if (c != null) {
                Destroy(c.gameObject);
            }
        }
        cannons = new List<DirectionalCannon>();
        foreach (IndicatorColorTag c in tags) {
            if (c != null) {
                Destroy(c.gameObject);
            }
        }
        foreach (SubGoalBlock s in subGoals) {
            if (s != null) {
                Destroy(s.gameObject);
            }
        }
        subGoals = new List<SubGoalBlock>();
        tags = new List<IndicatorColorTag>();
    }

    public void SetInGameBoxMenu() {
        if (currentLevelType == LevelType.PULL) {
            inGameBoxCollider.size = new Vector3(save.width + 6, save.height + 6, pullLevelDepth + 5);
        } else if (currentLevelType == LevelType.CRASH) {
            inGameBoxCollider.size = new Vector3(save.width + 16, save.height + 6, pullLevelDepth + 15);
        }
        inGameBoxCollider.center = new Vector3(0, (((float)save.height) / 2f), 0);
        timeController.ResetTimeline();
        timeController.StartRecording();
        if (currentLevelType == LevelType.CRASH) {
            foreach (Block b in blocks) {
                b.RecordInitialPositionForTimer();
            }
        }
    }

    public void SetBackBoard(List<Vector3Int> backboard, Vector3 spawnSpot) {
        backBoardBlock = Instantiate(blockPrefab, spawnSpot + new Vector3(0,0,0.01f), Quaternion.identity);
        backBoardBlock.generator = this;
        backBoardBlock.isBackBoard = true;
        backBoardBlock.material = pullBackMaterial;
        backBoardBlock.blockColor = Color.white;
        backBoardBlock.BuildChunk(backboard, save.width, save.height, save.depthGrid, save);
        backBoardBlock.transform.localScale = new Vector3(1, 1, 0.1f);
        //backBoardBlock.frontMeshRenderer.material = backBoardMaterial;
        backBoardBlock.frontMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        backBoardBlock.name = "BackBoard";
        backBoardBlock.sideMeshCollider.enabled = false;
        backBoardBlock.frontMeshCollider.enabled = true;
        backBoardBlock.sideMeshFilter.transform.gameObject.SetActive(false);
        backBoardBlock.transform.localScale = new Vector3(1, 1, -1);
        backBoardBlock.transform.SetParent(transform);
        backBoardBlock.HideFrontOverLay();
    }

  

    public IEnumerator GenerateLevel(BlockSaveFile saveFile, bool canChangeArea, bool startPlayerMovingAfter) {
       
        arrowIndicator.Hide();
        timerPresent = false;
        bool changeHappened = false;
        if (canChangeArea) {
            
            LevelGroup g = GameMasterManager.instance.GetLevelGroupFromString(saveFile.environmentName);

            if (g == null) g = GameMasterManager.instance.GetLevelGroupFromString(GameMasterManager.instance.environmentController.currentEnvironmentGroup.envKey) ;

           

            if (g != null && !g.envGroup.CanFitLevel(new Vector2Int(saveFile.width, saveFile.height)) && GameMasterManager.currentGameMode != GameMode.STORYLEVEL) {
               
                g = GameMasterManager.instance.overSizeLevelArea;
            }

           

            if (g != null && g.envGroup != GameMasterManager.instance.environmentController.currentEnvironmentGroup /*&& GameMasterManager.instance.LevelGroupUnlocked(g)*/) {
           //     Debug.Log("Change Area Here " + Time.time);
                yield return GameMasterManager.instance.environmentController.SetEnvironmentNum(g.envGroup, false);

                changeHappened = true;


            }
            yield return null;
            isCustomLevel = false;
         //   print(saveFile.environmentName + " " + GameMasterManager.instance.environmentController.currentEnvironmentGroup + g.envGroup);

        }

        coreRenderer.material = GameMasterManager.envLevelGroup.envGroup.coreMaterial;
        coreCenterSprite.color = GameMasterManager.envLevelGroup.envGroup.coreCenterColor;
        coreBorderSprite.color = GameMasterManager.envLevelGroup.envGroup.coreBorderColor;


        pullLevelDepth = 3;
        if (saveFile.is4Level()) pullLevelDepth = 4;
        if (saveFile.is5Level()) pullLevelDepth = 5;
        if (saveFile.isSixLevel()) pullLevelDepth = 6;

        lastSelected = saveFile;
        player.cameraPositionSetter.SetDefaultIndex();
        currentLevelType = saveFile.levelType;
        resetButton.Setup();
        coordToBlock = new Dictionary<Vector3Int, Block>();
        goalBlock = null;
        levelName.SetText(saveFile.levelName);
       // textDisplayer.SetTemplateStrings(saveFile.messages);
        firstBlockPulledOutYet = false;
        ClearLevelObjects();
        save = saveFile;

        sideAmount = 4;
        sideOffset = 1;
        zDepthStage = pullLevelDepth + 3;
        if (saveFile.levelType == LevelType.CRASH) {
            sideAmount = 12;
            sideOffset = 5;
            if (saveFile.is3DLevel) {
                zDepthStage = saveFile.threeDMap.GetLength(2) + 8;

            } else {
                zDepthStage = 9;
            }
        }


        if (save.is3DLevel) {
            xmax = save.threeDMap.GetLength(0) - 1;
            ymax = save.threeDMap.GetLength(1) - 1;
            zmax = save.threeDMap.GetLength(2) - 1;

        } else {
            xmax = save.width - 1;
            ymax = save.height - 1;
            zmax = 0;
        }
        spawnSpot = new Vector3(-(save.width / 2f), 0, 0);
       if (save.width % 2 == 1) {

       } else {
            spawnSpot += new Vector3(0.5f,0,0);
       }

        float zpos;
        if (currentLevelType == LevelType.PULL) {
            zpos = -3;
        } else {
            zpos = -1.5f;
        }

        if (save.width % 2 == 1) {
            platform.transform.position = new Vector3(0, platform.transform.position.y, zpos);
            platform.transform.localScale = new Vector3(save.width + sideAmount, platform.transform.localScale.y, zDepthStage);
        } else {
            platform.transform.position = new Vector3(0.5f, platform.transform.position.y, zpos);
            platform.transform.localScale = new Vector3(save.width + sideAmount, platform.transform.localScale.y, zDepthStage);
        }


        SetLowPolyStage();
        float offset = 0;
        if (save.width % 2 == 0) offset = evenoffset;
        player.width = save.width;
        player.offset = offset;
        SetTileMats();

        

        crashGrid = null;

        if (saveFile.levelType == LevelType.PULL) {
            GeneratePullLevel();
        } else if (saveFile.levelType == LevelType.CRASH) {
            GenerateCrashLevel();
        } else if (saveFile.levelType == LevelType.STRETCH) {
            GenerateStretchLevel();
        }

        if (GameMasterManager.IsCurrentlyGamePad()) {
            GameMasterManager.instance.inputManger.jumpDown = false;
        }

        foreach (LadderBlock l in ladders) {
            l.SetOtherLadderBlock();
        }
        LadderCheck();
        SetInGameBoxMenu();

        float amt = 0.5f;
        if (saveFile.width % 2 == 1) amt = 0;


        resetButton.transform.localPosition = new Vector3(amt, resetButton.transform.localPosition.y, -pullLevelDepth - 2.5f);

        oppositePullLevel = 0;
        SetOppositeBlock(true);
        hintsAvilable = false;
        stringCutscene.gameObject.SetActive(false);
        tutorialNPC.gameObject.SetActive(false);
        stringCutscene.manager = GameMasterManager.instance.cutsceneManager;
        GameMasterManager.instance.hintButton.gameObject.SetActive(false);
        GameMasterManager.instance.cutsceneManager.currentCutscene = null;
        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL && saveFile.cutsceneid != null && saveFile.cutsceneid.Length > 0) {
           // Debug.Log("CUTSCENE INNER" + saveFile.cutsceneid);
            GameMasterManager.instance.cutsceneManager.ShowCutscene(saveFile.cutsceneid);
        } else if (GameMasterManager.currentGameMode != GameMode.MENUBACKGROUNDLOAD) {
            GameMasterManager.instance.cutsceneManager.HideCutsceneManager();


            if (save.messages != null && save.messages.Count > 0) {
                GameMasterManager.instance.hintButton.gameObject.SetActive(true);
                GameMasterManager.instance.hintButton.gameObject.SetActive(true);
                stringCutscene.SetMessages(save.messages);
                hintsAvilable = true;
                GameMasterManager.instance.cutsceneManager.currentCutscene = stringCutscene;
            }
           
        }

        if (startPlayerMovingAfter) {
            player.SetIdleUI();
            GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
            PlayerController.cantJumpTime = Time.time + 0.25f;
            
            SetPlayerAtStartPostion();
            player.StartMoving();
        }

       

        if (Environment.instance.startCutscene != null && changeHappened) {
            if (GameMasterManager.gameSaveFile.viewedStoryCutscenes == null) GameMasterManager.gameSaveFile.viewedStoryCutscenes = new HashSet<string>();

            if (!GameMasterManager.gameSaveFile.viewedStoryCutscenes.Contains(Environment.instance.startCutscene.cutsceneid)) {

                yield return GameMasterManager.instance.StoryCutsceneNum(Environment.instance.startCutscene, false);
                // GameMasterManager.gameSaveFile.viewedStoryCutscenes.Add(Environment.instance.startCutscene.cutsceneid);
                GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
                GameMasterManager.instance.playerController.gameObject.SetActive(true);
                GameMasterManager.instance.playerController.SetIdleUI();


            }

        }

        if (currentLevelType == LevelType.CRASH && !GameMasterManager.instance.mainMenu.gameObject.activeInHierarchy) {
            player.machine.ChangeState(new GravityCheckState(new PlayerIdleState()));
        }



    }

    public bool hintsAvilable = false;


    public void SetPlayerAtStartPostion() {
        //  Debug.Break();
        
        player.gameObject.SetActive(true);
       // GameMasterManager.instance.ShowPlayerUI();
        //player.machine = new StateMachine<PlayerController>(new PlayerIdleState(), player);
        GameMasterManager.instance.pauseScreen.EnsureClosed();

        if (save == null || save.width % 2 == 1) {
            player.transform.position = playerStartPostion.transform.position;
            resetButton.transform.position = new Vector3(playerStartPostion.transform.position.x, resetButton.transform.position.y, resetButton.transform.position.z);
        } else {
            player.transform.position = playerStartPostion.transform.position + new Vector3(evenoffset,0,0);
            resetButton.transform.position = new Vector3(playerStartPostion.transform.position.x + evenoffset, resetButton.transform.position.y, resetButton.transform.position.z);
        }

        resetButton.transform.localPosition = new Vector3(resetButton.transform.localPosition.x, resetButton.transform.localPosition.y, -pullLevelDepth - 2.5f);
       // Debug.LogError(player.transform.position);
        player.body.velocity = new Vector3(0, 0, 0);
        player.position = new Vector3Int((int)playerStartPostion.transform.position.x, (int)playerStartPostion.transform.position.y, (int)playerStartPostion.transform.position.z);
        player.body.position = player.transform.position;
        player.transform.rotation = Quaternion.Euler(0,180,0);
        //player.transform.position += new Vector3(0.001f,0,0);
        
    }


    public void LadderCheck() {

        foreach (LadderBlock l in ladders) {
            l.LadderCheck();
        }
        foreach (LadderBlock l in ladders) {
            l.OtherLadderCheck();
        }
        foreach (DirectionalCannon c in cannons) {
            c.CannonCheck();
        }



    }

    public void PullOutCheck() {
        foreach (PulloutBlock p in pullOuts) {
            p.PressCheck();
        }
    }

    public void CleanUp() {
        goal.transform.SetParent(transform);
        arrowIndicator.Hide();
        arrowIndicator.transform.SetParent(transform);
        foreach (Block b in blocks) {
            if (b != null) {
                Destroy(b.gameObject);
            }
        }
        if (backBoardBlock != null) Destroy(backBoardBlock.gameObject);
        blocks = new List<Block>();
    }

    private void OnDrawGizmos() {
        if (playerStartPostion != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playerStartPostion.position + new Vector3(0,0.5f,0), 0.25f);
        }
    }

    public void GoalCheck() {
        goal.Check();
    }

    public void BlockCheck() {
        LadderCheck();
        PullOutCheck();
    }

    public void SetOppositeBlock(bool instant) {

        foreach (Block b in positiveBlocks) {
            if (!instant) {
                b.SetPullLevel(oppositePullLevel);
            } else {
                b.SetPullLevelInstantly(oppositePullLevel);
            }
            if (b.currentPullLevel > 0)b.MakeSureWallIsShowing();
        }

        foreach (Block b in negativeBlocks) {
            if (!instant) {
                b.SetPullLevel( pullLevelDepth - oppositePullLevel);
            } else {
                b.SetPullLevelInstantly(pullLevelDepth - oppositePullLevel);
            }
            if (b.currentPullLevel > 0) b.MakeSureWallIsShowing();
        }


    }

    public bool IncreaseOppositeLevel() {
        if (oppositePullLevel >= pullLevelDepth) {
            return false;
        }
        oppositePullLevel++;
        SetOppositeBlock(false);
        return true;
    }

    public bool DecreaseOppositePullLevel() {
        if (oppositePullLevel <= 0) {
            return false;
        }
        oppositePullLevel--;
        SetOppositeBlock(false);
        return true;
    }

    internal bool HasPulloutAt(Vector3Int belowPos) {
        foreach (PulloutBlock p in pullOuts) {
            if (p.position == belowPos) {
                return true;
            }
        }
        return false;
    }


    [ContextMenu("RandomizeBlocks")]
    public void RandomizeBlocks() {
        foreach (Block b in blocks) {
            b.SetPullLevelInstantly(UnityEngine.Random.Range(1, pullLevelDepth+1));
        }
    }

    public Block GetCrashBlock(Vector3Int pos) {
        if (pos.y < 0) return null;
        if (pos.x < 0 || pos.y >= crashGrid.GetLength(1) || pos.z < 0 || pos.z >= crashGrid.GetLength(2) || pos.x >= crashGrid.GetLength(0)) return null;
        return crashGrid[pos.x, pos.y, pos.z];
    }

    internal bool SubGoalCheck() {
        foreach (SubGoalBlock g in subGoals) {
            if (g.showing) {
                return false;
            }
        }
        return true;
    }
}





