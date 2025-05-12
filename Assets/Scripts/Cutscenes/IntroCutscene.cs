using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    public Transform boattransform;


    public void Cutscene() {

        StartCoroutine(IntroNum());


    }


    public IEnumerator IntroNum() {

        yield return null;



    }

}
