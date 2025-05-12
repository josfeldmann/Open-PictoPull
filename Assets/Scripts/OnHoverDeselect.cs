using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHoverDeselect : MonoBehaviour, IPointerEnterHandler {

    public UnityEvent HoverEvent;
    
    public void OnPointerEnter(PointerEventData eventData) {
        HoverEvent?.Invoke();
    }

    public void DeselectNavigation() {
        NavigationManager.DeselectObject();
    }


}
