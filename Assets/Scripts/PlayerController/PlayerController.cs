using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{

    public const string walking = "Walking";
    public const string groundedString = "Grounded";
    public const string jumpString = "Jump";
    public const string returnToIdleString = "ReturnIdle";
    public const string pushDirX = "PullDirectionX";
    public const string pushDirY = "PullDirectionY";
    public const string victoryDanceString = "Dance";
    public const string sidePushPull = "SidePushPull";





    public static bool playerCanCollideWithPulloutBlocks = true;
    public static PlayerController instance;
    public InteractionPromptText warpButton;
    public InputManager inputManager;
    public Rigidbody body;
    public float moveSpeed = 5f;
    public float airMoveSpeed = 2.5f;
    public float coyoteTime = 0.25f;
    public float jumpSpeed = 8;
    public float teleportMoveSpeed = 5f;
    public Camera cam;
    public LayerMask blockLayer;
    public FeetCheck feetCheck;
    // Start is called before the first frame update
    public Animator anim;
    public BodyFloater floater;
    public TextMeshProUGUI debugText;
    public Vector3Int position;
    public float gravity = -9.81f;
    public float maxnegativeVelocity = -20f;
    public GameObject mobileUI;


    public static Vector3Int currentGrabbingDirection;

    

    public static float cantJumpTime = 0;

    public CameraPositionSetter cameraPositionSetter;
    public Blocker blocker;
    public Transform lastValidGridPositionIndicator;
    public Transform floorChecker;


    public RaycastHit hit;
    internal float pauseBuffer;
    public float pullBufferTime = 0.35f;

    
    public float controllerDeadZone = 0.25f;
    public static List<LadderBlock> ladderBlocks = new List<LadderBlock>();
    public static List<DirectionalCannon> cannonBlocks = new List<DirectionalCannon>();

    [Header("Character Customization")]
    public List<MeshRenderer> bodyrenderers = new List<MeshRenderer>(); 
    public BlockLevelGenerator generator;

    public bool currentlyGrabbing = false;


    [Header("Warp")]
    public float warpShrinkSpeed = 2f;
    public float warpMoveSpeed = 1f;


    [Header("Sound")]
    public AudioPlayer jumpSound;
    public AudioPlayer landSound, grabSound, releaseSound, pushPullSound, winSound, errorSound, willhelmscream, portalSound;

    [Header("Rewind")]
    public GameObject rewindUI;
    public RewindEffect rewindEffect;
    public AudioPlayer rewindSound, rewindStart, rewindStop;
    public Image rewindBar;
    public GameObject rewindPrompt;

    [Header("Cubby Check")]
    public Transform cubbyCheck;



    private void Awake() {
        instance = this;
        Physics.gravity = new Vector3(0, gravity,0);
    }

    public bool isPulling() {
        return machine.currentState is PlayerPushState;
    }

    public void Setup()
    {
        instance = this;
        warpButton.gameObject.SetActive(false);
        cam = Camera.main;
        machine = new StateMachine<PlayerController>(new PlayerDoNothingWhileInMenuState(true), this); ;
    }

    public Vector3Int lastvalidGridPosition;

    public void SetLastValidGridPosition() {
        if (feetCheck.grounded && Physics.Raycast(floorChecker.position, Vector3.down, 0.2f, feetCheck.layer)) {
            lastvalidGridPosition = position;
            lastValidGridPositionIndicator.transform.position = PositionToVector3(lastvalidGridPosition);
            if (lastvalidGridPosition.y == 0) return;
            Vector3Int below = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y - 1,0);
            int pullLevel = -lastvalidGridPosition.z + 1;
            if (generator.coordToBlock.ContainsKey(below) && generator.coordToBlock[below].currentPullLevel < pullLevel) {
                lastvalidGridPosition.z = -generator.coordToBlock[below].currentPullLevel + 1;
            }
        }
    }

    public static Vector3 PositionToVector3(Vector3Int v) {
        Vector3 t = PlayerController.instance.generator.leftSide.transform.position + v - new Vector3(-2f, 0, 1);
        t = new Vector3(t.x, t.y, Mathf.Clamp(t.z, - GameMasterManager.instance.generator.pullLevelDepth, -0f));
        return t;
    } 

    public bool CielingCheck() {
        Vector3Int above = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y,0) + Vector3Int.up;
        if (generator.coordToBlock.ContainsKey(above) && generator.coordToBlock[above].currentPullLevel >= (-lastvalidGridPosition.z + 1)) {
            return true;
        }
       
        return false;
    }

    public void EnterPulloutFallState(Block b) {
        machine.ChangeState(new PushOutBlockPreFall(b));
    }

    public bool RightCheck() {
        Vector3Int right = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y,0) + Vector3Int.right;
        if (generator.coordToBlock.ContainsKey(right) && generator.coordToBlock[right].currentPullLevel >= (-lastvalidGridPosition.z + 1)) {
            return true;
        }

        return false;
    }
    public bool LeftCheck() {
        Vector3Int left = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y,0) - Vector3Int.right;
        if (generator.coordToBlock.ContainsKey(left) && generator.coordToBlock[left].currentPullLevel >= (-lastvalidGridPosition.z + 1)) {
            return true;
        }

        return false;
    }


    public bool CubbyCheck() {
        if (Physics.Raycast(cubbyCheck.transform.position, Vector3.up, 0.51f, feetCheck.layer) && Physics.Raycast(cubbyCheck.transform.position, Vector3.down, 1f, feetCheck.layer)) {
            return true;
        }
        return false;
    }


    public bool FarRightCheck() {
        Vector3Int left = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y,0) + (Vector3Int.right * 3) - Vector3Int.up;
        if (generator.coordToBlock.ContainsKey(left) && generator.coordToBlock[left].currentPullLevel >= (-lastvalidGridPosition.z + 1)) {
            return true;
        }

        return false;
    }

    public bool FarLeftCheck() {
        Vector3Int left = new Vector3Int(lastvalidGridPosition.x, lastvalidGridPosition.y,0) - (Vector3Int.right * 3) - Vector3Int.up;
        if (generator.coordToBlock.ContainsKey(left) && generator.coordToBlock[left].currentPullLevel >= (-lastvalidGridPosition.z + 1)) {
            return true;
        }

        return false;
    }





    public StateMachine<PlayerController> machine;


    [HideInInspector] public float width, offset;
    public Vector3 worldMovement;
    

    internal void MoveWithBlock(Block block, BlockDirection direction) {
        //Debug.Break();
       // transform.SetParent(block.transform);
        machine.ChangeState(new MoveWithBlockState(block, direction));
    }

    public void RespawnCheck() {
        if (transform.position.y < Environment.minYDistance) {
            willhelmscream.Play();
            if (Environment.instance != null && Environment.instance.respawnSpot != null) {
                //transform.position = Environment.instance.respawnSpot.transform.position;
            } else {
              //  if (Environment.instance == null) Environment.instance = FindObjectOfType<Environment>();
               // transform.position = new Vector3(0, 5, 0);
            }
            transform.position = generator.resetButton.transform.position + new Vector3(0.001f, 0.1f, 1f);

        }
    }

    public float ClampInputs(float f,  float amt) {
        return (int)(f / amt) * amt;
    }

    // Update is called once per frame
    void Update()
    {

        if (gravity != Physics.gravity.y) {
            Physics.gravity = new Vector3(0, gravity, 0);
        }

        if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
            position = new Vector3Int((int)(transform.position.x + (width / 2 - offset)), Mathf.RoundToInt(transform.position.y + 0.1f), (int)transform.position.z);
            if (transform.position.x + (width / 2 - offset) < 0) position.x = -1;
        } else {
            Vector3 pos = transform.position - generator.CrashBlockParentGoHere.position;
            if (pos.z < 0) pos.z -= 1;
            position = new Vector3Int((int)pos.x,  Mathf.RoundToInt(pos.y), (int)pos.z);   
        }


        Vector3 camforward = cameraPositionSetter.vcam.transform.forward;
        camforward.y = 0;

        worldMovement = (cameraPositionSetter.vcam.transform.right * ClampInputs(inputManager.horizontal, .125f)) + (camforward * ClampInputs(inputManager.vertical, .125f));
        worldMovement.y = 0;
        worldMovement = Vector3.ClampMagnitude(worldMovement, 1);

        
        

        debugText.text = position.ToString();
        if (machine != null && machine.currentState != null)machine.Update();

    }

    public bool warpingEnabled = false;

    public void DisableWarps() {
        warpingEnabled = false;
        LadderCheck();
    }

    public void EnableWarps() {
        warpingEnabled = true;
        LadderCheck();
    }


    public float lastMoveCameraTime = 0;
    public float moveCameraCooldown = 0.5f;

    public void MoveCameraIndex(int direction) {
      //  cameraPositionSetter.MoveIndex(direction);
    }

    public bool CameraMoveLogic() {


        if (inputManager.camHorizontal != 0 || inputManager.camVertical != 0) {

            if (lastMoveCameraTime > Time.time) {
                return false;
            }

            if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
                return false;
            }

            if (cameraPositionSetter.rotating == true) {
                return false;
            }
            lastMoveCameraTime = Time.time + moveCameraCooldown;

            
        }
        return true;


    }

    public void SetCharacterObject(CharacterObject o) {
        foreach (MeshRenderer mr in bodyrenderers) {
            mr.material = o.mat;
        }
    }

    public Block GetClickedBlock() {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100, blockLayer)) {
            return hit.collider.GetComponent<Block>();
        } else {
            return null;
        }
    }

    public void Win() {
        body.velocity = Vector3.zero;
        body.constraints = RigidbodyConstraints.FreezeAll;
        machine.ChangeState(new WinState());
        winSound.Play();
        foreach (Block b in GameMasterManager.instance.generator.blocks) {
            b.moving = false;
        }

        GameMasterManager.ModeDebugLog();

        if ( GameMasterManager.currentGameMode == GameMode.STORYLEVEL && !GameMasterManager.gameSaveFile.completedStoryLevels.Contains(GameMasterManager.currentLevel.saveFile.GetSaveKey())) {
            GameMasterManager.gameSaveFile.completedStoryLevels.Add(GameMasterManager.currentLevel.saveFile.GetSaveKey());
        

        
        } else if (GameMasterManager.currentGameMode == GameMode.LEVELSELECT &&  GameMasterManager.instance.isCurrentlyCustomLevel && !GameMasterManager.gameSaveFile.completedCustomLevels.Contains(GameMasterManager.currentLevel.saveFile.GetSaveKey()))
        {
            GameMasterManager.gameSaveFile.completedCustomLevels.Add(GameMasterManager.currentLevel.saveFile.GetSaveKey());
        }


        if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL && GameMasterManager.gameSaveFile.skippedLevels.Contains(GameMasterManager.currentLevel.saveFile.GetSaveKey())) {
            GameMasterManager.gameSaveFile.skippedLevels.Remove(GameMasterManager.currentLevel.saveFile.GetSaveKey());
        }

        
        if (GameMasterManager.gameSaveFile.trackerSaveFile.savedInts.ContainsKey(AchievementTrackerSaveFile.LEVELSCOMPLETED)) {
            GameMasterManager.gameSaveFile.trackerSaveFile.savedInts[AchievementTrackerSaveFile.LEVELSCOMPLETED]++;
        } else {
            GameMasterManager.gameSaveFile.trackerSaveFile.savedInts[AchievementTrackerSaveFile.LEVELSCOMPLETED] = 1;
        }


        if (GameMasterManager.currentGameMode == GameMode.EDITOR && !GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.ContainsKey(AchievementTrackerSaveFile.ARTIST)) {
            GameMasterManager.gameSaveFile.trackerSaveFile.savedBools.Add(AchievementTrackerSaveFile.ARTIST, true);
            GameMasterManager.instance.tracker.CheckAchievements<BoolAchievementObject>();
        }

        if (GameMasterManager.currentGameMode == GameMode.LEVELPACK) {
           
            GameMasterManager.currentLevelPack.CompleteLevel(GameMasterManager.currentLevelPackIndex);
        }


        GameMasterManager.instance.tracker.CheckAchievements<IntAchievementObject>();
        GameMasterManager.instance.tracker.CheckAchievements<LevelAchievementObject>();
        GameMasterManager.instance.SaveGameFile();

    }

    internal void SetIdleUI() {
        
        rewindUI.gameObject.SetActive(false);
        rewindEffect.HideInstantly();
    }

    internal void StartMoving() {
        machine.ChangeState(new PlayerIdleState());
    }

    public static void RemoveLadder(LadderBlock ladderBlock) {
        if (ladderBlocks.Contains(ladderBlock)) {
            ladderBlocks.Remove(ladderBlock);
        }
        LadderCheck();
    }

    public static void AddLadder(LadderBlock ladderBlock) {
        if (!ladderBlocks.Contains(ladderBlock)) {
            ladderBlocks.Add(ladderBlock);
        }
        LadderCheck();
    }

    public static void AddCannon(DirectionalCannon c ) {
        if (!cannonBlocks.Contains(c)) {
            cannonBlocks.Add(c);
        }
        CannonCheck();

    }


    public static void RemoveCannon(DirectionalCannon c) {
        if (cannonBlocks.Contains(c)) {
            cannonBlocks.Remove(c);
        }
        CannonCheck();
    }

    public static DirectionalCannon currentCannonBlock;

    public static void CannonCheck() {
        if (cannonBlocks.Count > 0 && instance.warpingEnabled) {
            currentCannonBlock = null;
            for (int i = 0; i < cannonBlocks.Count; i++) {
                if (cannonBlocks[i] != null) {
                    currentCannonBlock = cannonBlocks[i];
                }
            }
        } else {
            currentCannonBlock = null;
        }
        if (currentCannonBlock != null) {
            instance.warpButton.gameObject.SetActive(true);
            instance.warpButton.SetCannonText();
        } else {
            instance.warpButton.gameObject.SetActive(false);
        }
    }



    public static LadderBlock currentLadderBlock;
    public Transform reParentTransform;

    public static void LadderCheck() {
       if (ladderBlocks.Count > 0 && instance.warpingEnabled) {
            currentLadderBlock = null;
            for (int i = 0; i < ladderBlocks.Count; i++) {
                if (ladderBlocks[i] != null && ladderBlocks[i].ShouldBeShowing) {
                    currentLadderBlock = ladderBlocks[i];
                }
            }
       } else {
            currentLadderBlock = null;
       }
        if (currentLadderBlock != null) {
           instance.warpButton.gameObject.SetActive(true);
            instance.warpButton.SetWarpText();
        } else {
           instance.warpButton.gameObject.SetActive(false);
        }
    }

    public bool inStorySelectBox = false;
    public Transform npcLookAtSpot;

    public void ShowSelectectStoryLevelText() {
        warpButton.gameObject.SetActive(true);
        warpButton.SetStoryLevelSelectText();
        inStorySelectBox = true;
    }


    internal void HideStorySelectText() {
        inStorySelectBox = false;
        warpButton.gameObject.SetActive(false);
    }

    internal void HideInteractionText() {
        warpButton.gameObject.SetActive(false);
    }

    public NPCHuman talkToHuman = null;
    public float playerCannonSpeed = 10f;
    public const string pushingString = "Pushing";

    internal void ShowTalkPrompt(NPCHuman human) {
        talkToHuman = human;
        warpButton.gameObject.SetActive(true);
        warpButton.SetTalkText();
       
    }

    internal void HideTalkPrompt() {
        talkToHuman = null;
        warpButton.gameObject.SetActive(false);
    }



    public void FreezeRigidbodyMovement() {
        //body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.None;
        body.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreezeRigidbodyMovement() {
       // body.isKinematic = false;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void DoNothingMenuState() {
      //  Debug.Log("Here " + Time.time);
        machine.ChangeState(new PlayerDoNothingWhileInMenuState(true));
    }

    internal bool InGame() {
        if (machine == null || machine.currentState == null) return false;
        return machine.currentState is PlayerIdleState;
    }

    public bool DirectGrounded() {
        return Physics.Raycast(feetCheck.transform.position, Vector3.down, 0.25f, feetCheck.layer);
    }

    public void AddPullStat() {

        GameMasterManager.gameSaveFile.trackerSaveFile.AddPushes(1);

    }

    public void LetGoOfBLockIfHolding(Block block) {
        
        if (machine.currentState is PlayerPushState) {
            PlayerPushState p = (PlayerPushState)machine.currentState;
            if (p.block == block) {

                machine.ChangeState(new PlayerIdleState());
            }
        }

    }

    public bool IsInGameplay() {
        return machine.currentState.isGamePlayState();
    }

    public static List<LabMonitor> monitors = new List<LabMonitor>();
    public static LabMonitor currentLabMonitor;

    public void RemoveMonitor(LabMonitor labMonitor) {
        if (monitors.Contains(labMonitor)) monitors.Remove(labMonitor);
        CheckMonitors();
    }

    public void AddMonitor(LabMonitor labMonitor) {
        if (!monitors.Contains(labMonitor)) monitors.Add(labMonitor);
        CheckMonitors();
    }

    public void CheckMonitors() {
        monitors.RemoveAll(t => t == null);
        if (monitors.Count  == 0) {
            HideInteractionText();
        } else {
            instance.warpButton.gameObject.SetActive(true);
            instance.warpButton.SetMonitorText();
            currentLabMonitor = monitors[0];
        }
    }

    public void SetDead() {
        machine.ChangeState(new DeadState());
    }

    public void Pause() {
        machine.ChangeState(new PlayerPauseState());
    }

    public void ReturnToParent() {
        transform.parent = generator.transform;
    }

    public bool IsPaused() {
        return machine.currentState is PlayerPauseState;
    }

    public void StopGrabbingBlockIfHoldingBlockOfIndex(int index) {
       // Debug.Break();
        if (machine.currentState is PlayerPushState) {
            PlayerPushState p = (PlayerPushState)machine.currentState;
            print(index + " " + p.block.colorIndex + " " + Time.time);
            if (p.block.colorIndex == index) {
                Debug.Break();
            }
        }
    }

    public void SetIdleGroundedAnimation() {
        anim.SetBool(groundedString, true);
        anim.SetTrigger(returnToIdleString);
    }

    internal bool InCrashMoveState() {
        if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            return true;
            
        }
        return false;
    }

    public bool InBlocksMoving() {
        if (BlockLevelGenerator.currentLevelType == LevelType.CRASH) {
            if (machine.currentState is PushCrashState || machine.currentState is GravityCheckState) return true;
        }
        return false;
    }
}


#region states

public class MoveWithBlockState : State<PlayerController> {

    public Block block;
    BlockDirection d;
    float maxZ;
    float leeway = -0.3f;

    public MoveWithBlockState(Block b, BlockDirection dir) {
        block = b;
        d = dir;

        
    }

    public Vector3 offset;
    

    public override void Update(StateMachine<PlayerController> obj) {
        bool done = false;
        if ( d == BlockDirection.UP && obj.target.transform.position.z > maxZ) done = true;
        obj.target.transform.position = new Vector3(obj.target.transform.position.x, obj.target.transform.position.y, Mathf.Min(obj.target.transform.position.z, maxZ));

        if (block.moving && !done) {
        } else {
            obj.ChangeState(new PlayerIdleState());
        }
    }

    public override void Enter(StateMachine<PlayerController> obj) {
        obj.target.transform.SetParent(block.transform);
        obj.target.FreezeRigidbodyMovement();
        Vector3Int check = new Vector3Int(obj.target.position.x, obj.target.position.y,0);
        
        
        if (obj.target.generator.coordToBlock.ContainsKey(check)) {
            maxZ = - obj.target.generator.coordToBlock[check].currentPullLevel;
        } else {
            maxZ = 0;
        }


        maxZ += leeway;
        

    }

    public override void Exit(StateMachine<PlayerController> obj) {
        obj.target.transform.SetParent(obj.target.reParentTransform);
        obj.target.UnFreezeRigidbodyMovement();
    }

    public override bool isGamePlayState() {
        return true;
    }

}

public class NPCTalkState : State<PlayerController> {


    public NPCHuman human;

    public NPCTalkState(NPCHuman h) {
        human = h;
    }

    public override void Enter(StateMachine<PlayerController> obj) {
        obj.target.StartCoroutine(TalkNumerator(obj));
    }

    public IEnumerator TalkNumerator(StateMachine<PlayerController> obj) {

        obj.target.HideTalkPrompt();
        
        yield return null;
        human.textObject.transform.localScale = Vector3.zero;
        human.textObject.gameObject.SetActive(true);
        human.text.SetText("");
        while (human.textObject.transform.localScale != Vector3.one) {
            human.textObject.transform.localScale = Vector3.MoveTowards(human.textObject.transform.localScale, Vector3.one, 10 * Time.deltaTime);
            yield return null;
        }
        for (int i = 0; i < human.dialogue.dialogue.Count; i++) {
            human.text.SetText(human.dialogue.dialogue[i]);
            human.text.maxVisibleCharacters = 0;

            float timer = 0;
            float charTime = 0.05f;
            
            while (human.text.maxVisibleCharacters < human.dialogue.dialogue[i].Length) {
                if (obj.target.inputManager.interactDown) {
                    human.text.maxVisibleCharacters = human.dialogue.dialogue[i].Length;
                } if (Time.time > timer) {
                    human.text.maxVisibleCharacters += 1;
                    timer = Time.time + charTime;
                    yield return null;
                } else {
                    yield return null;
                }
            }

            while(!obj.target.inputManager.interactDown) {
                yield return null;
            }
            yield return null;
        }

        while (human.textObject.transform.localScale != Vector3.zero) {
            human.textObject.transform.localScale = Vector3.MoveTowards(human.textObject.transform.localScale, Vector3.zero, 10 * Time.deltaTime);
            yield return null;
        }

        human.textObject.SetActive(false);
        obj.ChangeState(new PlayerIdleState());
        obj.target.ShowTalkPrompt(human);

    }

}




public class WinState : State<PlayerController> {

    bool done = false;
    bool deployed = false;

    float startingy;
    float currenty;
    float speed = 5f;
    public override void Enter(StateMachine<PlayerController> obj) {
        GameMasterManager.instance.cutsceneManager.HideCutsceneManager();
        startingy = obj.target.transform.rotation.eulerAngles.y;
        currenty = startingy;
        GameMasterManager.instance.winScreen.gameObject.SetActive(true);
        obj.target.anim.SetBool(PlayerController.walking, false);
        obj.target.anim.SetBool(PlayerController.groundedString, true);
        obj.target.anim.SetTrigger(PlayerController.victoryDanceString);
        obj.target.generator.goal.HideTouchablePart();
        obj.target.floater.StopFloating();
        obj.target.FreezeRigidbodyMovement();
        GameMasterManager.instance.cutsceneManager.EndCurrentCutscene();

        float f = Vector3.Distance(obj.target.transform.position, GameMasterManager.instance.generator.goal.transform.position) * 5;
        if (f > speed) speed = f;

        if (GameMasterManager.currentGameMode == GameMode.WORKSHOPCOMPLETENESSCHECK) {



            if (GameMasterManager.currentLevelPack == null) {
                obj.ChangeState(new DeadState());
                obj.target.anim.ResetTrigger(PlayerController.victoryDanceString);
#if !DISABLESTEAMWORKS
               // GameMasterManager.instance.workShopManager.uploadUI.CompleteTestPuzzle();
#endif
            } else {

                GameMasterManager.currentLevelPackIndex++;
                if (GameMasterManager.currentLevelPackLevels.Count <= GameMasterManager.currentLevelPackIndex) {

                    GameMasterManager.currentLevelPackIndex = 0;
                    obj.ChangeState(new DeadState());
                    obj.target.anim.ResetTrigger("Dance");
#if !DISABLESTEAMWORKS
                  //  GameMasterManager.instance.workShopManager.uploadUI.CompleteTestPack();
#endif
                } else {
                    GameMasterManager.instance.StartLevel(GameMasterManager.currentLevelPackLevels[GameMasterManager.currentLevelPackIndex].saveFile, GameMasterManager.currentLevelPackLevels[GameMasterManager.currentLevelPackIndex], 0, true);
                }


            }
        }

       
    }
    public override void Update(StateMachine<PlayerController> obj) {
        done = true;
        if (obj.target.transform.position != GameMasterManager.instance.generator.goal.transform.position) {
            obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, GameMasterManager.instance.generator.goal.transform.position, speed * Time.deltaTime);
            done = false;
          
        }
        if (currenty != 180) {
            currenty = Mathf.MoveTowards(currenty, 180, 180 * Time.deltaTime);
            obj.target.transform.eulerAngles = new Vector3(0, currenty, 0);
            done = false;
        }
        if (done && !deployed) {

            if (GameMasterManager.currentGameMode == GameMode.STORYLEVEL && GameMasterManager.skipLevel && Environment.instance.endCutscen != null) {
                obj.ChangeState(new DeadState());
                GameMasterManager.instance.StartCoroutine(WinScreenEndSceneNum());
                return;
            }

            GameMasterManager.instance.winScreen.ShowWinScreen();
            deployed = true;
            
            obj.ChangeState(new DeadState());
          
        }


    }

    public IEnumerator WinScreenEndSceneNum() {

        yield return GameMasterManager.instance.StoryCutsceneNum(Environment.instance.endCutscen, false);
        GameMasterManager.instance.playerController.gameObject.SetActive(true);
        GameMasterManager.instance.winScreen.ShowWinScreen();

    }

}

public class DeadState : State<PlayerController> {

    public override void Enter(StateMachine<PlayerController> obj) {
        obj.target.anim.SetBool(PlayerController.groundedString, true);
    }


}


public class PushCrashState : State<PlayerController> {

    public State<PlayerController> nextState;
    public List<Block> block;
    Vector3 offset = new Vector3();
    bool playerMove;
    Block followBlock;

    public float blockSpeed = 0;
    bool playerAnim;

    bool sideMove = false;
    bool sideMoveLeft = false;

    public PushCrashState(List<Block> blocksToPush, Vector3Int direction, State<PlayerController> nstate, bool pMove, bool pAnim) {
        playerAnim = pAnim;
      //  Debug.Log("ENTER PUSH CRASH STATE " + Time.time.ToString());
        block = blocksToPush;
        nextState = nstate;
        playerMove = pMove;

        foreach (Block b in blocksToPush) {
            b.crashMovePosition = b.crashMovePosition + direction;
        }
        if (direction.y < 0) {
            blockSpeed = 8;
        } else {
            blockSpeed = 4;
        }


        if (pMove) {
            PlayerController controller = GameMasterManager.instance.playerController;

            Vector3 dir = controller.transform.forward;

            int aDir = 0;

            if (PlayerController.currentGrabbingDirection == new Vector3Int(0, 0, -1)) {

                if (direction.z == -1) {
                    Debug.Log("Push Forward");
                    aDir = 0;
                } else if (direction.z == 1) {
                    Debug.Log("Push Backward");
                    aDir = 1;
                } else if (direction.x == 1) {
                    Debug.Log("Push Left");
                    aDir = 3;
                } else {
                    Debug.Log("Push Right");
                    aDir = 2;
                }


            }
            if (PlayerController.currentGrabbingDirection == new Vector3Int(0, 0, 1)) {

                if (direction.z == -1) {
                    Debug.Log("Push Backward");
                    aDir = 1;
                } else if (direction.z == 1) {
                    Debug.Log("Push Forward");
                    aDir = 0;
                } else if (direction.x == 1) {
                    Debug.Log("Push Right");
                    aDir = 2;
                } else {
                    Debug.Log("Push Left");
                    aDir = 3;
                }


            }
            if (PlayerController.currentGrabbingDirection == new Vector3Int(1, 0, 0)) {
                if (direction.z == -1) {
                    Debug.Log("Push Right");
                    aDir = 2;
                } else if (direction.z == 1) {
                    Debug.Log("Push Left");
                    aDir = 3;
                } else if (direction.x == 1) {
                    Debug.Log("Push Forward");
                    aDir = 0;
                } else {
                    Debug.Log("Push Backward");
                    aDir = 1;
                }
            }
            if (PlayerController.currentGrabbingDirection == new Vector3Int(-1, 0, 0)) {
                if (direction.z == -1) {
                    Debug.Log("Push Left");
                    aDir = 3;
                } else if (direction.z == 1) {
                    Debug.Log("Push Right");
                    aDir = 2;
                } else if (direction.x == 1) {
                    Debug.Log("Push Backward");
                    aDir = 1;
                } else {
                    Debug.Log("Push Forward");
                    aDir = 0;
                }
            }

            if (aDir == 0) {

                PlayerController.instance.anim.SetFloat(PlayerController.pushDirX, 0);
                PlayerController.instance.anim.SetFloat(PlayerController.pushDirY, 1);
            } else if (aDir == 1) {

                PlayerController.instance.anim.SetFloat(PlayerController.pushDirX, 0);
                PlayerController.instance.anim.SetFloat(PlayerController.pushDirY, -1);
            } else if (aDir == 2) {

                PlayerController.instance.anim.SetFloat(PlayerController.pushDirX, 1);
                PlayerController.instance.anim.SetFloat(PlayerController.pushDirY, 0);
            } else {
                PlayerController.instance.anim.SetFloat(PlayerController.pushDirX, -1);
                PlayerController.instance.anim.SetFloat(PlayerController.pushDirY, 0);
            }
        }


    }

    public override void Enter(StateMachine<PlayerController> obj) {

        if (playerMove) {
            offset = obj.target.transform.position - block[0].transform.position;
            if (playerAnim) obj.target.anim.SetTrigger(PlayerController.pushingString);
            if (playerAnim) obj.target.anim.SetBool(PlayerController.walking, true);
            EventSystem.current.SetSelectedGameObject(null);
            obj.target.FreezeRigidbodyMovement();
            obj.target.floater.StopFloating();
            followBlock = block[0];
            
        }
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (playerMove)obj.target.transform.position = followBlock.transform.position + offset;

        bool done = true;

        foreach (Block b in block) {
            if (b.transform.localPosition != b.crashMovePosition) {
                b.transform.localPosition = Vector3.MoveTowards(b.transform.localPosition, b.crashMovePosition, blockSpeed * Time.deltaTime);
                TimeController.instance.AddCrashBlockMove(b);
                done = false;
            } else {
                TimeController.instance.AddCrashBlockMove(b);
                CrashMoveBlockPosition movePos = new CrashMoveBlockPosition();
                movePos.block = b;
                
                movePos.endPos = b.crashMovePosition;
                movePos.startPos = b.crashStartPosition;
               // movePos.endPos.y = (int)movePos.endPos.y;
              //  movePos.startPos.y = (int)movePos.startPos.y;
                TimeController.instance.AddCrashEndMove(movePos);
                
            }
        }

        if (done) {
            //   Debug.Log("Push State Done " + Time.time);
            obj.target.generator.FullySetCrashBlocks();
           
            obj.ChangeState(new GravityCheckState(nextState));
            
        }
            

       
    }

    public override void Exit(StateMachine<PlayerController> obj) {
        if (playerMove) {
            if (playerAnim) obj.target.anim.SetBool(PlayerController.walking, false);
        }
    }





    public override bool isGamePlayState() {
        return true;
    }




}

public class PlayerPushState : State<PlayerController> {

    public Block block;
    public bool sidePull;
    RaycastHit hitEnter;
    public Block blockUnderneath;
    bool isPullOutMovement = false;
    BlockDirection pulldir;


    public PlayerPushState(Block b, RaycastHit hit, bool alreadyAtTarget, bool isCrashPulloutButton, BlockDirection d) {
        hitEnter = hit;
        block = b;
        isPullOutMovement = isCrashPulloutButton;
        pulldir = d;

        

        if (hit.normal.x == 0) {

            

            targetPosition = new Vector3( Mathf.RoundToInt(hit.point.x), Mathf.Floor(hit.point.y), Mathf.RoundToInt(hit.point.z)) + hit.normal * 0.5f;
            //Debug.Log(targetPosition + " " + Time.time);
            sidePull = false;
            if (hit.normal.z > 0) {
                Debug.Log("Facing Back");
                PlayerController.currentGrabbingDirection = new Vector3Int(0, 0, -1);
            } else {
                Debug.Log("Facing Forward");
                PlayerController.currentGrabbingDirection = new Vector3Int(0, 0, 1);
            }
        } else {
            targetPosition = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, Mathf.Floor(hit.point.y), Mathf.Floor(hit.point.z) + 0.5f) + hit.normal * 0.5f;
            sidePull = true;
            if (hit.normal.x > 0) {
                Debug.Log("Facing Left");
                PlayerController.currentGrabbingDirection = new Vector3Int(-1, 0, 0);
            } else {
                Debug.Log("Facing Right");
                PlayerController.currentGrabbingDirection = new Vector3Int(1, 0, 0);
            }
        }
        targetRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

        // Debug.Log("Point: " + hit.point + " Target Position: " + targetPosition);
        AtTarget = alreadyAtTarget;
    }

    float bufferTime = 0;

    bool pulledOnceYet = false;

    Vector3 targetPosition;
    public Quaternion targetRotation;
    public bool AtTarget = false;

    static float lastPulloutTime = 0;

    public override void Enter(StateMachine<PlayerController> obj) {
       // Debug.Log("ENTER PUSH STATE: " + Time.time);
       
        EventSystem.current.SetSelectedGameObject(null);
        obj.target.FreezeRigidbodyMovement();
        obj.target.floater.StopFloating();
        if (obj.target.inputManager.vertical != 0) {
            
        }
        obj.target.grabSound.Play();
        obj.target.currentlyGrabbing = true;

        if (obj.target.position.y <= 0) {
            blockUnderneath = null;
        } else if ( BlockLevelGenerator.currentLevelType == LevelType.CRASH && obj.target.position.x >= 0 && obj.target.position.x <= obj.target.generator.crashGrid.GetLength(0) && obj.target.position.z >= 0 && obj.target.position.z <= obj.target.generator.crashGrid.GetLength(2) ) {
            //Debug.Break();
            blockUnderneath = obj.target.generator.crashGrid[obj.target.position.x, obj.target.position.y - 1, obj.target.position.z];
            if (blockUnderneath == null) {
            //    Debug.Log("Block Underneath NULL " + Time.time);
            } else {
           //     Debug.Log("BlockUnderneath " + blockUnderneath.gameObject.name + " " + Time.time);
            }
        } else {
            blockUnderneath = null;
        }

        if (!isPullOutMovement) {
          //  Debug.Log("ENTER NOT PULLOUT " + Time.time);
            obj.target.anim.SetTrigger(PlayerController.pushingString);
            obj.target.anim.SetBool(PlayerController.sidePushPull, sidePull);
        } else {
           // Debug.Log("ENTER PULLOUT " + Time.time);
            if (Time.time < lastPulloutTime) {
               // obj.ChangeState(new PlayerIdleState());
            }
            lastPulloutTime = Time.time + 0.25f;

        }

        block.HideCheck(false);
        prevmoveVal = obj.target.inputManager.vertical;

    }

    public override void Exit(StateMachine<PlayerController> obj) {
        //Debug.Log("EXIT PUSH STATE: " + Time.time);
        block.HideCheck(true);

    }

    Vector3 offset;

    public void PullLevelLogic(StateMachine<PlayerController> obj) {

        
        if (block.moving) {
            obj.target.anim.SetBool(PlayerController.walking, true);
            obj.target.transform.position = block.transform.position + offset;
            prevmoveVal = 0;
        } else {

            if (!obj.target.feetCheck.grounded) {

                obj.target.anim.SetTrigger("ReturnIdle");
                obj.target.UnFreezeRigidbodyMovement();
                obj.target.releaseSound.Play();
                obj.ChangeState(new PlayerIdleState());
                return;
            }
            if (obj.target.inputManager.pullButtonHeld) {

            } else {

                obj.target.anim.SetTrigger("ReturnIdle");
                obj.target.UnFreezeRigidbodyMovement();
                obj.target.releaseSound.Play();
                obj.ChangeState(new PlayerIdleState());
                return;
            }

            if (lastPull < Time.time) {
                pulledOnceYet = false;
            }

            bool pushStartedThisFrame = false;

            if (prevmoveVal == 0 && obj.target.inputManager.vertical != 0) {
                pushStartedThisFrame = true;
            }


            blockUnderneath = null;
            Vector3Int belowPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);

            if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {

                blockUnderneath = GameMasterManager.instance.generator.coordToBlock[belowPos];
            }


            if (pulledOnceYet) {
                DoPushPullCheck(obj, obj.target.controllerDeadZone);
            } else {
                DoPushPullCheck(obj, 0);
            }

            if (!block.moving) {
                if (pushStartedThisFrame && Time.time > bufferTime) {
                   // obj.target.errorSound.Play();
                    obj.target.anim.SetBool(PlayerController.walking, false);
                    if (obj.target.inputManager.vertical < 0 && blockUnderneath != block && block.currentPullLevel > 0) {
                        obj.ChangeState(new MoveShowError(this));
                    } else {
                        obj.target.errorSound.Play();
                    }
                }
                obj.target.anim.SetBool(PlayerController.walking, false);

            }

            prevmoveVal = obj.target.inputManager.vertical;
        }
       
    }

    
    float prevmoveVal = 0;

  

    public void CrashLevelLogic(StateMachine<PlayerController> obj) {


        if (!AtTarget) {

        }


        Vector3Int input = new Vector3Int();
        bool inpuThisFrame = false;

       // Debug.Log("dsadasdas" + Time.time);

        if (!isPullOutMovement) {
         //   Debug.Log("NOT PULLOUT" + Time.time);
            if (obj.target.inputManager.pullButtonHeld && obj.target.feetCheck.grounded) {

            } else {
                obj.target.anim.SetTrigger("ReturnIdle");
                obj.target.UnFreezeRigidbodyMovement();
                obj.target.releaseSound.Play();
                obj.target.anim.ResetTrigger(PlayerController.pushingString);
                obj.ChangeState(new PlayerIdleState());
                return;
            }

            if (Mathf.Abs(obj.target.worldMovement.z) > obj.target.controllerDeadZone && Time.time > bufferTime) {
                inpuThisFrame = true;
                if (obj.target.worldMovement.z > 0) {
                    input.z = 1;
                } else {
                    input.z = -1;
                }
            } else if (Mathf.Abs(obj.target.worldMovement.x) > obj.target.controllerDeadZone && Time.time > bufferTime) {
                inpuThisFrame = true;
                if (obj.target.worldMovement.x > 0) {
                    input.x = 1;
                } else {
                    input.x = -1;
                }
            }
        } else {
            input = new Vector3Int(LadderBlock.DirectionToVector(pulldir).x, 0, LadderBlock.DirectionToVector(pulldir).y);
        }

        blocksToPush = new List<Block>();

        if (inpuThisFrame || isPullOutMovement) {

            

            blocksToPush.Add(block);

            if ( !isPullOutMovement && blockUnderneath == block) {
           //     Debug.Log("Cant push block because currently starnding on it " + Time.time);
                return;
            }

            Vector3Int movePos = input + obj.target.position;
            if (movePos.x >= 0 && movePos.y >= 0 && movePos.z >= 0 &&
                movePos.x < obj.target.generator.crashGrid.GetLength(0) &&
                movePos.y < obj.target.generator.crashGrid.GetLength(1) &&
                movePos.z < obj.target.generator.crashGrid.GetLength(2)) {

                Block blockindir = obj.target.generator.crashGrid[movePos.x, movePos.y, movePos.z];

                if ( !isPullOutMovement &&  blockindir != null && blockindir != block) {
                 //   Debug.Log("Cant move in that dirction " + input + " " + Time.time);
                   
                  
                        Debug.Log("Early Return " + Time.time);
                       // obj.ChangeState( new BufferPullOutButtonState(new PlayerIdleState()));
                    
                    return;
                }

            }

            Block.blockingPlayer = new List<Block>();

            if (obj.target.position.y > 0) {
                underBlock = GameMasterManager.instance.generator.crashGrid[Mathf.Clamp(obj.target.position.x, 0, GameMasterManager.instance.generator.crashGrid.GetLength(0)-1),
                                                                            obj.target.position.y - 1,
                                                                            Mathf.Clamp(obj.target.position.z, 0, GameMasterManager.instance.generator.crashGrid.GetLength(2) - 1)];
            } else {
                underBlock = null;
            }
            if (block.CanMoveCrashBlockInThisDirection(input, blocksToPush, true)) {

                if (isPullOutMovement) {
                   // Debug.Log("IS PULL OUT " + Time.time);
                    obj.ChangeState(new PushCrashState(blocksToPush, input, new BufferPullOutButtonState( new PlayerIdleState()), true, false));

                } else {
                 //   Debug.Log("ISNT PULL OUT " + Time.time);
                    obj.ChangeState(new PushCrashState(blocksToPush, input, new PlayerPushState(block, hitEnter, true, isPullOutMovement, pulldir), true, true));
                }
            } else {
             //   Debug.Log("HERE " + Time.time);
            }
            underBlock = null;
        }


    }

    public static Block underBlock = null;

    public static List<Block> blocksToPush = new List<Block>();

    public void DoPushPullCheck(StateMachine<PlayerController> obj, float mindeadzonelevel) {
       

        if (obj.target.position.y > 0) {
            Vector3Int belowPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);

            if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {
                
                if (block == GameMasterManager.instance.generator.coordToBlock[belowPos]) return;
            }

        }


        //push
        //replace obj.target.inputmanager.vertical;
        if (obj.target.inputManager.vertical > mindeadzonelevel && Time.time > bufferTime) {
            if (block.currentPullLevel > 0) {
                Vector3Int OnPos = new Vector3Int(obj.target.position.x, obj.target.position.y,0);
                

                if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(OnPos) && GameMasterManager.instance.generator.coordToBlock[OnPos] != block) {
                    if (MathF.Abs(obj.target.position.z) <= GameMasterManager.instance.generator.coordToBlock[OnPos].currentPullLevel) {
                        //   Debug.Log("FIRST RETURN" + obj.target.position.z  + " " + GameMasterManager.instance.generator.coordToBlock[OnPos].currentPullLevel + " "  + GameMasterManager.instance.generator.coordToBlock[OnPos].name);
                        return;
                    }
                } else {
                    if (obj.target.position.z == 0) {
                        //    Debug.Log("SECOND RETURN");
                       
                        return;
                    }
                }

                if (block.isOpposite) {

                    Vector3Int blPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);
                    Block bl = null;
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(blPos)) {
                        bl = GameMasterManager.instance.generator.coordToBlock[blPos];
                        if (bl.isOpposite) return;
                    }


                    //SidePush Case
                    if (sidePull) {
                        Vector3Int vvv = new Vector3Int(obj.target.position.x, obj.target.position.y,0);
                        if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(vvv)) {
                            bl = GameMasterManager.instance.generator.coordToBlock[vvv];
                            if (bl != null && bl != block && bl.isOpposite && bl.isPositiveOpposite != block.isPositiveOpposite) {
                                if ((bl.currentPullLevel + 1) == ((-obj.target.position.z + 1) - 1)) {
                                    
                                    return;
                                }
                            }
                        }
                    }



                    foreach (Block b in obj.target.generator.positiveBlocks) {
                        TimeController.AddBlockMove(b, b.currentPullLevel);
                    }
                    foreach (Block b in obj.target.generator.negativeBlocks) {
                        TimeController.AddBlockMove(b, b.currentPullLevel);
                    }

                    if (block.isPositiveOpposite) {
                        if (!obj.target.generator.DecreaseOppositePullLevel()) return;
                    } else {
                        if (!obj.target.generator.IncreaseOppositeLevel()) return;
                    }
                    offset = obj.target.transform.position - block.transform.position;
                    
                    obj.target.pushPullSound.Play();
                    obj.target.AddPullStat();
                    bufferTime = Time.time + obj.target.pullBufferTime;
                    obj.target.anim.SetBool(PlayerController.walking, true);
                    pulledOnceYet = true;
                    return;
                }


                if (block.isUnion) {
                    Vector3Int belowPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);
                    Block below = null;
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {
                        below = GameMasterManager.instance.generator.coordToBlock[belowPos];
                        if (block.unionIndexes.Contains(below.colorIndex)) {
                            return;
                        }
                    }
                }
                
                offset = obj.target.transform.position - block.transform.position;
                TimeController.AddBlockMove(block, block.currentPullLevel);
                block.DecreasePullLevel();
                if (block.isUnion) {
                    foreach (Block b in obj.target.generator.blocks) {
                        if (b != block && block.unionIndexes.Contains(b.colorIndex) && !b.isOpposite) {
                            TimeController.AddBlockMove(b, b.currentPullLevel);
                            b.DecreasePullLevel();
                        }
                    }
                }
                obj.target.pushPullSound.Play();
                obj.target.AddPullStat();
                bufferTime = Time.time + obj.target.pullBufferTime;
                obj.target.anim.SetBool(PlayerController.walking, true);
                pulledOnceYet = true;
               // Debug.Log("THIRD RETURN");
                return;
            }
        }

       
        //pull
        if (obj.target.inputManager.vertical < -mindeadzonelevel && Time.time > bufferTime) {
            if (block.currentPullLevel < (block.generator.pullLevelDepth/*Block.zDistances.Length - 1*/)) {
                Vector3Int belowPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);

                if (GameMasterManager.instance.generator.HasPulloutAt(belowPos)) {

                    Block bl = null;
                    int zlevel = -obj.target.position.z + 1;
                    Debug.LogError("ZLEVEL  " + zlevel.ToString() + " " + Time.time.ToString());
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {
                        bl = GameMasterManager.instance.generator.coordToBlock[belowPos];
                        if ( !sidePull && (bl.currentPullLevel) == zlevel + 1) {
                            Debug.LogError("Here1Case");
                            return;
                        }
                        if (sidePull && (bl.currentPullLevel) == zlevel + 1) {
                            Debug.LogError("Here2Case");
                            return;
                        }
                    }
                }


                if (obj.target.position.y > 0) {
                    
                    
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {
                        Block b = GameMasterManager.instance.generator.coordToBlock[belowPos];
                        if (MathF.Abs(obj.target.position.z) + 1 == b.currentPullLevel) {
                            //Debug.Log("FOURTH RETURN");
                            return;
                        }
                    }
                   
                }

                if (block.isOpposite) {

                    Vector3Int blPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);
                    Block bl = null;
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(blPos)) {
                        bl = GameMasterManager.instance.generator.coordToBlock[blPos];
                        if (bl.isOpposite) return;
                    }


                    foreach (Block b in obj.target.generator.positiveBlocks) {
                        TimeController.AddBlockMove(b, b.currentPullLevel);
                    }
                    foreach (Block b in obj.target.generator.negativeBlocks) {
                        TimeController.AddBlockMove(b, b.currentPullLevel);
                    }

                    if (!block.isPositiveOpposite) {
                        if (!obj.target.generator.DecreaseOppositePullLevel()) return;
                    } else {
                        if (!obj.target.generator.IncreaseOppositeLevel()) return;
                    }
                    offset = obj.target.transform.position - block.transform.position;
                    
                    obj.target.pushPullSound.Play();
                    obj.target.AddPullStat();
                    bufferTime = Time.time + obj.target.pullBufferTime;
                    obj.target.anim.SetBool(PlayerController.walking, true);
                    pulledOnceYet = true;
                    return;
                }


                if (block.isUnion) {
                    belowPos = new Vector3Int(obj.target.position.x, obj.target.position.y - 1,0);
                    Block below = null;
                    if (GameMasterManager.instance.generator.coordToBlock.ContainsKey(belowPos)) {
                        below = GameMasterManager.instance.generator.coordToBlock[belowPos];
                        if (block.unionIndexes.Contains(below.colorIndex)) {
                            return;
                        }
                    }
                }


                offset = obj.target.transform.position - block.transform.position;
                TimeController.AddBlockMove(block, block.currentPullLevel);
                block.IncreasePullLevel();
                if (block.isUnion) {
                    foreach (Block b in obj.target.generator.blocks) {
                        if (b != block && block.unionIndexes.Contains(b.colorIndex) && !b.isOpposite) {
                            TimeController.AddBlockMove(b, b.currentPullLevel);
                            b.IncreasePullLevel();
                        }
                    }
                }
                obj.target.pushPullSound.Play();
                obj.target.AddPullStat();
                obj.target.generator.firstBlockPulledOutYet = true;
                bufferTime = Time.time + obj.target.pullBufferTime;
                pulledOnceYet = true;
                obj.target.anim.SetBool(PlayerController.walking, true);
               // Debug.Log("FIFTH RETURN");
                return;
            }
        }

       // Debug.Log("END RETURN");



    }

    public void SetLastPull() {
        lastPull = Time.time + 0.75f;
    }

    float lastPull = 0;

    public override void Update(StateMachine<PlayerController> obj) {

        if (!isPullOutMovement) {
            if (Block.blockingPlayer.Count > 0) {
                obj.target.anim.SetTrigger("ReturnIdle");
                obj.target.anim.ResetTrigger("Pushing");
                obj.target.UnFreezeRigidbodyMovement();
                obj.target.releaseSound.Play();
                obj.ChangeState(new PlayerIdleState());
                return;
            }

            if (!AtTarget && (obj.target.transform.position != targetPosition || obj.target.transform.rotation != targetRotation)) {
                obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, targetPosition, 5 * Time.deltaTime);
                obj.target.transform.rotation = Quaternion.RotateTowards(obj.target.transform.rotation, targetRotation, 360 * Time.deltaTime);
                return;
            } else if (!AtTarget) {
                AtTarget = true;
            }
        }


        if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
            PullLevelLogic(obj);
        } else {
            CrashLevelLogic(obj);
        }

    }


    public override bool isGamePlayState() {
        return true;
    }


}

public class MoveShowError : State<PlayerController> {

    public PlayerPushState state;

    bool outYet = false;

    public Vector3 target;
    public Vector3 offset;



    public MoveShowError(PlayerPushState p) {
        state = p;
        target = state.block.transform.position + new Vector3(0, 0, -0.25f);
        offset =  GameMasterManager.instance.playerController.transform.position - state.block.transform.position;
    }


    public override void Enter(StateMachine<PlayerController> obj) {
        
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (state.block.transform.position == target) {
            if (outYet == false) {
                outYet = true;
                target = state.block.transform.position - new Vector3(0, 0, -0.25f);
                obj.target.errorSound.Play();
            } else {
                obj.currentState = state;
            }
        } else {
            state.block.transform.position = Vector3.MoveTowards(state.block.transform.position, target, Time.deltaTime);
            GameMasterManager.instance.playerController.transform.position = state.block.transform.position + offset;
        }
    }



}



public class GravityCheckState : State<PlayerController> {


    public State<PlayerController> nextState;

    public GravityCheckState(State<PlayerController> state) {
        nextState = state;
    }

    public override void Enter(StateMachine<PlayerController> obj) {

        List<Block> blockToMove = new List<Block>();
        bool done = true;
        Block.blockingPlayer = new List<Block>();
        foreach (Block b in obj.target.generator.blocks) {
          //  Debug.Log(b.name + " " + Time.time);
            if (b.CanMoveCrashBlockInThisDirection(Vector3Int.down, new List<Block>(), true)) {
            //    Debug.Log("ADDED MOVE " + blockToMove.Count + " " + Time.time);
                done = false;
                blockToMove.Add(b);
            }
        }

        if (done) {
            //  Debug.Log("Gravity Done " + Time.time);
            obj.target.generator.BlockCheck();
            obj.ChangeState(nextState);
        } else {
           
            obj.ChangeState(new PushCrashState(blockToMove, Vector3Int.down, nextState, false, false));
        }


    }


}


public class PushOutBlockPreFall : State<PlayerController> {

    Block pBlock;
    

    public PushOutBlockPreFall(Block b) {
        pBlock = b;
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (pBlock.moving) {
            if (!obj.target.DirectGrounded()) {
                obj.ChangeState(new PushOutPostFall(pBlock));
            }
        } else {
            obj.ChangeState(new PlayerIdleState());
        }
    }

    public override bool isGamePlayState() {
        return true;
    }
}

public class PushOutPostFall : State<PlayerController> {

    Block pBlock;

    public PushOutPostFall(Block b) {
        pBlock = b;
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (pBlock.moving && !obj.target.DirectGrounded()) {
            obj.target.body.velocity = new Vector3(0, -40, 0);
        } else {
            obj.ChangeState(new PlayerIdleState());
        }
    }


    public override bool isGamePlayState() {
        return true;
    }
}


public class BufferPullOutButtonState : State<PlayerController> {

    public State<PlayerController> nextState;

    public BufferPullOutButtonState(State<PlayerController> nState) {
        nextState = nState;
    }

    public override void Enter(StateMachine<PlayerController> obj) {
        PulloutBlock.bufferTime = Time.time + 1f;
    }
    public override void Update(StateMachine<PlayerController> obj) {
        obj.ChangeState(nextState);
    }


}


public class PlayerDoNothingWhileInMenuState : State<PlayerController> {

    public PlayerDoNothingWhileInMenuState(bool b) {
        if (b) GameMasterManager.instance.generator.SetPlayerAtStartPostion();
    }

    public override void Enter(StateMachine<PlayerController> obj) {
        obj.target.FreezeRigidbodyMovement();
        obj.target.anim.SetBool(PlayerController.walking, false);
        obj.target.anim.SetTrigger("ReturnIdle");
        GameMasterManager.instance.gameplayUI.SetActive(false);
       
    }


    public override bool isGamePlayState() {
        return true;
    }
}

public class PlayerIdleState : State<PlayerController> {

    float lastGroundedTime = 0;
    bool isCurrentlyBlocked = false;


    public override bool isGamePlayState() {
        return true;
    }

    public override void Enter(StateMachine<PlayerController> obj) {

       // Debug.LogError("Player Starts Moving " + Time.time.ToString());


        GameMasterManager.instance.PlayEnvTrack();
        GameMasterManager.showCursorWithGamePad = false;
        

        obj.target.UnFreezeRigidbodyMovement();
        obj.target.feetCheck.NullRemoval();
        obj.target.floater.StartFloating();
        obj.target.EnableWarps();
        CursorObject.HideCursor();
        obj.target.anim.ResetTrigger(PlayerController.pushingString);
        obj.target.anim.SetTrigger(PlayerController.returnToIdleString);
        if (GameMasterManager.inGameArea && !GameMasterManager.gameSaveFile.hidegameplayUI) GameMasterManager.instance.gameplayUI.SetActive(true);
        else GameMasterManager.instance.gameplayUI.SetActive(false);
        if (Block.blockingPlayer.Count > 0) {
            isCurrentlyBlocked = true;
        }
        NavigationManager.DeselectObject();
        prevgrounded = obj.target.feetCheck.grounded;
        bCooldown = Time.time + 0.25f;
        //PlayerController.playerCanCollideWithPulloutBlocks = false;
        PulloutBlock.pulloutcooldown = 0;
        PlayerController.CannonCheck();

        //obj.target.rewindPrompt.SetActive(BlockLevelGenerator.currentLevelType == LevelType.PULL);

    }

    float bCooldown = 0;

    public override void Exit(StateMachine<PlayerController> obj) {
        obj.target.anim.SetBool(PlayerController.walking, false);
        obj.target.DisableWarps();
        obj.target.blocker.DisableBlockers();
    }

    float lastJump;

    bool prevgrounded = false;
    

    public override void Update(StateMachine<PlayerController> obj) {


        if (GameMasterManager.IsEditor() && Keyboard.current.digit0Key.wasPressedThisFrame) {
            obj.target.Win();
        }

        if (NavigationManager.instance.currentNavigationObject != null) {
            NavigationManager.DeselectObject();
        }
        if (CursorObject.instance.IsShowing()) {
            CursorObject.cursorShouldBeShowing = false;
            CursorObject.HideCursor();
        }

        obj.target.SetLastValidGridPosition();
        obj.target.feetCheck.NullRemoval();

        if (obj.target.feetCheck.grounded && Time.time > Blocker.lastEnableTime && obj.target.blocker.blockersEnabled) {
            obj.target.blocker.DisableBlockers();
        } 

        if (obj.target.feetCheck.grounded) {
            obj.target.blocker.MoveToPos(obj.target.lastvalidGridPosition);
        }
       // if (PlayerController.playerCanCollideWithPulloutBlocks && Time.time > bCooldown)PlayerController.playerCanCollideWithPulloutBlocks = true;
        obj.target.RespawnCheck();
        obj.target.CameraMoveLogic();


        if (obj.target.feetCheck.grounded && !prevgrounded ) {
            //jumpdir = Vector3.zero;
            obj.target.landSound.Play();
        }
        prevgrounded = obj.target.feetCheck.grounded;

        if (isCurrentlyBlocked) {
            Block aboveBlock = null;
            if (obj.target.position.x >= 0 && obj.target.position.x <= obj.target.generator.crashGrid.GetLength(0) &&
                obj.target.position.y >= 0 && obj.target.position.y <= obj.target.generator.crashGrid.GetLength(1) - 1 &&
                obj.target.position.z >= 0 && obj.target.position.z <= obj.target.generator.crashGrid.GetLength(2)) {
                aboveBlock = obj.target.generator.crashGrid[obj.target.position.x, obj.target.position.y  +1, obj.target.position.z];
            }
            if (aboveBlock == null) {
                obj.ChangeState(new GravityCheckState(new PlayerIdleState()));
                return;
            }

        }


        if (obj.target.inputManager.interactDown && !isCurrentlyBlocked) {
            if (PlayerController.currentLadderBlock != null) {
                obj.ChangeState(new PlayerWarpState(PlayerController.currentLadderBlock, PlayerController.currentLadderBlock.otherLadderBlock));
                return;
            } else if (obj.target.inStorySelectBox) {
                GameMasterManager.instance.ShowStoryMenu();
                obj.target.warpButton.gameObject.SetActive(false);
                obj.target.inStorySelectBox = false;
                CursorObject.ShowCursor();
                obj.ChangeState(new PlayerDoNothingWhileInMenuState(false));
                return;
            } else if (obj.target.talkToHuman != null) {
                obj.target.warpButton.gameObject.SetActive(false);
                obj.ChangeState(new NPCTalkState(obj.target.talkToHuman));
                obj.target.talkToHuman = null;
                return;
            } else if (PlayerController.currentCannonBlock != null) {
                obj.ChangeState(new DirectionCannonState(PlayerController.currentCannonBlock));
            } else if (PlayerController.currentLabMonitor != null) {
                PlayerController.currentLabMonitor.Click();
            }

            
        }

        if (obj.target.inputManager.timeRewindButtonDown && GameMasterManager.inGameArea && !isCurrentlyBlocked) {
           // Debug.LogError("HERE sda ds");
            if (BlockLevelGenerator.currentLevelType == LevelType.PULL) {
                obj.ChangeState(new TimeRewindState());
            } else {
                obj.ChangeState(new CrashRewindState());
            }
        }

        if (obj.target.inputManager.escKeyDown && Time.time > obj.target.pauseBuffer && !isCurrentlyBlocked) {
            
            obj.ChangeState(new PlayerPauseState());
            return;
        }

        // Vector2 v = new Vector3(obj.target.inputManager.horizontal, obj.target.inputManager.vertical);
        Vector2 v = new Vector2(obj.target.worldMovement.x, obj.target.worldMovement.z);
        if (v.magnitude >= 0.9f) {
            v = v.normalized * 0.9f;
          //  Debug.Log(v.magnitude);
        }

        bool cubbyCheck = false;
        if (!obj.target.feetCheck.grounded && obj.target.body.velocity.y < 0 && (obj.target.inputManager.horizontal != 0 || obj.target.inputManager.vertical != 0) && obj.target.CubbyCheck()) {
           // Debug.Log("Cubby Check " + Time.time);
            cubbyCheck = true;
        }

        obj.target.anim.SetBool(PlayerController.groundedString, obj.target.feetCheck.grounded);

        if (obj.target.feetCheck.grounded) {
            
            obj.target.body.velocity = new Vector3(v.x * obj.target.moveSpeed, obj.target.body.velocity.y + Time.deltaTime, v.y * obj.target.moveSpeed);
        } else {

            if (cubbyCheck) {
                obj.target.body.velocity = new Vector3(v.x * obj.target.moveSpeed, obj.target.body.velocity.y, v.y * obj.target.moveSpeed);
            } else {
                obj.target.body.velocity = new Vector3(v.x * obj.target.airMoveSpeed, obj.target.body.velocity.y, v.y * obj.target.airMoveSpeed);
            }

            // float y = obj.target.body.velocity.y;
            // Vector3 jumptarget = jumpdir * obj.target.airMoveSpeed;
            // jumptarget.y = y;
            // obj.target.body.velocity = Vector3.MoveTowards(obj.target.body.velocity, jumptarget , obj.target.airMoveSpeed/2);
            // obj.target.body.velocity = new Vector3(obj.target.body.velocity.x, y, obj.target.body.velocity.z);
            //if (jumpdir == Vector3.zero) jumpdir = Vector3.ClampMagnitude(new Vector3(obj.target.body.velocity.x, 0, obj.target.body.velocity.z), 1);
        }

        if (obj.target.feetCheck.grounded && obj.target.feetCheck.onlyTouchingBack && obj.target.transform.position.y > 0.5f) {
            obj.target.body.velocity -= Vector3.forward * 20 * Time.deltaTime;
            PlayerController.cantJumpTime = Time.time + 0.25f;
        }

        if (obj.target.feetCheck.grounded) lastGroundedTime = Time.time;
        if (obj.target.inputManager.horizontal != 0 || obj.target.inputManager.vertical != 0) {
            obj.target.transform.forward = new Vector3(v.x, 0,v.y);
            obj.target.anim.SetBool(PlayerController.walking, true);
        } else {
            obj.target.anim.SetBool(PlayerController.walking, false);
        }

        


        //Debug.LogError("Right Before Jump" + Time.time.ToString());
        if (obj.target.inputManager.jumpDown) {
            //  Debug.LogError("?????");
            if (Time.time > PlayerController.cantJumpTime &&
                !isCurrentlyBlocked &&
                (obj.target.feetCheck.grounded || (((lastGroundedTime + obj.target.coyoteTime) >= Time.time) && (Time.time > lastJump))) &&
                !obj.target.feetCheck.onlyTouchingBack) {

               // Debug.LogError("Jump GOOD " + Time.time.ToString());


                obj.target.body.velocity = new Vector3(obj.target.body.velocity.x, obj.target.jumpSpeed, obj.target.body.velocity.z);
                lastJump = Time.time + obj.target.coyoteTime + 0.1f;
                obj.target.anim.SetTrigger(PlayerController.jumpString);
                obj.target.jumpSound.Play();
                obj.target.blocker.EnableBlockers();
                //Debug.Break();
                GameMasterManager.gameSaveFile.trackerSaveFile.AddJumps(1);
                //  jumpdir = Vector3.ClampMagnitude(new Vector3(obj.target.body.velocity.x, 0, obj.target.body.velocity.z), 1);

            } else {
            //    Debug.LogError("HERE");
              //  if (!(Time.time > PlayerController.cantJumpTime)) Debug.LogError("First Reason");
                //if (isCurrentlyBlocked) Debug.LogError("Second Reason");
               // if (!(obj.target.feetCheck.grounded || (((lastGroundedTime + obj.target.coyoteTime) >= Time.time) && (Time.time > lastJump)))) Debug.LogError("Third Reason");
               // if (obj.target.feetCheck.onlyTouchingBack) Debug.LogError("Fourth Reason");

            }
        }

        bool xskip = (Mathf.Abs(obj.target.transform.position.x - (Mathf.RoundToInt(obj.target.transform.position.x) + 0.5f)) < 0.05f);
        float amt = 0;
        if (xskip) {
            amt = -0.1f;//Mathf.Abs(Mathf.Abs(obj.target.transform.position.x - (Mathf.RoundToInt(obj.target.transform.position.x) + 0.5f)));
            xskip = false;
        }
        

        if (obj.target.inputManager.pullButtonHeld && (obj.target.feetCheck.grounded) && !isCurrentlyBlocked) {
            if (Physics.Raycast(obj.target.transform.position + new Vector3(amt ,0.5f,0), obj.target.transform.forward, out obj.target.hit, 0.75f, obj.target.blockLayer)) {
                //obj.target.transform.LookAt(obj.target.hit.point);

                

                Vector3 targetPosition = new Vector3(Mathf.Floor(obj.target.hit.point.x) + 0.5f, Mathf.Floor(obj.target.hit.point.y), Mathf.Floor(obj.target.hit.point.z) + 0.5f) + obj.target.hit.normal * 0.5f;

            //    Debug.Log((int)targetPosition.x + " target " + obj.target.position.x + " position ");

                bool zskip = obj.target.hit.normal.x != 0 && (((int)targetPosition.z) != obj.target.position.z);

                


                if ( BlockLevelGenerator.currentLevelType == LevelType.PULL && (xskip || zskip)  ) {
                   
                    if (xskip) Debug.LogError("XSkipped " + Time.time);
                    if (zskip) Debug.LogError("ZSkipped " + Time.time);

                } else {
                   // Debug.Log(obj.target.hit.point + " " + obj.target.position.z + Time.time);
                    Block b = null;
                    if (obj.target.hit.collider.gameObject.layer == Layers.Ground) {
                        b = obj.target.hit.collider.transform.GetComponent<BlockProxy>().block;
                    } else {
                        b = obj.target.hit.collider.GetComponent<Block>();
                    }
                    if (b == null) return;
                    if (b.isBlocked) return;
                    if (b.moving) return;
                    obj.ChangeState(new PlayerPushState(b, obj.target.hit, false, false, BlockDirection.UP));
                    return;
                }
            }
        }

        if (!obj.target.feetCheck.grounded && obj.target.body.velocity.y < 0) {
            obj.target.body.velocity -= new Vector3(0, 10, 0) * Time.deltaTime;
            obj.target.body.velocity = new Vector3(obj.target.body.velocity.x, Mathf.Max(obj.target.body.velocity.y, obj.target.maxnegativeVelocity), obj.target.body.velocity.z);
        }


        if (obj.target.inputManager.hintDown) {

            if (!GameMasterManager.instance.cutsceneManager.isCutscenePlaying) {

                BlockSaveFile currentLevel = GameMasterManager.currentLevel.saveFile;

                if (currentLevel.messages != null && currentLevel.messages.Count > 0) {

                    GameMasterManager.instance.generator.stringCutscene.SetMessages(currentLevel.messages);
                    GameMasterManager.instance.generator.hintsAvilable = true;
                    GameMasterManager.instance.cutsceneManager.currentCutscene = GameMasterManager.instance.generator.stringCutscene;
                    GameMasterManager.instance.cutsceneManager.ShowCutscene(GameMasterManager.instance.generator.stringCutscene);

                } else if (GameMasterManager.instance.cutsceneManager.currentCutscene != null && GameMasterManager.instance.cutsceneManager.currentCutscene.CanReplayCutscene()) {
                    GameMasterManager.instance.cutsceneManager.ShowCutscene(GameMasterManager.instance.cutsceneManager.currentCutscene);
                }
            }
        }

       // if (Input.GetMouseButtonDown(0)) {
     //       Block hit = obj.target.GetClickedBlock();
      //      if (hit != null) {
      //          hit.IncreasePullLevel();
      //      }
     //   }

     //   if (Input.GetMouseButtonDown(1)) {
      //      Block hit = obj.target.GetClickedBlock();
       //     if (hit != null) {
       //         hit.DecreasePullLevel();
       //     }
      //  }

        if (obj.target.inputManager.cameraButtonHeld && obj.target.feetCheck.grounded && GameMasterManager.inGameArea && !isCurrentlyBlocked) {
            obj.ChangeState(new ViewLevelCameraState());
        }

       
    }
}


public class PlayerWarpState : State<PlayerController> {

    public LadderBlock start, end;

    public PlayerWarpState(LadderBlock s, LadderBlock e) {
        start = s;
        end = e;
    }

    public override void Enter(StateMachine<PlayerController> obj) {
        obj.target.FreezeRigidbodyMovement();
        obj.target.StopAllCoroutines();
        obj.target.StartCoroutine(Warp(obj.target));
        obj.target.portalSound.Play();
        GameMasterManager.gameSaveFile.trackerSaveFile.AddPortals(1);
    }

    public IEnumerator Warp(PlayerController player) {

        Vector3 targetPos = start.GetWarpToPos();
        player.warpButton.gameObject.SetActive(false);
        player.warpingEnabled = false;
        player.FreezeRigidbodyMovement();
        while (player.transform.position != targetPos || player.transform.localScale != Vector3.zero) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, player.warpMoveSpeed * Time.deltaTime);
            player.transform.localScale = Vector3.MoveTowards(player.transform.localScale, Vector3.zero, player.warpShrinkSpeed * Time.deltaTime);
            yield return null;
        }


      //  Debug.Log("End Start: " + end.GetWarpToPos().ToString() + " End End: " + end.GetWarpPlacementPosition());

        player.transform.position = end.GetWarpToPos();
        player.body.position = end.GetWarpToPos();
       // Debug.Log("Player: " + player.transform.position);
        yield return null;
      //  Debug.Log("Player2: " + player.transform.position);
        targetPos = end.GetWarpPlacementPosition();

        while (player.transform.position != targetPos || player.transform.localScale != Vector3.one) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, player.warpMoveSpeed * Time.deltaTime);
            player.transform.localScale = Vector3.MoveTowards(player.transform.localScale, Vector3.one, player.warpShrinkSpeed * Time.deltaTime);
            yield return null;
        }

        player.UnFreezeRigidbodyMovement();

        player.machine.ChangeState(new PlayerIdleState());


    }

    public override void Exit(StateMachine<PlayerController> obj) {
        obj.target.portalSound.Play();
    }


}

public class PlayerPauseState : State<PlayerController> {

    public override void Enter(StateMachine<PlayerController> obj) {
        GameMasterManager.instance.ShowPauseScreen();
        CursorObject.SetDefaultCursorSprite();
        CursorObject.ShowCursorIfNeeded();
        obj.target.body.velocity = Vector3.zero;
        //obj.target.body.isKinematic = true;
        obj.target.FreezeRigidbodyMovement();
        Time.timeScale = 1;
        GameMasterManager.instance.gameplayUI.SetActive(false);
    }


    public override void Update(StateMachine<PlayerController> obj) {
        if (GameMasterManager.paused == false || ((obj.target.inputManager.escKeyDown || obj.target.inputManager.cancelButtonDown) && Time.time > obj.target.pauseBuffer)) {
            GameMasterManager.instance.HidePauseScreen();
            obj.ChangeState(new PlayerIdleState());
            obj.target.pauseBuffer = Time.time + 0.25f;
        }
    }


    public override void Exit(StateMachine<PlayerController> obj) {
        //obj.target.body.isKinematic = false;
        obj.target.UnFreezeRigidbodyMovement();
        Time.timeScale = 1;
        GameMasterManager.instance.gameplayUI.SetActive(true);
    }
}

public class ViewLevelCameraState : State<PlayerController> {

    public override void Enter(StateMachine<PlayerController> obj) {
        GameMasterManager.instance.generator.levelName.enabled = false;
        GameMasterManager.instance.cameraManager.ShowLevelViewerCamera();
        obj.target.body.velocity = Vector3.zero;
    }

    public override void Update(StateMachine<PlayerController> obj) {
        if (!obj.target.inputManager.cameraButtonHeld) {
            GameMasterManager.instance.cameraManager.ShowPlayerCamera(true);
            obj.ChangeState(new PlayerIdleState());
        }
    }

    public override void Exit(StateMachine<PlayerController> obj) {
        GameMasterManager.instance.generator.levelName.enabled = true;
    }

    public override bool isGamePlayState() {
        return true;
    }
}

public class DirectionCannonState : State<PlayerController> {

    public DirectionalCannon cannon;

    public bool goingDown = true;

    public Vector3 down;
    public Vector3 target;


    public DirectionCannonState(DirectionalCannon d) {
        cannon = d;
    }

    float cannonSpeed;

    public override void Enter(StateMachine<PlayerController> obj) {
        goingDown = true;
        down = cannon.transform.position - new Vector3(0,1f,0);
        target = cannon.GetPointSpot();
        obj.target.FreezeRigidbodyMovement();

        cannonSpeed = Vector3.Distance(target, obj.target.transform.position);

        cannonSpeed = Mathf.Max(obj.target.playerCannonSpeed, cannonSpeed);


        switch (cannon.direction) {
            case BlockDirection.UP:
               // obj.target.transform.rotation = Quaternion.identity;
                break;
            case BlockDirection.DOWN:
               /// obj.target.transform.rotation = Quaternion.identity;
                break;
            case BlockDirection.LEFT:
               // obj.target.transform.rotation = Quaternion.Euler(0,0,90);
                break;
            case BlockDirection.RIGHT:
              //  obj.target.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

    }

    public override void Exit(StateMachine<PlayerController> obj) {
        obj.target.UnFreezeRigidbodyMovement();
      //  obj.target.transform.rotation = Quaternion.identity;

    }

    public override void Update(StateMachine<PlayerController> obj) {
        

        if (goingDown) {
            if (obj.target.transform.position != down) {
                obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, down, cannonSpeed * Time.deltaTime);
            } else {
                goingDown = false;
            }
        } else {
            if (obj.target.transform.position != target) {
                obj.target.transform.position = Vector3.MoveTowards(obj.target.transform.position, target, cannonSpeed * Time.deltaTime);
            } else {
                obj.ChangeState(new PlayerIdleState());
            }
        }


    }
}

#endregion
