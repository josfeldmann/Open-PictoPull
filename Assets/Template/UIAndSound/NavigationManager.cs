using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NavigationManager : MonoBehaviour
{

    public static bool navigation = false;
    public static NavigationManager instance;
    public InputManager manager;
    public TextMeshProUGUI debugText;
    public NavigationObject currentNavigationObject;

    public static void InstanceCheck() {
        if (instance == null) {
            instance = FindObjectOfType<NavigationManager>();
        }
    }

    public static void SetSelectedObject(NavigationObject nav) {
        if (nav == null) return;
        InstanceCheck();
        instance.currentNavigationObject = nav;
        EventSystem.current.SetSelectedGameObject(nav.gameObject);
        navigation = true;
        UpdateText();
        NavigationCooldown();
        instance.currentNavigationObject.OnSelect.Invoke();
    }

    public static void StopNavigation() {
        InstanceCheck();
        navigation = false;
        UpdateText();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public static float NavigationTime = .30f, nextNavigationTime = 0;
    public static void NavigationCooldown() {
        nextNavigationTime = Time.time + NavigationTime;
    }

    public static void UpdateText() {
        if (navigation) {
            instance.debugText.SetText("Navigation: true \n" + instance.currentNavigationObject.gameObject.name);
        } else {
            instance.debugText.SetText("Navigation: false");
        }
    }

    Vector2 move;

    private void Update() {
        if (((GameMasterManager.IsCurrentlyGamePad() && manager.jumpDown) ||
             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)) && 
             !(GameMasterManager.instance.playerController.machine.currentState is PlayerIdleState ||
             (GameMasterManager.instance.playerController.machine.currentState is TimeRewindState || GameMasterManager.instance.playerController.machine.currentState is CrashRewindState))) {
           
            GameMasterManager.instance.UIClickSound();
        }
        if (navigation) {
            if (currentNavigationObject == null || !currentNavigationObject.gameObject.activeInHierarchy) {
                navigation = false;
                UpdateText();
            }

            /* if (manager.aDown) {
                 currentNavigationObject.ADown();
             } else if (manager.bDown) {
                 currentNavigationObject.BDown();
             } else*/

           

            move = new Vector2(manager.horizontal + manager.camHorizontal, manager.vertical + manager.camVertical);
                
            if (move.x != 0 || move.y != 0) {

             //   print("Here 1 " + Time.time);

                if (Time.time > nextNavigationTime) {
                   // print("Here 2 " + Time.time);
                    if (move.x != 0 && Mathf.Abs(move.x) > Mathf.Abs(move.y)) {
                        if (move.x > 0) {
                            NavigationObject nav = currentNavigationObject.GetRightObject();
                            if (nav != null && nav != currentNavigationObject) {
                                GameMasterManager.instance.UIMoveSound();
                                SetSelectedObject(nav);
                            }
                        } else {
                            NavigationObject nav = currentNavigationObject.GetLeftObject();
                            if (nav != null && nav != currentNavigationObject) {
                                GameMasterManager.instance.UIMoveSound();
                                SetSelectedObject(nav);
                            }
                        }
                    } else if (move.y != 0) {
                      //  print("Here 3 " + Time.time);
                        if (move.y < 0) {
                          //  print("Here 4 " + Time.time);
                            NavigationObject nav = currentNavigationObject.GetDownObject();
                            if (nav != null && nav != currentNavigationObject) {
                                GameMasterManager.instance.UIMoveSound();
                                SetSelectedObject(nav);
                            }
                        } else {
                         //   print("Here 5 " + Time.time);
                            NavigationObject nav = currentNavigationObject.GetUpObject();
                            if (nav != null && nav != currentNavigationObject) {
                                GameMasterManager.instance.UIMoveSound();
                                SetSelectedObject(nav);
                            }
                        }
                    }


                }
            }
        
        }
    }

    internal static void DeselectObject() {
        InstanceCheck();
        if (instance == null) return;
        instance.currentNavigationObject = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public static void SetupNavigationGrid(List<NavigationObject> navs, int column) {
        int colone = column - 1;
        for (int i = 0; i < navs.Count; i++) {
            NavigationObject n = navs[i];
            if (i % column == 0 && navs.Count >= i + column) {
                n.left = navs[i + column - 1];
            } else {
                if (i != 0) n.left = navs[i - 1];
            }

            if (i % column == colone) {
                n.right = navs[i - colone];
            } else if (i == navs.Count - 1) {
                n.right = navs[0];
            } else {
                n.right = navs[i + 1];
            }

            if (i + column < navs.Count) {
                n.down = navs[i + column];
            } else {
                n.down = null;
            }

            if (i > colone) {
                n.up = navs[i - column];
            } else {
                n.up = null;
            }



        }
        foreach (NavigationObject n in navs) {
            n.MakeButtonFix();
        }

    }

}
