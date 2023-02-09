using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvent : MonoBehaviour
, IPointerClickHandler
, IPointerEnterHandler
, IPointerExitHandler
{
    private int _id = 0;
    public int Id { get { return _id; } set { _id = value; } }

    public Action<CardEvent, CardEventArgs> OnEvent;

    private void CallEvent(int id, CardEventType cardEventType)
    {
        OnEvent?.Invoke(this, new CardEventArgs() { id = id, cardEventType = cardEventType });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                CallEvent(_id, CardEventType.ClickLeft);
                break;

            case PointerEventData.InputButton.Right:
                CallEvent(_id, CardEventType.ClickRight);
                break;

            default:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CallEvent(_id, CardEventType.PointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CallEvent(_id, CardEventType.PointerExit);
    }
}

public class CardEventArgs : EventArgs
{
    public int id;
    public CardEventType cardEventType;
}

public enum CardEventType
{
    ClickLeft,
    ClickRight,
    PointerEnter,
    PointerExit,
}
