using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTileHolder : MonoBehaviour
{
    public TileMesh wallPrefab, frontTile, backBackTile;

    public List<TileMesh> tileList;

    public void SetRange(int amt) {

        amt -= 2;

        frontTile.transform.localPosition = new Vector3(0, 0, -5.5f);
        backBackTile.transform.localPosition = new Vector3(0, 0, -5.5f + (amt+1));

        if (amt > tileList.Count) {
            int toAdd = amt - tileList.Count;
            for (int i = 0; i < toAdd; i++) {
                TileMesh t = Instantiate(wallPrefab, transform);
                tileList.Add(t);
            }
        }

        for (int i = 0; i < tileList.Count; i++) {
            if (i < amt) {
                tileList[i].gameObject.SetActive(true);
                tileList[i].transform.localPosition = frontTile.transform.localPosition + new Vector3(0, 0, i + 1f);
            } else {
                tileList[i].gameObject.SetActive(false);
            }
        }

       

    }
    

}
