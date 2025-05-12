using UnityEngine;
using UnityEngine.EventSystems;

public class CustomLevelSelectButton : MonoBehaviour, IPointerEnterHandler {
    public NavigationObject nav;
    public void OnPointerEnter(PointerEventData eventData) {
        Hover();
    }

    public void Hover() {
      //  Debug.Log("Hover Called " + Time.time);
        GameMasterManager.instance.customLevelSelect.HoverNavigationObject(nav);
    }




}
