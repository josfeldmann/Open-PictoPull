using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubGoalBlock : MonoBehaviour
{

    public SpriteRenderer mainRenderer,subRenderer;

    public float rotSpeed = 180f;

    public Transform hide;
    Block block;
    BlockLevelGenerator generator;

    public bool showing = true;
    public float shrinkSpeed = 1f;

    public void SetColor(Block b, BlockLevelGenerator gen, Vector2Int vec) {

        mainRenderer.color = gen.coreCenterSprite.color;
        subRenderer.color = gen.coreBorderSprite.color;
        block = b;
        generator = gen;

        transform.localPosition = new Vector3(vec.x + 0.5f, vec.y + 1,0.5f);
        showing = true;

        
    
    }

    private void OnTriggerEnter(Collider other) {
        if (Layers.Player == other.gameObject.layer) {
            if (showing) {
                showing = false;
            }
        }
    }



    public void Update() {
        mainRenderer.transform.Rotate(0, 0, rotSpeed * Time.deltaTime);

        if (showing == true && hide.transform.localScale != Vector3.one) {
            hide.transform.localScale = Vector3.MoveTowards(hide.transform.localScale, Vector3.one, Time.deltaTime * shrinkSpeed);
        }

        if (showing == false && hide.transform.localScale != Vector3.zero) {
            hide.transform.localScale = Vector3.MoveTowards(hide.transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);
        }

    }

}
