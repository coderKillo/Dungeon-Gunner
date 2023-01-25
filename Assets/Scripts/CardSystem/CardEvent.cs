using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvent : MonoBehaviour
, IPointerClickHandler
{
    private int _id = 0;
    public int Id { get { return _id; } set { _id = value; } }

    public Action<CardEvent, CardEventArgs> OnEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        CallEvent(_id, CardEventType.Click);
    }

    private void CallEvent(int id, CardEventType cardEventType)
    {
        OnEvent?.Invoke(this, new CardEventArgs() { id = id, cardEventType = cardEventType });
    }
}

public class CardEventArgs : EventArgs
{
    public int id;
    public CardEventType cardEventType;
}

public enum CardEventType
{
    Click,
}
