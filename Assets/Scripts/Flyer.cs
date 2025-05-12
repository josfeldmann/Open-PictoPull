using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    public bool isCart;
    public bool isHuman;
    public float humanOffset = 1f;
    public float currentHumanOffset;
    public Vector3 position;
    public FlightNode targetNode;
    public FlightNode prevNode;
    public float speed = 5f;
    public float turnSpeed = 180f;
    public Animator anim;

    public Transform birdTransform;
    public List<FlightNode> visitedNodes = new List<FlightNode>();
    public List<MeshRenderer> renderers;
    public CharacterObject cobj;

    private void Awake() {
       if (anim) anim.SetBool("Walking", true);
       if (isHuman) {
            foreach (MeshRenderer renderer in renderers) {
                renderer.material = cobj.mat;
            }
        }
        StartOnNode(targetNode);
        
    }

    public void StartOnNode(FlightNode f) {
        f.RemoveNullNodes();
        transform.position = f.transform.position;
        GetNewNode();
        if (isHuman) currentHumanOffset = Random.Range(-humanOffset, humanOffset);
    }

    public void GetNewNode() {
        FlightNode f = targetNode.GetNextNode(this);
        if (!visitedNodes.Contains(targetNode)) visitedNodes.Add(targetNode);
        if (f == null) {
            visitedNodes = new List<FlightNode>();
            f = targetNode.NewStartingNode(prevNode);
            if (isHuman) currentHumanOffset = Random.Range(-humanOffset, humanOffset);
        }
        prevNode = targetNode;
        targetNode = f;

        Vector3 offset = new Vector3(Random.Range(-f.radius, f.radius), Random.Range(-f.radius, f.radius), Random.Range(-f.radius, f.radius));
        offset = Vector3.ClampMagnitude(offset, f.radius);
        if (isCart) offset = Vector3.zero;
        if (isHuman) offset = targetNode.transform.right * currentHumanOffset;
        position = f.transform.position + offset;
        if (isHuman) {
            Vector3 v = (position - transform.position);
            v.y = 0;
            transform.forward = v;
        }
    }


    private void Update() {
        
        if (transform.position == position) {

            GetNewNode();

        } else {
            
            
            if (!isHuman) transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            else transform.position = Vector3.MoveTowards(transform.position, position, cobj.characterWalkSpeed * Time.deltaTime);

            Vector3 vv = position - transform.position;

            if (!isHuman && vv != Vector3.zero)transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((vv).normalized, transform.up), turnSpeed * Time.deltaTime);
            
        }

        if (!isCart && !isHuman)birdTransform.localPosition = new Vector3(0, Mathf.Sin(Time.time), 0);


    }


}
