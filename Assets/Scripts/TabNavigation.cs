using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour
{
    public void Update() {

      /*  if (Input.GetKeyDown(KeyCode.Tab)) {
            if (EventSystem.current.currentSelectedGameObject == null) return;
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null) {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(EventSystem.current));  //if it's an input field, also set the text caret

                EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
            }
            //else Debug.Log("next nagivation element not found");

        }
      */
    }
}
