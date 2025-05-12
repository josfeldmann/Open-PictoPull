using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix;
//using Sirenix.OdinInspector;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlightNode : MonoBehaviour
{

    public bool drawGizmos = false, drawSpheres = false;
    public bool streetNode = false;

    private void Awake() {
        RemoveNullNodes();    
    }

    public List<FlightNode> nodes = new List<FlightNode>();
    public float radius = 1f;


  //  [Button]
    public void MakeNeighbour() {

       FlightNode n =  Instantiate(this, this.transform.parent);
        nodes.Add(n);
        n.nodes = new List<FlightNode>() { this };
        n.transform.position = transform.position + new Vector3(1, 0, 0);
        n.gameObject.name = "FlightNode";
#if UNITY_EDITOR
        Selection.activeObject = n.gameObject;
#endif

    }

    public void RemoveNullNodes() {
        nodes.RemoveAll( t => t ==null );
    }

    public FlightNode NewStartingNode(FlightNode prevNode) {
        if (prevNode == null) return nodes.PickRandom();
        List<FlightNode> ns = new List<FlightNode>();
        foreach (FlightNode dd in nodes) {
            if (dd != prevNode) {
                ns.Add(dd);
            }
        }
        if (ns.Count == 0) {
            return nodes.PickRandom();
        }
        return ns.PickRandom();
    }

    public FlightNode GetNextNode(Flyer flyer) {
        List<FlightNode> f = new List<FlightNode>();

        foreach (FlightNode d in nodes) {
            if (!flyer.visitedNodes.Contains(d)) {
                f.Add(d);
            }
        }
        if (f.Count == 0) return null;
        return f.PickRandom();


    }

    //[Button("Connect Selected Nodes")]
    public void ConnectSelectedNodes() {
#if UNITY_EDITOR
        FlightNode[] n = Selection.GetFiltered<FlightNode>(SelectionMode.Unfiltered);
        foreach (FlightNode nn in n) {
            if (nn != this && !nodes.Contains(nn)) {
                nodes.Add(nn);
            }
        }
#endif
    }




    private void OnDrawGizmos() {

        if (!drawGizmos) return;
        if (drawSpheres) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, radius);
        }

        foreach (FlightNode n in nodes) {
            if (n != null) {
                if (n.GetInstanceID() > GetInstanceID()) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, n.transform.position);
                }
            }
        }


    }

}
