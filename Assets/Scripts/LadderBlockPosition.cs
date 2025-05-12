using UnityEngine;

[System.Serializable]
public class LadderBlockPosition {
    public Vector3 portalPosition;
    public Vector3 portalRotation;

    public LadderBlockPosition(Vector3 portalPosition, Vector3 portalRotation) {
        this.portalPosition = portalPosition;
        this.portalRotation = portalRotation;
    }
}
