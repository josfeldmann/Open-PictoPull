//using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{

    public Vector3 size = new Vector3Int(16, 0, 16);
    public Vector3 distanceInbetween = new Vector3Int(2, 0, 1);
    public Vector2 variation = new Vector2(-0.5f, 0.5f);


    public FlowerPrefab srPrefab;
    public List<FlowerPrefab> srs = new List<FlowerPrefab>();
    public List<Sprite> flowerSprites;

    //[Button("Spawn")]
    public void Spawn() {

        foreach (FlowerPrefab srsss in srs) {
           if (srsss) DestroyImmediate(srsss.gameObject);
        }

        srs = new List<FlowerPrefab>();

        for (int x =0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                FlowerPrefab s =  Instantiate(srPrefab, transform);
                s.transform.localPosition = new Vector3(x * distanceInbetween.x, 0.1f, z * distanceInbetween.z);
                s.transform.localPosition += new Vector3(Random.Range(variation.x, variation.y), 0, Random.Range(variation.x, variation.y));
                srs.Add(s);
            }
        }
    }

    //[Button("RandomizeSprites")]
    public void RandomizeSprites() {

        List<Sprite> s = new List<Sprite>(flowerSprites);
        s = new List<Sprite> (s.Shuffle());

        int index = 0;

        for (int i = 0; i >= 0; i++) {
            if(index >= srs.Count) return;
            if (i >= s.Count) {
                i = 0;
                s = new List<Sprite>(s.Shuffle());
            }

            srs[index].SetSprite(s[i]);

            index++;
        }



    }


}
