using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterObject")]
public class CharacterObject : ScriptableObject {
    [SerializeField]private string characterName;
    [SerializeField]private string characterKey;
    public Material mat;
    public Sprite characterIcon;
    public float characterWalkSpeed = 4;

    internal string GetName() {
        return characterName;
    }

    internal string GetKey() {
        return characterKey;
    }
}

