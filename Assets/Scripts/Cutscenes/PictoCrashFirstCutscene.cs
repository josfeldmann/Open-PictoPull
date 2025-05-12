using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictoCrashFirstCutscene : StoryCutScene {

  

    [TextArea]
    public string string1;

    [TextArea]
    public string string2;


    [TextArea]
    public string string3;

    [TextArea]
    public string string4;

    [TextArea]
    public string string5;


    public override IEnumerator CutScene() {


        List<string> s = new List<string>() { string1, string2 };

        Debug.LogError("Start");

        DisplaySpeechBox(s);
        yield return WaitTillSpeechIsDone();


    }





}

