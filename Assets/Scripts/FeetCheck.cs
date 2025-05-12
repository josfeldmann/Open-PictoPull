using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCheck : MonoBehaviour
{
    
    public LayerMask layer;
    public bool grounded = true;
    public bool onlyTouchingBack = false;
    public List<Collider> touchingObjects = new List<Collider>();
    public List<Block> touchingBlocks = new List<Block>();





    public void NullRemoval() {
        touchingObjects.RemoveAll(item =>( (item == null) || item.enabled == false || !item.gameObject.activeInHierarchy));
        touchingBlocks.RemoveAll(item => ((item == null) || !item.gameObject.activeInHierarchy));
        if (touchingObjects.Count == 0) grounded = false;
        else grounded = true;
        BackCheck();
    }



    private void OnTriggerEnter(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, layer) && !touchingObjects.Contains(other)){
            touchingObjects.Add(other);
            grounded = true;
            Block b = other.GetComponent<Block>();
            if (b != null) {
                if (!touchingBlocks.Contains(b)) {
                    touchingBlocks.Add(b);
                    BackCheck();
                }
            }
        }

       // Debug.Log(other.tag + Time.time.ToString());
        if (other.tag.Equals(Layers.blockerTag)) {
            //Debug.Break();
            if (transform.position.y > other.transform.position.y) {
                BlockerMesh b = other.transform.GetComponent<BlockerMesh>();
                if (b != null)b.Disable();
            }
        }
    }

    public void BackCheck() {
        onlyTouchingBack = true;
        if (touchingBlocks == null || touchingBlocks.Count == 0) {
            onlyTouchingBack = false;
        }
        foreach (Block b  in touchingBlocks) {
            if (b.sideMeshCollider.enabled) onlyTouchingBack = false;
        } 
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, layer) && touchingObjects.Contains(other)) {
            touchingObjects.Remove(other);
            if (touchingObjects.Count == 0) grounded = false;
            Block b = other.GetComponent<Block>();
            if (b != null) {
                if (touchingBlocks.Contains(b)) {
                    touchingBlocks.Remove(b);
                    BackCheck();
                }
            }
        }
    }



}
