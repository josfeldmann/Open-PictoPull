using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSetter : MonoBehaviour
{

    public CharacterObject ob;
    public List<MeshRenderer> rend;

    void Start()
    {
        foreach (MeshRenderer r in rend) {
            r.material = ob.mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
