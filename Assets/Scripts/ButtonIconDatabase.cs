using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class StringSprite {

    public string name;
    [CanBeNull] public string spriteLookupName;
    public Sprite spr;
    public List<ControlVariable> action;

}

[System.Serializable]
public class ControlSchemeButtons {
    public string name;
    public List<StringSprite> stringSprites;
}


[CreateAssetMenu(menuName = "ButtonIconDataBase")]
public class ButtonIconDatabase :ScriptableObject
{

    public List<ControlSchemeButtons> buttons = new List<ControlSchemeButtons>();
   
}
