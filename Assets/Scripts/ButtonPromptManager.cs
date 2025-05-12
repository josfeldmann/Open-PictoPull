using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum ControlVariable { JUMP = 0, CAMERA = 1, REWIND = 2, INTERACT = 3, PAUSE = 4, PULL = 5, SUBMIT = 6, CANCEL = 7, MOVE = 8, VERTICAL = 9, MIDDLEMOUSE = 10, HINT = 11 }


public class ButtonPromptManager : MonoBehaviour
{
    public static Dictionary<string, Dictionary<string, Sprite>> buttonSprite = new Dictionary<string, Dictionary<string, Sprite>>();
    public ButtonIconDatabase database;
    public PlayerInput m_PlayerInput;
    public static PlayerInput input;
    public static bool initYet = false;
    public static List<ButtonImagePrompt> prompts = new List<ButtonImagePrompt>();

    

    public void Initialize() {
        if (initYet) return;
        input = this.m_PlayerInput;

        buttonSprite = new Dictionary<string, Dictionary<string, Sprite>>();
        textIconSprite = new Dictionary<string, Dictionary< ControlVariable, string>>();
        foreach (ControlSchemeButtons c in database.buttons) {

            Dictionary<string, Sprite> s = new Dictionary<string, Sprite>();
            Dictionary<ControlVariable, string> textsprite = new Dictionary<ControlVariable, string>();
            foreach (StringSprite ss in c.stringSprites) {
                s.Add(ss.name, ss.spr);
                foreach (ControlVariable cc in ss.action)
                {
                    var spriteName = !string.IsNullOrEmpty(ss.spriteLookupName) ? ss.spriteLookupName : ss.name;
                    textsprite.Add(cc, spriteName);
                }
            }
            
            textIconSprite[c.name] = textsprite;
            buttonSprite[c.name] = s;
        }
        
        initYet = true;
    }

    internal static string GetSpriteTextIcon(ControlVariable jUMP)
    {
        var controlScheme = input.currentControlScheme;
        #if UNITY_SWITCH
        controlScheme = "Gamepad";
        #endif
        if (!textIconSprite.ContainsKey(controlScheme)) {
            Debug.LogError(controlScheme + " control scheme not in dictionary ");
            return "";
        }
        if (textIconSprite[controlScheme].ContainsKey(jUMP)) {
            return textIconSprite[controlScheme][jUMP];
        }
        Debug.LogError(jUMP.ToString() + " not in dictionary ");
        return "";
    }

    public static string ControlVariableToString(ControlVariable c) {


        switch (c) {
            case ControlVariable.CANCEL:
                return "[Cancel]";

            case ControlVariable.SUBMIT:
                return "[Submit]";

            case ControlVariable.PULL:
                return "[Pull]";

            case ControlVariable.JUMP:
                return "[Jump]";

            case ControlVariable.CAMERA:
                return "[Camera]";

            case ControlVariable.REWIND:
                return "[Rewind]";

            case ControlVariable.INTERACT:
                return "[Interact]";

            case ControlVariable.PAUSE:
                return "[Pause]";

            case ControlVariable.MOVE:
                return "[Move]";

            case ControlVariable.VERTICAL:
                return "[Vertical]";
            case ControlVariable.MIDDLEMOUSE:
                return "[MiddleMouse]";
            default:
                break;

        }

        return "sfkjaklfals";



    }


    public static void AddListener(ButtonImagePrompt b) {
        InitCheck();
        if (!prompts.Contains(b)) {
           // Debug.Log("Listener Added");
            prompts.Add(b);
        }
    }

    public void UpdatePrompts() {
        foreach (ButtonImagePrompt b in prompts) {
            b.InputUpdate();
        }
    }

    public static void InitCheck() {
        if (!initYet) FindObjectOfType<ButtonPromptManager>().Initialize();
    }

    public static ButtonInfo GetSprite(string s)
    {
        var controlScheme = input.currentControlScheme;

        if (s.Contains(" | Esc")) {
            s = s.Replace(" | Esc", "");
        }

        #if UNITY_SWITCH
        controlScheme = "Gamepad";
        #endif
        try {
            InitCheck();

            s = input.actions[s].GetBindingDisplayString(group: controlScheme);
            if (buttonSprite.TryGetValue(controlScheme, out var value))
            {
                return new ButtonInfo(ButtonType.CONTROLLER, s, value[s]);
            }

            return new ButtonInfo(ButtonType.KEYBOARDKEY, s, null);
        } catch (Exception e) {
            Debug.LogError(e);
            return null;
        }
    }

    public static Dictionary<string, Dictionary<ControlVariable, string>> textIconSprite = new();
}


public enum ButtonType { KEYBOARDKEY = 0, CONTROLLER = 1, UNBOUND = 2 }
