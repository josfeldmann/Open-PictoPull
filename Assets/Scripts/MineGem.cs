using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGem : MonoBehaviour
{

    public SpriteRenderer sprite;
    public float radius = 0.5f;

    public void SetSprite(Sprite s) {
        transform.position = (transform.up * Random.Range(-radius, radius)) + (transform.right * Random.Range(-radius, radius)) + transform.position ;
        sprite.sprite = s;
    }
}
