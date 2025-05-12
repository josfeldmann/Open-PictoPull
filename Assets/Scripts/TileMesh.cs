using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMesh : MonoBehaviour
{
    public MeshRenderer mesh;
    public MeshFilter meshFilter;
    public Vector2Int index;
    public bool isWall;
    bool added = false;
    private void Awake() {
        Add();
        if (meshFilter == null && mesh != null) meshFilter = mesh.GetComponent<MeshFilter>();
    }

    public void Add() {
        if (isWall) {
          //  if (!BlockLevelGenerator.wallMeshes.Contains(this)) BlockLevelGenerator.wallMeshes.Add(this);
        } else {
           // if (!BlockLevelGenerator.topMeshes.Contains(this)) BlockLevelGenerator.topMeshes.Add(this);
        }
        added = true;
    }



    public void SetIndex(Vector2Int v) {
        index = v;
    }

    public void SetMaterial(Material mat) {
        if (mesh.material != mat)mesh.material = mat;
    }

    public void SetMesh(Mesh m) {
        meshFilter.sharedMesh = m;
    }

}
