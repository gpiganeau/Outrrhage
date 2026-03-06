using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SelectionListener : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public event Action OnSelected;
    public event Action OnDeselected;

    public void OnSelect(BaseEventData eventData) {
        OnSelected?.Invoke();
        AudioManager.Instance.PlayUiSelect();

    }
    public void OnDeselect(BaseEventData eventData) => OnDeselected?.Invoke();
    public void OnPointerEnter(PointerEventData e) => OnSelected?.Invoke();
    public void OnPointerExit(PointerEventData e) => OnDeselected?.Invoke();
}