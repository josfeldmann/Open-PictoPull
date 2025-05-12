using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGemGenerator : MonoBehaviour
{
    public List<MineGem> gems = new List<MineGem>();
    public List<Sprite> gemSprites = new List<Sprite>();
    public Vector2 scale = new Vector2(0.75f, 1.25f);

    private void Awake() {
        foreach (MineGem g in gems) {
            g.SetSprite(gemSprites.PickRandom());
            float f = Random.Range(scale.x, scale.y);
            g.transform.localScale = new Vector3(f,f, 1);
        }
    }


}
