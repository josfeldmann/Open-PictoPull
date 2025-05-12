using System;
using UnityEngine;

public class DirectionalCannon : MonoBehaviour {

    public MeshRenderer cannonRenderer;
    public BlockDirection direction;
    public Vector3Int position;
    public SpriteRenderer spriteIcon;
    

    public BoxCollider rightCol, leftCol, upCol, downCol;
    public LayerMask playerLayer;
    public Transform rotDir;
    public Block block;

    public void OnTriggerEnter(Collider other) {
       // Debug.Log(other.gameObject.name);
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            if (block.currentPullLevel <= 0) return;
            if (!PlayerController.cannonBlocks.Contains(this)) {
                PlayerController.AddCannon(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            if (PlayerController.cannonBlocks.Contains(this)) {
                PlayerController.RemoveCannon(this);
            }
        }
    }


    public void CannonCheck() {
        if (block.currentPullLevel <= 0) {
           // transform.localScale = Vector3.zero;
        } else {
           // transform.localScale = Vector3.one;
        }
    }


    public void SetColor(int v, Block block, Color c, BlockLevelGenerator blockLevelGenerator, Vector2Int position, BlockDirection direction) {

        spriteIcon.color = c;
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        propBlock.SetColor("_Color", c);
        propBlock.SetColor("_CrossColor", c);
        cannonRenderer.SetPropertyBlock(propBlock);
    }


    public void SetDirection(BlockDirection b) {

        Vector3Int v = LadderBlock.DirectionToVector(b);
        direction = b;
        spriteIcon.transform.up = -new Vector3(v.x, v.y,0);
        rotDir.localEulerAngles = new Vector3(0, 0, BlockLevelCreator.DirectionToZRotation(b) + 180);
        rightCol.enabled = false;
        leftCol.enabled = false;
        upCol.enabled = false;
        downCol.enabled = false;

        switch (b) {
            case BlockDirection.UP:
                upCol.enabled = true;
                break;
            case BlockDirection.DOWN:
                downCol.enabled = true;
                break;
            case BlockDirection.LEFT:
                leftCol.enabled = true;
                break;
            case BlockDirection.RIGHT:
                rightCol.enabled = true;
                break;
            case BlockDirection.FORWARD:
                break;
            case BlockDirection.BACKWARD:
                break;
        }


    }

    public Vector3 GetPointSpot() {


        Vector3 castPositon = new Vector3();
        switch (direction) {
            case BlockDirection.UP:
                castPositon = upCol.transform.position + upCol.center; ;
                break;
            case BlockDirection.DOWN:
                castPositon = downCol.transform.position + downCol.center; ;
                break;
            case BlockDirection.LEFT:
                castPositon = leftCol.transform.position + leftCol.center;
                break;
            case BlockDirection.RIGHT:
                castPositon = rightCol.transform.position + rightCol.center; ;
                break;
            case BlockDirection.FORWARD:
                break;
            case BlockDirection.BACKWARD:
                break;
        }

        RaycastHit hit;

        Vector3Int d = LadderBlock.DirectionToVector(direction);

        Vector3 dir = new Vector3(d.x, d.y, 0);
        float distance = 0;
        if (dir.x == 0)  distance = Mathf.Max(1, GameMasterManager.instance.generator.save.height - position.y - 1);
        else distance = Mathf.Max(1, GameMasterManager.instance.generator.save.width - position.x - 1);

        Vector3 result = castPositon + dir * distance;

        if (Physics.Raycast(castPositon, dir, out hit, distance, GameMasterManager.instance.playerController.blockLayer)) {
            result = hit.point + hit.normal * 0.5f;
        }
        return result - new Vector3(0,0.5f,0);
    }
}
