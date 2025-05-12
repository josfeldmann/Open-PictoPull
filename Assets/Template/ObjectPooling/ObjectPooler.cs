using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour, IObjectPooler
{

    private void Awake() {
        if (objPrefab != null) {
            InitializePool();
        } else {
            Debug.Log("Pooled Object is empty");
        }
    }

    public int initialPoolSize = 10;
    public PooledObject objPrefab;
    public Transform inactiveTransform;
    private List<PooledObject> inactiveObjects = new List<PooledObject>();
    public void InitializePool() {
        ClearInactiveObjects();
        AddXPooledObjectsToPool(initialPoolSize);
    }
    public void ClearInactiveObjects() {
        foreach (PooledObject obj in inactiveObjects) {
            GameObject.Destroy(obj);
        }
        inactiveObjects = new List<PooledObject>();
    }
    public void AddXPooledObjectsToPool(int x) {
        for (int i = 0; i < x; i++) {
            PooledObject p = Instantiate(objPrefab, inactiveTransform);
            p.gameObject.SetActive(false);
            p.pooler = this;
            inactiveObjects.Add(p);
        }
    }
    public PooledObject CreateObjectAt(Vector3 position, Quaternion rotation) {
        if (inactiveObjects.Count <= 0) {
            AddXPooledObjectsToPool(1);
        }
        PooledObject p = inactiveObjects[0];
        p.transform.SetParent(null);
        p.gameObject.SetActive(true);
        p.transform.position = position;
        p.transform.rotation = rotation;
        inactiveObjects.RemoveAt(0);
        return p;
    }
    public void ReturnPooledObject(PooledObject obj) {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(inactiveTransform);
        inactiveObjects.Add(obj);
    }


    

    [ContextMenu("Test")]
    public void TestMethod() {
       PooledObject p =  CreateObjectAt(new Vector3(), Quaternion.identity);
       p.ReturnToPoolInXSeconds(5);
    }

}


public interface IObjectPooler {

    public void InitializePool();
    public PooledObject CreateObjectAt(Vector3 position, Quaternion rotation);
    public void ReturnPooledObject(PooledObject obj);

}
