
//using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };

public class Block : MonoBehaviour {

    public const float TIMERRETRACTTIME = 12;
    public bool isNormalBlock = true;
    public static List<Sprite> guidingSprites = new List<Sprite>();
    [Header("MovementStats")]
    public float pushMoveSpeed = 2f;
    public float fastMoveSpeed = 5f;
    public static float[] zDistances = new float[7] { -0.01f, -1, -2, -3, -4, -5, -6 };
    public bool hasGoal = false;
    public int currentPullLevel = 0;
    public bool moving = false;
    public float targetZ = 0;
    public int colorIndex;
    public bool isUnion = false;
    public bool isTimer = false;
    public bool isBlocked = false;
    public bool isCloud;
    public bool isOpposite;
    public bool isPositiveOpposite;
    public List<int> unionIndexes = new List<int>();
    public float timerReTract = 0;
    public Material material;
    public Color blockColor;
    public List<Vector3Int> VecList = new List<Vector3Int>();
    public MeshFilter frontMeshFilter;
    public MeshRenderer frontMeshRenderer;
    public MeshCollider frontMeshCollider;
    public MeshFilter frontOverFilter;
    public MeshRenderer frontOverRenderer;
    public MeshFilter sideMeshFilter;
    public MeshRenderer sideMeshRenderer;
    public MeshCollider sideMeshCollider;

    [HideInInspector] public BlockLevelGenerator generator;
    private Vector3 vertexoffsets = new Vector3(0.5f, 0.5f, 0.5f);
    public const int EMPTYBLOCKNUMBER = 0;
    public int[,,] chunkData;
    public int chunkSize, chunkHeight;
    public Vector3 vec = new Vector3();
    public int crashminx = 0;
    public int crashxmax = 0;
    public int crashMinY;
    int xmax, ymax, zmax;
    int x, y, z;



    [Header("Crash positions")]
    public Vector3 crashStartPosition;
    public Vector3 crashMovePosition;
    public Vector3 crashRevertToPos;



    public void SetPullLevelInstantly(int i) {
        currentPullLevel = i;
        transform.position = new Vector3(transform.position.x, transform.position.y, zDistances[i]);
        targetZ = zDistances[i];
        moving = false;
        generator.GoalCheck();
        SetWallScale();
        HideCheck(true);
    }

    public bool isBackBoard = false;

    public LevelType leveltype;

    private void LateUpdate() {
        //if (checkedThisFrame.Count > 0) ResetAllCheckThisFrame();
    }

    public void Update() {

        if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
            if (moving) {
                if (transform.position.z != targetZ) {

                    if (TimeController.isReversing) {
                        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.MoveTowards(transform.position.z, targetZ, pushMoveSpeed * Time.deltaTime * TimeController.rewindSpeedInterval));
                    } else {
                        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.MoveTowards(transform.position.z, targetZ, pushMoveSpeed * Time.deltaTime));
                    }
                } else {
                    moving = false;
                    generator.GoalCheck();
                    HideCheck(false);
                    if (isTimer) {
                        timerReTract = TIMERRETRACTTIME;
                        ShowTimerMesh();
                    }
                }
            } else if (isTimer) {

                if (currentPullLevel > 0 && GameMasterManager.instance.playerController.IsInGameplay()) {
                    if (timerReTract <= 0) {
                        TimeController.AddBlockMove(this, this.currentPullLevel);
                        SetPullLevel(0);
                        GameMasterManager.instance.playerController.pushPullSound.Play();
                        GameMasterManager.instance.playerController.LetGoOfBLockIfHolding(this);
                        HideTimerMat();
                    } else {
                        timerReTract -= Time.deltaTime;
                        SetTimerMaterial();
                    }
                    TimeController.AddBlockTimerThisFrame(this);
                }

                if (currentPullLevel == 0 && frontOverRenderer.gameObject.activeInHierarchy) {
                    HideTimerMat();
                }
            }

        }
    }

  

    public void HideCheck(bool overrideIsPulling) {
        if (currentPullLevel == 0 && BlockLevelGenerator.currentLevelType == LevelType.PULL && (!GameMasterManager.instance.generator.player.isPulling() || overrideIsPulling)) {
            sideMeshRenderer.gameObject.SetActive(false);
            if (isUnion) {
                foreach (Block b in generator.blocks) {
                    if (unionIndexes.Contains(b.colorIndex) && b.currentPullLevel == 0) {
                        b.sideMeshRenderer.gameObject.SetActive(false);
                    }
                }
            }
        } else {
            sideMeshRenderer.gameObject.SetActive(true);
        }

        if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            sideMeshRenderer.gameObject.SetActive(false);
        }

    }

    public bool isEmpty(int x, int y, int z) {
        return chunkData[x, y, z] == EMPTYBLOCKNUMBER;
    }

    public void SetWallScale() {
        if (currentPullLevel == 0) {
            sideMeshCollider.enabled = false;
            sideMeshCollider.transform.localScale = new Vector3(1, 1, 0);
            
        } else {
            sideMeshCollider.enabled = true;
            /*  if (currentPullLevel == 1) {
                  sideMeshCollider.transform.localScale = new Vector3(1, 1, 1f / 3f);
              } else if (currentPullLevel == 2) {
                  sideMeshCollider.transform.localScale = new Vector3(1, 1, 2f / 3f);
              } else if (currentPullLevel == 3) {
                  sideMeshCollider.transform.localScale = new Vector3(1, 1, 1);
              }*/
            sideMeshCollider.transform.localScale = new Vector3(1, 1, (float)currentPullLevel/generator.pullLevelDepth);
            
        }
    }
    
    public void SetPullLevel(int v) {
        currentPullLevel = v;
        targetZ = zDistances[currentPullLevel];
        moving = true;
        generator.LadderCheck();
        generator.PullOutCheck();
        SetWallScale();
    }

    public void ResetButtonPullLevel() {
        TimeController.AddBlockMove(this, currentPullLevel);


        if (isOpposite) {
            if (isPositiveOpposite) {
                currentPullLevel = generator.oppositePullLevel;
            } else {
                currentPullLevel = generator.pullLevelDepth - generator.oppositePullLevel;
            }
        } else {
            currentPullLevel = 0;
        }
            targetZ = zDistances[currentPullLevel];
            moving = true;
            generator.LadderCheck();
            generator.PullOutCheck();
            SetWallScale();
        
    }

    public void DecreasePullLevel() {
        if (moving) return;
        if (currentPullLevel > 0) {
            currentPullLevel--;
            targetZ = zDistances[currentPullLevel];
            moving = true;
        }
        generator.LadderCheck(); 
        generator.PullOutCheck();
        SetWallScale();
    }

    internal void IncreasePullLevel() {
        if (moving) return;
        if (currentPullLevel < generator.pullLevelDepth /*(zDistances.Length - 1)*/) {
            currentPullLevel++;
            targetZ = zDistances[currentPullLevel];
            moving = true;
        }
        generator.LadderCheck();
        generator.PullOutCheck();
        MakeSureWallIsShowing();

    }

    public void MakeSureWallIsShowing() {
        SetWallScale();
        sideMeshRenderer.gameObject.SetActive(true);
        sideMeshRenderer.enabled = true;
    }



    [HideInInspector] public static List<Vector3> frontVertices = new List<Vector3>();
    [HideInInspector] public static List<Vector3> frontNormals = new List<Vector3>();
    [HideInInspector] public static List<Vector2> frontUVs = new List<Vector2>();
    [HideInInspector] public static List<int> frontTriangles = new List<int>();


    [HideInInspector] public static List<Vector3> backVertices = new List<Vector3>();
    [HideInInspector] public static List<Vector3> backNormals = new List<Vector3>();
    [HideInInspector] public static List<Vector2> backUVs = new List<Vector2>();
    [HideInInspector] public static List<int> backTriangles = new List<int>();



    //all possible UVs
    [HideInInspector] public static Vector2 uv00 = new Vector2(0,0);
    [HideInInspector] public static Vector2 uv10 = new Vector2(1, 0);
    [HideInInspector] public static Vector2 uv01 = new Vector2(0, 1);
    [HideInInspector] public static Vector2 uv11 = new Vector2(1, 1);


    static Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
    static Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

    public void Draw(int bType, bool top, bool bot, bool front, bool back, bool left, bool right) {

      //  Debug.Log("GET HERE????");

        if (bType != 0) {

            if (front)
                CreateQuad(Cubeside.FRONT);
            if (back)
                CreateQuad(Cubeside.BACK);
            if (top)
                CreateQuad(Cubeside.TOP);
            if (bot)
                CreateQuad(Cubeside.BOTTOM);
            if (left)
                CreateQuad(Cubeside.LEFT);
            if (right)
                CreateQuad(Cubeside.RIGHT);
        }
    }

    public void DrawSegmentedMesh(int bType, bool top, bool bot, bool front, bool back, bool left, bool right, int z, int x, int y) {

        if (bType != 0) {

            if (front)
                CreateSegmentedQuad(Cubeside.FRONT, z, x ,y );
            if (back)
                CreateSegmentedQuad(Cubeside.BACK,z, x, y);
            if (top)
                CreateSegmentedQuad(Cubeside.TOP,z, x, y);
            if (bot)
                CreateSegmentedQuad(Cubeside.BOTTOM,z, x, y);
            if (left)
                CreateSegmentedQuad(Cubeside.LEFT,z, x, y);
            if (right)
                CreateSegmentedQuad(Cubeside.RIGHT,z, x, y);
        }
    }



    int frontCounter = 0;
    int sideCounter = 0;

    void CreateQuad( Cubeside side) {

        switch (side) {
            case Cubeside.BOTTOM:
              
                frontVertices.Add(p0 + vec + vertexoffsets);
                frontVertices.Add(p1 + vec + vertexoffsets);
                frontVertices.Add(p2 + vec + vertexoffsets);
                frontVertices.Add(p3 + vec + vertexoffsets);

                frontNormals.Add(Vector3.down);
                frontNormals.Add(Vector3.down);
                frontNormals.Add(Vector3.down);
                frontNormals.Add(Vector3.down);

                break;
            case Cubeside.TOP:

                frontVertices.Add(p7 + vec + vertexoffsets);
                frontVertices.Add(p6 + vec + vertexoffsets);
                frontVertices.Add(p5 + vec + vertexoffsets);
                frontVertices.Add(p4 + vec + vertexoffsets);

                frontNormals.Add(Vector3.up);
                frontNormals.Add(Vector3.up);
                frontNormals.Add(Vector3.up);
                frontNormals.Add(Vector3.up);

                break;
            case Cubeside.LEFT:


                frontVertices.Add(p7 + vec + vertexoffsets);
                frontVertices.Add(p4 + vec + vertexoffsets);
                frontVertices.Add(p0 + vec + vertexoffsets);
                frontVertices.Add(p3 + vec + vertexoffsets);
                frontNormals.Add(Vector3.left);
                frontNormals.Add(Vector3.left);
                frontNormals.Add(Vector3.left);
                frontNormals.Add(Vector3.left);


                break;
            case Cubeside.RIGHT:
             
                frontVertices.Add(p5 + vec + vertexoffsets);
                frontVertices.Add(p6 + vec + vertexoffsets);
                frontVertices.Add(p2 + vec + vertexoffsets);
                frontVertices.Add(p1 + vec + vertexoffsets);
                frontNormals.Add(Vector3.right);
                frontNormals.Add(Vector3.right);
                frontNormals.Add(Vector3.right);
                frontNormals.Add(Vector3.right);


                break;
            case Cubeside.FRONT:
            
                frontVertices.Add(p4 + vec + vertexoffsets);
                frontVertices.Add(p5 + vec + vertexoffsets);
                frontVertices.Add(p1 + vec + vertexoffsets);
                frontVertices.Add(p0 + vec + vertexoffsets);

                frontNormals.Add(Vector3.forward);
                frontNormals.Add(Vector3.forward);
                frontNormals.Add(Vector3.forward);
                frontNormals.Add(Vector3.forward);


                break;
            case Cubeside.BACK:

             
                frontVertices.Add(p6 + vec + vertexoffsets);
                 frontVertices.Add(p7 + vec + vertexoffsets);
                  frontVertices.Add(p3 + vec + vertexoffsets);
                  frontVertices.Add(p2 + vec + vertexoffsets);

                  frontNormals.Add(Vector3.back);
                 frontNormals.Add(Vector3.back);
                 frontNormals.Add(Vector3.back);
                  frontNormals.Add(Vector3.back);
                

                break;
        }


        frontUVs.Add(uv11);
        frontUVs.Add(uv01);
        frontUVs.Add(uv00);
        frontUVs.Add(uv10);

        frontTriangles.Add(3 + frontCounter);
        frontTriangles.Add(1 + frontCounter);
        frontTriangles.Add(0 + frontCounter);
        frontTriangles.Add(3 + frontCounter);
        frontTriangles.Add(2 + frontCounter);
        frontTriangles.Add(1 + frontCounter);
        frontCounter += 4;
    }

    public void ImmediatelyUpdateHiddenMesh() {
        sideMeshRenderer.gameObject.SetActive(true);
    }

    public void AddSegmentedUVs(int depth) {
        if (generator.pullLevelDepth == 3) {
            ThreeColors(depth);
        } else if (generator.pullLevelDepth == 4) {
            FourColor(depth);
        } else if (generator.pullLevelDepth == 5) {
            FiveColor(depth);
        } else if (generator.pullLevelDepth == 6) {
            SixColor(depth);
        }
    }

    public void ThreeColors(int depth) {
        if (depth == 0) {
            //green
            backUVs.Add(new Vector2(1, 1));
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.75f, 0.75f));
            backUVs.Add(new Vector2(1, 0.75f));
        } else if (depth == 1) {
            //yellow
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.5f, 0.75f));
            backUVs.Add(new Vector2(0.75f, 0.75f));
        } else {
            //red
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.75f));
        }
    }

    public void FourColor(int depth) {
        if (depth == 0) {
            //Blue
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.0f, 1));
            backUVs.Add(new Vector2(0.0f, 0.75f));
            backUVs.Add(new Vector2(0.25f, 0.75f));
        } else if (depth == 1) {
            //green
            backUVs.Add(new Vector2(1, 1));
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.75f, 0.75f));
            backUVs.Add(new Vector2(1, 0.75f));
        } else if (depth == 2) {
            //yellow
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.5f, 0.75f));
            backUVs.Add(new Vector2(0.75f, 0.75f));
        } else if (depth == 3) {
            //red
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.75f));
        }
    }

    public void FiveColor(int depth) {
        if (depth == 0) {
            //Blue
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.0f, 1));
            backUVs.Add(new Vector2(0.0f, 0.75f));
            backUVs.Add(new Vector2(0.25f, 0.75f));
        } else if (depth == 1) {
            //green
            backUVs.Add(new Vector2(1, 1));
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.75f, 0.75f));
            backUVs.Add(new Vector2(1, 0.75f));
        } else if (depth == 2) {
            //yellow
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.5f, 0.75f));
            backUVs.Add(new Vector2(0.75f, 0.75f));
        } else if (depth == 3) {
            //Oragne
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.0f, 0.75f));
            backUVs.Add(new Vector2(0.0f, 0.5f));
            backUVs.Add(new Vector2(0.25f, 0.5f));
        } else if (depth == 4) {
            //red
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.75f));
        }
    }

    public void SixColor(int depth) {
        if (depth == 0) {
            //Blue
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.0f, 1));
            backUVs.Add(new Vector2(0.0f, 0.75f));
            backUVs.Add(new Vector2(0.25f, 0.75f));
        } else if (depth == 1) {
            //green
            backUVs.Add(new Vector2(1, 1));
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.75f, 0.75f));
            backUVs.Add(new Vector2(1, 0.75f));
        } else if (depth == 2) {
            //yellow
            backUVs.Add(new Vector2(0.75f, 1));
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.5f, 0.75f));
            backUVs.Add(new Vector2(0.75f, 0.75f));
        } else if (depth == 3) {
            //Oragne
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.0f, 0.75f));
            backUVs.Add(new Vector2(0.0f, 0.5f));
            backUVs.Add(new Vector2(0.25f, 0.5f));
        } else if (depth == 4) {
            //red
            backUVs.Add(new Vector2(0.5f, 1));
            backUVs.Add(new Vector2(0.25f, 1));
            backUVs.Add(new Vector2(0.25f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.75f));
        } else if (depth == 5) {
            //purple
            backUVs.Add(new Vector2(0.75f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.75f));
            backUVs.Add(new Vector2(0.5f, 0.5f));
            backUVs.Add(new Vector2(0.75f, 0.5f));
        }
    }

    void CreateSegmentedQuad(Cubeside side, int z, int x, int y) {

        switch (side) {
            case Cubeside.BOTTOM:
                // print("Bot");

                if (leveltype == LevelType.PULL) {

                    backVertices.Add(p0 + vec + vertexoffsets);
                    backVertices.Add(p1 + vec + vertexoffsets);
                    backVertices.Add(p2 + vec + vertexoffsets);
                    backVertices.Add(p3 + vec + vertexoffsets);

                    backNormals.Add(Vector3.down);
                    backNormals.Add(Vector3.down);
                    backNormals.Add(Vector3.down);
                    backNormals.Add(Vector3.down);

                    AddSegmentedUVs(z);

                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    backTriangles.Add(0 + sideCounter);
                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(2 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    sideCounter += 4;

                } else {
                    frontVertices.Add(p0 + vec + vertexoffsets);
                    frontVertices.Add(p1 + vec + vertexoffsets);
                    frontVertices.Add(p2 + vec + vertexoffsets);
                    frontVertices.Add(p3 + vec + vertexoffsets);

                    frontNormals.Add(Vector3.down);
                    frontNormals.Add(Vector3.down);
                    frontNormals.Add(Vector3.down);
                    frontNormals.Add(Vector3.down);

                    UpDownUVS(x, y, z, false);

                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontTriangles.Add(0 + frontCounter);
                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(2 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontCounter += 4;
                }

                break;
            case Cubeside.TOP:

                if (leveltype == LevelType.PULL) {

                    backVertices.Add(p7 + vec + vertexoffsets);
                    backVertices.Add(p6 + vec + vertexoffsets);
                    backVertices.Add(p5 + vec + vertexoffsets);
                    backVertices.Add(p4 + vec + vertexoffsets);

                    backNormals.Add(Vector3.up);
                    backNormals.Add(Vector3.up);
                    backNormals.Add(Vector3.up);
                    backNormals.Add(Vector3.up);

                    AddSegmentedUVs(z);

                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    backTriangles.Add(0 + sideCounter);
                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(2 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    sideCounter += 4;
                } else {
                    frontVertices.Add(p7 + vec + vertexoffsets);
                    frontVertices.Add(p6 + vec + vertexoffsets);
                    frontVertices.Add(p5 + vec + vertexoffsets);
                    frontVertices.Add(p4 + vec + vertexoffsets);

                    frontNormals.Add(Vector3.up);
                    frontNormals.Add(Vector3.up);
                    frontNormals.Add(Vector3.up);
                    frontNormals.Add(Vector3.up);

                    UpDownUVS(x, y, z, false);

                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontTriangles.Add(0 + frontCounter);
                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(2 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontCounter += 4;
                }

                break;
            case Cubeside.LEFT:
                if (leveltype == LevelType.PULL) {

                    backVertices.Add(p7 + vec + vertexoffsets);
                    backVertices.Add(p4 + vec + vertexoffsets);
                    backVertices.Add(p0 + vec + vertexoffsets);
                    backVertices.Add(p3 + vec + vertexoffsets);
                    backNormals.Add(Vector3.left);
                    backNormals.Add(Vector3.left);
                    backNormals.Add(Vector3.left);
                    backNormals.Add(Vector3.left);

                    AddSegmentedUVs(z);

                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    backTriangles.Add(0 + sideCounter);
                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(2 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    sideCounter += 4;
                } else {
                    frontVertices.Add(p7 + vec + vertexoffsets);
                    frontVertices.Add(p4 + vec + vertexoffsets);
                    frontVertices.Add(p0 + vec + vertexoffsets);
                    frontVertices.Add(p3 + vec + vertexoffsets);
                    frontNormals.Add(Vector3.left);
                    frontNormals.Add(Vector3.left);
                    frontNormals.Add(Vector3.left);
                    frontNormals.Add(Vector3.left);

                    LeftRightUVS(x, y, z, false);

                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontTriangles.Add(0 + frontCounter);
                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(2 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontCounter += 4;
                }

                break;
            case Cubeside.RIGHT:


                if (leveltype == LevelType.PULL) {

                    backVertices.Add(p5 + vec + vertexoffsets);
                    backVertices.Add(p6 + vec + vertexoffsets);
                    backVertices.Add(p2 + vec + vertexoffsets);
                    backVertices.Add(p1 + vec + vertexoffsets);
                    backNormals.Add(Vector3.right);
                    backNormals.Add(Vector3.right);
                    backNormals.Add(Vector3.right);
                    backNormals.Add(Vector3.right);

                    AddSegmentedUVs(z);

                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    backTriangles.Add(0 + sideCounter);
                    backTriangles.Add(3 + sideCounter);
                    backTriangles.Add(2 + sideCounter);
                    backTriangles.Add(1 + sideCounter);
                    sideCounter += 4;
                } else {
                    frontVertices.Add(p5 + vec + vertexoffsets);
                    frontVertices.Add(p6 + vec + vertexoffsets);
                    frontVertices.Add(p2 + vec + vertexoffsets);
                    frontVertices.Add(p1 + vec + vertexoffsets);
                    frontNormals.Add(Vector3.right);
                    frontNormals.Add(Vector3.right);
                    frontNormals.Add(Vector3.right);
                    frontNormals.Add(Vector3.right);

                    LeftRightUVS(x, y, z, false);

                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontTriangles.Add(0 + frontCounter);
                    frontTriangles.Add(3 + frontCounter);
                    frontTriangles.Add(2 + frontCounter);
                    frontTriangles.Add(1 + frontCounter);
                    frontCounter += 4;
                }

                break;
            case Cubeside.FRONT:

                if (isBackBoard) return;
                frontVertices.Add(p4 + vec + vertexoffsets);
                frontVertices.Add(p5 + vec + vertexoffsets);
                frontVertices.Add(p1 + vec + vertexoffsets);
                frontVertices.Add(p0 + vec + vertexoffsets);
                 


                //frontVertices.Add(p6 + vec + vertexoffsets);
               // frontVertices.Add(p7 + vec + vertexoffsets);
               // frontVertices.Add(p3 + vec + vertexoffsets);
               // frontVertices.Add(p2 + vec + vertexoffsets);


                //frontVertices.Add(p5 + vec + vertexoffsets);
               // frontVertices.Add(p4 + vec + vertexoffsets);
               // frontVertices.Add(p0 + vec + vertexoffsets);
               // frontVertices.Add(p1 + vec + vertexoffsets);









                //  frontNormals.Add(Vector3.back);
               //  frontNormals.Add(Vector3.back);
               //   frontNormals.Add(Vector3.back);
               //   frontNormals.Add(Vector3.back);

               frontNormals.Add(Vector3.forward);
                  frontNormals.Add(Vector3.forward);
                  frontNormals.Add(Vector3.forward);
                  frontNormals.Add(Vector3.forward);

                FrontUVS(x, y, z, true);

                frontTriangles.Add(3 + frontCounter);
                frontTriangles.Add(1 + frontCounter);
                frontTriangles.Add(0 + frontCounter);
                frontTriangles.Add(3 + frontCounter);
                frontTriangles.Add(2 + frontCounter);
                frontTriangles.Add(1 + frontCounter);
                frontCounter += 4;

                break;
            case Cubeside.BACK:


                frontVertices.Add(p6 + vec + vertexoffsets);
                frontVertices.Add(p7 + vec + vertexoffsets);
                frontVertices.Add(p3 + vec + vertexoffsets);
                frontVertices.Add(p2 + vec + vertexoffsets);

                frontNormals.Add(Vector3.back);
                frontNormals.Add(Vector3.back);
                frontNormals.Add(Vector3.back);
                frontNormals.Add(Vector3.back);

                FrontUVS(x, y, z, false);

                frontTriangles.Add(3 + frontCounter);
                frontTriangles.Add(1 + frontCounter);
                frontTriangles.Add(0 + frontCounter);
                frontTriangles.Add(3 + frontCounter);
                frontTriangles.Add(2 + frontCounter);
                frontTriangles.Add(1 + frontCounter);
                frontCounter += 4;
                break;
        }


        
       
    }

    public void SetCrashGrid(BlockLevelGenerator blockLevelGenerator) {

        Vector3Int v = new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y, (int)transform.localPosition.z);

        crashxmax = 0;
        crashminx = 100000;
        crashMinY = 100000;

        foreach (Vector3Int vec in VecList) {
            if (crashxmax < vec.x) crashxmax = vec.x;
            if (crashminx > vec.x) crashminx = vec.x;
            if (crashMinY > vec.y) crashMinY = vec.y;
            

            Vector3Int pos = new Vector3Int(vec.x, vec.y) + v;
            if (pos.y >= 0) {

                if (!hasCrashDepth) {
                    SetCrashPosition(vec.x + v.x, vec.y + v.y, v.z + vec.z);
                } else {
                    SetCrashPosition(vec.x + v.x, vec.y + v.y, v.z+ crashDepthOffset + vec.z);
                }

                
            }
        }
    }


    public void SetCrashPosition(int x, int y, int z) {
        if (generator.crashGrid[x, y, z] != null && generator.crashGrid[x,y,z] != this) Debug.LogError( new Vector3(x,y,z).ToString() + "  OVERLAP BETWEEN " + name + " and " + generator.crashGrid[x, y, z].name);
        generator.crashGrid[x, y, z] = this;
    }
    public static List<Block> blockingPlayer = new List<Block>();
    List<Vector2Int> checkedThisFrame = new List<Vector2Int>();
    public void ResetAllCheckThisFrame() {
        checkedThisFrame = new List<Vector2Int>();
    }


   

    public bool CanMoveCrashBlockInThisDirection(Vector3Int input, List<Block> visitedBlocks, bool startFresh) {

        // if (input != new Vector3Int(0,-1,0)) Debug.Log("CHECKING IF " + gameObject.name + " CAN MOVE IN DIRECTION " + input.ToString() + "VISITEDBLOCKS " + visitedBlocks.Count);

        float minXValue, maxXValue, maxZValue, minYValue;
        if (isCloud && input.y != 0) {
            return false;
        }

        maxXValue = generator.crashGrid.GetLength(0) - crashxmax - 1;
        if (hasCrashDepth) {
            maxZValue = generator.crashGrid.GetLength(2) - 3 - (crashdepthWidth - 1);
        } else {
            if (is3DLevel) {
                maxZValue = generator.crashGrid.GetLength(2) - 3 - (maxZInVecList);
            } else {
                maxZValue = generator.crashGrid.GetLength(2) - 3;
            }
        }
     //   Debug.Log("Maz Z is " + maxZValue.ToString());


        minXValue = 0 - crashminx;
        minYValue = 0 - crashMinY;
     //   Debug.Log("MinYValue: " + minYValue + " " + Time.time);
       // Debug.Log(maxXValue + " xmaxvalue " + Time.time.ToString());
       // Debug.Log(minXValue + " xminvalue " + Time.time.ToString());
       // Debug.Log(maxZValue + " zmaxvalue");

        if (input.y < 0 && transform.localPosition.y <= minYValue) {
           // Debug.Log(gameObject.name + " SHOULD NOT MOVE DOWN " + Time.time);
            return false;
        }

        if (input.x < 0 && transform.localPosition.x <= minXValue) {
           // Debug.Log("SHOULD NOT MOVE " + Time.time);
            return false;
        }
        if (input.x > 0 && transform.localPosition.x >= maxXValue) {
           // Debug.Log("SHOULD NOT MOVE" + Time.time);
            return false;
        }
     //   Debug.Log(transform.localPosition.z);
        if (input.z > 0 && transform.localPosition.z >= maxZValue) {
          //  Debug.Log("SHOULD NOT MOVE " + Time.time);
            return false;
        }
        if (input.z < 0 && transform.localPosition.z <= 0) {
          //  Debug.Log("SHOULD NOT MOVE" + Time.time);
            return false;
        }

        if (this == PlayerPushState.underBlock) {
            return false;
        }

        Vector3Int playerPosition = generator.player.position;


        List<Block> blocksToCheckAfter = new List<Block>();

        bool blockUnderneath = false;

        foreach (Vector3Int v in VecList) {

            
                Block bDir = generator.crashGrid[(int)(transform.localPosition.x + input.x + v.x), (int)(transform.localPosition.y + input.y + v.y), (int)(transform.localPosition.z + input.z + v.z + crashDepthOffset)];

                //if (bDir != null) Debug.Log("LOOKING AT " + bDir.name + " " + Time.time);

               

                if (bDir != null && bDir != this) {

                blockUnderneath = true;

                if (!blocksToCheckAfter.Contains(bDir)) blocksToCheckAfter.Add(bDir);         
                   

                }
          
        }

        if (!visitedBlocks.Contains(this)) visitedBlocks.Add(this);

       

        foreach (Block b in blocksToCheckAfter) {
          // Debug.LogError(gameObject.name + " Checking " + b.gameObject.name);
            if (!visitedBlocks.Contains(b) && !b.CanMoveCrashBlockInThisDirection(input, visitedBlocks, false)) return false;
        }


       // if (!blockUnderneath) {
            foreach (Vector2Int v in VecList) {
                if (input.y < 0 && new Vector3Int((int)transform.localPosition.x + v.x, (int)transform.localPosition.y + v.y - 1, (int)transform.localPosition.z) == playerPosition) {
                    Debug.Log("PLAYER BLOCK " + Time.time);
                    if (!blockingPlayer.Contains(this)) blockingPlayer.Add(this);
                    return false;
                }
            }
       // }

        return true;
    }

    public bool TrueIfPlayerNotBeneath() {
        return true;
    }

    bool is3DLevel;
    int maxZInVecList;

    public void BuildChunk(List<Vector3Int> vec, int width, int height, int[,] depthGrid, BlockSaveFile save) {

        this.depthGrid = depthGrid;
        hasCrashDepth = false;
        crashDepthOffset = 0;
        crashdepthWidth = 0;
        //  Debug.Log("GET HEEEEEERRREEEE");
        int maxDepth = 0;

        if (leveltype == LevelType.CRASH && save.is3DLevel) {

            chunkData = new int[save.threeDMap.GetLength(0), save.threeDMap.GetLength(1), save.threeDMap.GetLength(2)];



        } else if (leveltype == LevelType.PULL) {
            chunkData = new int[width, height, generator.pullLevelDepth];
            
        } else {
            if (depthGrid == null) {
               // Debug.LogError("DepthNull");
                depthGrid = new int[width, height];
            }
            
            foreach (Vector3Int v in vec) {
                if (depthGrid[v.x,v.y] > maxDepth) maxDepth = depthGrid[v.x,v.y];
            }


            if (maxDepth == 0) {
              //  Debug.LogError("Max Depth is 0 " + Time.time);
                chunkData = new int[width, height, 1];
            } else {
                int crashdepth = 1 + (maxDepth * 2);
               // Debug.LogError("Max Depth is " + maxDepth.ToString() +" crash depth is " + crashdepth.ToString() + " " + Time.time);
                chunkData = new int[width, height, crashdepth];
            }
            
            
            frontOverRenderer.gameObject.SetActive(false);
        }
       
        chunkHeight = height;
        chunkSize = width;

        maxZInVecList = 0;
        is3DLevel = false;

        if (save.levelType == LevelType.CRASH && save.is3DLevel) {
            foreach (Vector3Int v in vec) {
                chunkData[v.x, v.y, v.z] = 1;
                if (v.z > maxZInVecList) maxZInVecList = v.z;
            }
            is3DLevel = true;
        } else if (maxDepth == 0) {
            foreach (Vector3Int v in vec) {
                for (int i = 0; i < chunkData.GetLength(2); i++) {
                    chunkData[v.x, v.y, i] = 1;
                }
            }
            crashdepthWidth = 1;
        } else {
            int midPoint = maxDepth;
            foreach (Vector3Int v in vec) {
                    chunkData[v.x, v.y, v.z + midPoint] = 1;
            }
            hasCrashDepth = true;
            crashDepthOffset = midPoint;
            crashdepthWidth = 1 + maxDepth * 2;

        }

        BuildChunkThing();


        crashMovePosition = Vector3.zero;
        crashStartPosition = Vector3.zero;
    //   StaticBatchingUtility.Combine(gameObject);

    }

    public bool hasCrashDepth;
    public int crashDepthOffset;
    public int crashdepthWidth = 1;
    public int [,] depthGrid;

 
    private void BuildChunkThing() {
        ymax = chunkHeight - 1;
        xmax = chunkSize - 1;
        zmax = chunkData.GetLength(2) - 1;
        frontTriangles = new List<int>();
        frontUVs = new List<Vector2>();
        frontVertices = new List<Vector3>();
        frontNormals = new List<Vector3>();
        sideCounter = 0;
        frontCounter = 0;

        backTriangles = new List<int>();
        backUVs = new List<Vector2>();
        backVertices = new List<Vector3>();
        backNormals = new List<Vector3>();


        z = 0;
        int amt = chunkData.GetLength(2);
        //print(amt + " depthamount");
        int amtMax =  chunkData.GetLength(2) - 1;
        for (x = 0; x < chunkSize; x++) {
            vec.x = x;
            for (z = 0; z < amt; z++) {
                vec.z = z;
                for (y = 0; y < chunkHeight; y++) {
                    vec.y = y;
                    if (chunkData[x, y, z] != EMPTYBLOCKNUMBER)
                     if (isNormalBlock) {
                          //  Debug.Log("LIVING BEST LIFE");
                            Draw(chunkData[x, y, z],
                            (y == ymax || (y < ymax && chunkData[x, y + 1, z] == EMPTYBLOCKNUMBER)),
                            (y == 0 || (y > 0 && chunkData[x, y - 1, z] == EMPTYBLOCKNUMBER)),
                            (z == amtMax || (z < amtMax && chunkData[x, y, z + 1] == EMPTYBLOCKNUMBER)),
                            (z == 0 || (z > 0 && chunkData[x, y, z - 1] == EMPTYBLOCKNUMBER)),
                            (x == 0 || (x > 0 && chunkData[x - 1, y, z] == EMPTYBLOCKNUMBER)),
                            (x == xmax || (x < xmax && chunkData[x + 1, y, z] == EMPTYBLOCKNUMBER)));
                     } else {
                            DrawSegmentedMesh(chunkData[x, y, z],
                            (y == ymax || (y < ymax && chunkData[x, y + 1, z] == EMPTYBLOCKNUMBER)),
                            (y == 0 || (y > 0 && chunkData[x, y - 1, z] == EMPTYBLOCKNUMBER)),
                            (z == amtMax || (z < amtMax && chunkData[x, y, z + 1] == EMPTYBLOCKNUMBER)),
                            (z == 0 || (z > 0 && chunkData[x, y, z - 1] == EMPTYBLOCKNUMBER)),
                            (x == 0 || (x > 0 && chunkData[x - 1, y, z] == EMPTYBLOCKNUMBER)),
                            (x == xmax || (x < xmax && chunkData[x + 1, y, z] == EMPTYBLOCKNUMBER)), z, x, y);
                     }


                }
            }
        }
        CombineQuads();
    }

    void CombineQuads() {
        


        Mesh frontMesh = new Mesh();
        frontMesh.vertices = frontVertices.ToArray();
        frontMesh.triangles = frontTriangles.ToArray();
        frontMesh.uv = frontUVs.ToArray();
        frontMesh.normals = frontNormals.ToArray();
        frontMesh.RecalculateBounds();
        frontMeshFilter.mesh = frontMesh;
        frontMeshRenderer.material = material;
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        frontMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", blockColor);
        propBlock.SetColor("_CrossColor", blockColor);
        frontMeshRenderer.SetPropertyBlock(propBlock);
        frontMeshCollider.sharedMesh = frontMesh;

        if (!isNormalBlock) {
            sideMeshCollider.enabled = true;
            Mesh sideMesh = new Mesh();
            sideMesh.vertices = backVertices.ToArray();
            sideMesh.triangles = backTriangles.ToArray();
            sideMesh.uv = backUVs.ToArray();
            sideMesh.normals = backNormals.ToArray();
            sideMesh.RecalculateBounds();
            sideMeshFilter.mesh = sideMesh;
            sideMeshCollider.sharedMesh = sideMesh;
        } else {
            sideMeshCollider.enabled = false;
            sideMeshCollider.sharedMesh = null;
        }

    }

    public void FrontUVS(int x, int y, int z, bool flip) {


        int xamt = 1;

        if (flip) xamt = -1;

        if (IsBlocked(x+xamt,y, z) && IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            AddFrontUVs(0);
        } else if (IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            AddFrontUVs(1);
        } else if (IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            AddFrontUVs(7);
        } else if (IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            AddFrontUVs(2);
        } else if (!IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            AddFrontUVs(4);
            ///////
        } else if (!IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            
            if (IsBlocked(x + xamt, y+1,z) && IsBlocked(x - xamt, y + 1, z)) {
                AddFrontUVs(37);
            } else if (IsBlocked(x + xamt, y + 1, z)) {
                AddFrontUVs(36);
            } else if (IsBlocked(x - xamt, y + 1, z)) {
                AddFrontUVs(35);
            } else {
                AddFrontUVs(27);
            }  
        } else if (!IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {

            if (IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(43);
            } else if (IsBlocked(x + xamt, y - 1, z)) {
                AddFrontUVs(42);
            } else if (IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(41);
            } else {
                AddFrontUVs(17);
            }
        } else if (!IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            if (IsBlocked(x + xamt, y - 1, z) && IsBlocked(x + xamt, y + 1, z)) {
                AddFrontUVs(45);
            } else if (IsBlocked(x + xamt, y + 1, z)) {
                AddFrontUVs(44);
            } else if (IsBlocked(x + xamt, y - 1, z)) {
                AddFrontUVs(46);
            } else {
                AddFrontUVs(23);
            }
        } else if (IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            if (IsBlocked(x - xamt, y - 1, z) && IsBlocked(x - xamt, y + 1, z)) {
                AddFrontUVs(38);
            } else if (IsBlocked(x - xamt, y + 1, z)) {
                AddFrontUVs(40);
            } else if (IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(39);
            } else {
                AddFrontUVs(21);
            }
            //////
        } else if (IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            AddFrontUVs(10);
        } else if (!IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            AddFrontUVs(13);
        } else if (IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            if (!IsBlocked(x - xamt, y - 1, z))
                AddFrontUVs(16);
            else
                AddFrontUVs(31);

        } else if (IsBlocked(x + xamt, y, z) && !IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            if (!IsBlocked(x - xamt, y + 1, z))
                AddFrontUVs(26);
            else
                AddFrontUVs(34);

        } else if (!IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && IsBlocked(x, y + 1, z) && !IsBlocked(x, y - 1, z)) {
            if (!IsBlocked(x + xamt, y - 1, z))
                AddFrontUVs(18);
            else
                AddFrontUVs(32);
        } else if (!IsBlocked(x + xamt, y, z) && IsBlocked(x - xamt, y, z) && !IsBlocked(x, y + 1, z) && IsBlocked(x, y - 1, z)) {
            if (!IsBlocked(x + xamt, y + 1, z))
                AddFrontUVs(28);
            else
                AddFrontUVs(33);
        } else {
            if (IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - xamt, y + 1, z) && IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(3);
            } else if (!IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - xamt, y + 1, z) && !IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(22);
            } else if (IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(12);
            } else if (IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(6);
            } else if (IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && !IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(5);
            } else if (!IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(9);
            } else if (IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(8);
            } else if (IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(20);
            } else if (!IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && !IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(15);
            } else if (IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && !IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(12);
            } else if (!IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(11);
            } else if (!IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                if (flip && !IsBlocked(x + 1, y - 1, z)) AddFrontUVs(25);
                else AddFrontUVs(14);
            } else if (IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - xamt, y + 1, z) && !IsBlocked(x - xamt, y - 1, z)) {
                //if (flip) AddFrontUVs(19);
                 AddFrontUVs(19);
            } else if (IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - xamt, y + 1, z) && !IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(30);
            } else if (!IsBlocked(x + xamt, y + 1, z) && IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && !IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(25);
            } else if (!IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && IsBlocked(x - xamt, y + 1, z) && !IsBlocked(x - xamt, y - 1, z)) {
                AddFrontUVs(29);
            } else if (!IsBlocked(x + xamt, y + 1, z) && !IsBlocked(x + xamt, y - 1, z) && !IsBlocked(x - 1, y + 1, z) && IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(24);
            } else if (flip && !IsBlocked(x+1, y-1, z)) {
                if (!IsBlocked(x-1, y-1, z)) AddFrontUVs(29);
                else AddFrontUVs(19);
            } else if (flip && !IsBlocked(x + 1, y + 1, z)) {
                AddFrontUVs(24);
               
            } else if (flip && !IsBlocked(x - 1, y + 1, z) && !IsBlocked(x - 1, y - 1, z)) {
                AddFrontUVs(11);
            } else {
                AddFrontUVs(22);
            }
             
        }


    }

    public void LeftRightUVS(int x, int y, int z, bool flip) {


        

        if (IsBlocked(x, y + 1, z) && IsBlocked(x,y-1,z)) {
            AddFrontUVs(0);
        } else if (IsBlocked(x, y + 1, z)) {
            AddFrontUVs(1);
        } else if (IsBlocked(x, y - 1, z)) {
            AddFrontUVs(7);
        } else {
            AddFrontUVs(10);
        }


    }

    private void UpDownUVS(int x, int y, int z, bool flip) {


        //Single Width UVS
        if (IsBlocked(x, y, z + 1) && IsBlocked(x, y, z - 1)) {
            if (IsBlocked(x + 1, y, z) && IsBlocked(x - 1, y, z)) {
                AddFrontUVs(0);
            } else if (IsBlocked(x + 1, y, z)) {
                AddFrontUVs(4);
            } else if (IsBlocked(x - 1, y, z)) {
                AddFrontUVs(2);
            } else {
                AddFrontUVs(13);
            }
        } else if (IsBlocked(x, y, z - 1)) {
            if (IsBlocked(x - 1, y, z) && IsBlocked(x + 1, y, z)) {
                AddFrontUVs(1);
            } else if (IsBlocked(x - 1, y, z)) {
                AddFrontUVs(16);
            } else if (IsBlocked(x + 1, y, z)) {
                AddFrontUVs(18);
            } else {
                AddFrontUVs(17);
            }
        } else if (IsBlocked(x, y, z + 1)) {
            if (IsBlocked(x - 1, y, z) && IsBlocked(x + 1, y, z)) {
                AddFrontUVs(7);
            } else if (IsBlocked(x - 1, y, z)) {
                AddFrontUVs(26);
            } else if (IsBlocked(x + 1, y, z)) {
                AddFrontUVs(28);
            } else {
                AddFrontUVs(27);
            }
        } else {


            if (IsBlocked(x+1, y, z) && !IsBlocked(x-1,y,z)) {
                AddFrontUVs(23);
            } else if (!IsBlocked(x + 1, y, z) && IsBlocked(x - 1, y, z)) {
                AddFrontUVs(21);
            } else {

                if (IsBlocked(x,y,z + 1) && IsBlocked(x, y, z - 1) && !IsBlocked(x - 1, y, z + 1) && !IsBlocked(x + 1, y, z)) {
                    AddFrontUVs(13);
                } else if (!IsBlocked(x, y, z + 1) && !IsBlocked(x, y, z - 1) && IsBlocked(x - 1, y, z + 1) && IsBlocked(x + 1, y, z)) {
                    AddFrontUVs(10);
                } else if (IsBlocked(x + 1, y, z + 1) && IsBlocked(x - 1, y, z + 1)) {
                    AddFrontUVs(14);
                } else if (IsBlocked(x + 1, y, z - 1) && IsBlocked(x - 1, y, z - 1)) {
                    AddFrontUVs(19);
                } else if (IsBlocked(x + 1, y, z - 1) && IsBlocked(x + 1, y, z + 1)) {
                    AddFrontUVs(11);
                } else if (IsBlocked(x - 1, y, z - 1) && IsBlocked(x - 1, y, z + 1)) {
                    AddFrontUVs(12);
                } else {
                    AddFrontUVs(22);
                }

            }

        }


    }

    public void AddFrontUVs(int i) {
        Sprite s = guidingSprites[i];
        frontUVs.Add(s.uv[0] + new Vector2(0.001f, -0.001f));
        frontUVs.Add(s.uv[1] + new Vector2(-0.001f, -0.001f));
        frontUVs.Add(s.uv[3] + new Vector2(-0.001f, 0.001f));
        frontUVs.Add(s.uv[2] + new Vector2(0.001f, 0.001f));

        if (i == 0) {
            //print(s.uv[0]);

         //   print(s.uv[1]);

          //  print(s.uv[2]);

           // print(s.uv[3]);

            //Debug.Break();
        }
    }


    public bool IsBlocked(int x, int y, int z) {
        if (x < 0 || x > xmax || y < 0 || y > ymax || z < 0 || z > zmax) return true;
        if (chunkData[x, y, z] == 0) return true;
        return false;
    }

    internal void ResetMaterial() {
        frontMeshRenderer.material = material;
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        frontMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", blockColor);
        propBlock.SetColor("_CrossColor", blockColor);
        frontMeshRenderer.SetPropertyBlock(propBlock);
    }

    public void SetOppositeOverlay() {
        frontOverFilter.mesh = frontMeshFilter.mesh;
        if (isPositiveOpposite) {
            frontOverRenderer.material = GameMasterManager.instance.generator.positiveOppoMat;
        } else {
            frontOverRenderer.material = GameMasterManager.instance.generator.negativeOppoMat;
        }
        frontOverFilter.gameObject.SetActive(true);
    }

    public void HideFrontOverLay() {
        frontOverFilter.gameObject.SetActive(false);
    }



    const string timerColorString = "_Color";

    public void ShowTimerMesh() {
        frontOverRenderer.gameObject.SetActive(true);
        SetTimerMaterial();
    }



    public float x1;
    Color c1;

    public void SetTimerMaterial() {
        x1 = TIMERRETRACTTIME - timerReTract;

        float f = Mathf.Cos((( x1 + 3) * x1)/2);
        if (x1 < TIMERRETRACTTIME/2) {
            c1 = Color.Lerp(Colors.TIMERGREEN, Colors.TIMERYELLOW, x1 / TIMERRETRACTTIME);
        } else {
            c1 = Color.Lerp(Colors.TIMERYELLOW, Colors.TIMERRED, x1 / TIMERRETRACTTIME);
        }

       // print(material.GetColor(timerColorString).ToString() + Time.time);
        frontOverRenderer.material.SetColor(timerColorString, new Color(c1.r,c1.g,c1.b, f));


    }


    private void HideTimerMat() {
        timerReTract = 0;
        frontOverRenderer.gameObject.SetActive(false);
        //frontOverRenderer.material.SetFloat(timerColorString, 0);
    }

    public void PrepTimerOverlay() {
        frontOverFilter.mesh = frontMeshFilter.mesh;
        frontOverRenderer.material = GameMasterManager.instance.generator.timerMat;
        
        frontOverFilter.gameObject.SetActive(true);
    }

    public void SetCloudMesh() {
        frontOverFilter.mesh = frontMeshFilter.mesh;
        frontOverRenderer.material = GameMasterManager.instance.generator.cloudMaterial;
        frontOverFilter.gameObject.SetActive(true);
    }

    public void SetTimeRewind(float timerVal) {
        
        if (isTimer) {
            timerReTract = timerVal;
            if (timerReTract > 0) {
                if (!frontOverRenderer.gameObject.activeInHierarchy) frontOverRenderer.gameObject.SetActive(true);
                SetTimerMaterial();
            } else {
                HideTimerMat();
            }
        }
    }

    public void SetGoalCrashPosition(Vector3 startPos, bool setTransformImmediately) {
        if (setTransformImmediately) transform.localPosition = startPos;
        crashMovePosition = startPos;
        crashStartPosition = startPos;
        
    }

    public void RecordInitialPositionForTimer() {
        TimeController.instance.AddCrashBlockMove(this);
        CrashMoveBlockPosition movePos = new CrashMoveBlockPosition();
        movePos.block = this;

        movePos.endPos = crashMovePosition;
        movePos.startPos = crashStartPosition;
        // movePos.endPos.y = (int)movePos.endPos.y;
        //  movePos.startPos.y = (int)movePos.startPos.y;
        TimeController.instance.AddCrashEndMove(movePos);
    }
}







