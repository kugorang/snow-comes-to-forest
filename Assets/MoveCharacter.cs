using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCharacter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public bool buttonCheck;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonCheck = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonCheck = false;
    }
}