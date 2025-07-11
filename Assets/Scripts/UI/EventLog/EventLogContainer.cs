using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventLogContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIEvents.UIOpen.OnMouseEnterUI.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIEvents.UIOpen.OnMouseExitUI.Invoke();
    }
}
