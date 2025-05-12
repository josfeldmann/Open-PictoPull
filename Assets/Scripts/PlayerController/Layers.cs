using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layers
{

    public const int Player = 8, Block = 6, Ground = 7;
    public const string blockerTag = "Blocker";
    public static bool inLayer(int layer, LayerMask layermask) {
        return layermask == (layermask | (1 << layer));
    }
}
