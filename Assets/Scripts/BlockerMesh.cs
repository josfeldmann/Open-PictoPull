using UnityEngine;

public class BlockerMesh : MonoBehaviour {

    public MeshRenderer mRenderer;
    public Collider col;
    public Blocker blocker;

    public void SetVisuals(bool b) {
        mRenderer.enabled = b;
    }

    public void Enable() {
        mRenderer.material = blocker.enabledMaterial;
        col.enabled = true;
    }

    public void Disable() {
        mRenderer.material = blocker.disabledMaterial;
        col.enabled = false;
    }


}
