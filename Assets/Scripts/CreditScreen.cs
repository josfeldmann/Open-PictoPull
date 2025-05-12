using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScreen : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if (GameMasterManager.instance.inputManger.cancelButtonDown) {
            GameMasterManager.instance.GoToMainMenu();
        }
    }
}
