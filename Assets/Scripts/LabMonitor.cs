using System.Collections;
using System.Linq;
using UnityEngine;

public class LabMonitor : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int index;
    public LabMonitorManager manager;
    public Collider boxCOllider;

    public void SetMonitor(Sprite s, int i) {
        boxCOllider.enabled = true;
        StartCoroutine(ChangeSprite(s, i));
    }

    IEnumerator ChangeSprite(Sprite s, int i) {

        if (spriteRenderer.enabled != false) {

            while (spriteRenderer.transform.localScale != Vector3.zero) {
                spriteRenderer.transform.localScale = Vector3.MoveTowards(spriteRenderer.transform.localScale, Vector3.zero, Time.deltaTime * 5);
                yield return null;
            }


        }
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = s;
        index = i;

        while (spriteRenderer.transform.localScale != Vector3.one) {
            spriteRenderer.transform.localScale = Vector3.MoveTowards(spriteRenderer.transform.localScale, Vector3.one, Time.deltaTime * 5);
            yield return null;
        }

    }

    public void Click() {
        manager.Click(this);
    }

    public LayerMask playerLayer;

    public void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            GameMasterManager.instance.playerController.AddMonitor(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (Layers.inLayer(other.gameObject.layer, playerLayer)) {
            GameMasterManager.instance.playerController.RemoveMonitor(this);
        }
    }





}

