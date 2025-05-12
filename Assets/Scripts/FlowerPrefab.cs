using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPrefab : MonoBehaviour
{
    public SpriteRenderer sr;
    public BoxCollider box;
    public LayerMask mask;

    public List<GameObject> games = new List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if (Layers.inLayer( other.gameObject.layer, mask)) {
          //  print("InLayer");
            
            if (!games.Contains(other.gameObject)) games.Add(other.gameObject);
            if (games.Count >= 0) sr.enabled = (false);
        }
     
    }

    public void Check() {
        if (games.Count >= 0) sr.enabled = (false);
        else sr.enabled = (true);
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, mask)) {
           // print("ExitLayer");
          
            if (games.Contains(other.gameObject)) games.Remove(other.gameObject);
            if (games.Count <= 0) sr.enabled = (true);
        }
        
    }

    public void SetSprite(Sprite s) {
        sr.sprite = s;
        box.size = new Vector3(sr.sprite.rect.width / sr.sprite.pixelsPerUnit, sr.sprite.rect.height / sr.sprite.pixelsPerUnit, 0.5f);
        box.center = new Vector3(0, box.size.y/2, 0);

    }

}
