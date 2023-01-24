using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    private int id = 0;
    public int Id { get { return id; } set { id = value; } }

    private bool selected = false;
    public bool Selected { get { return selected; } set { selected = value; } }

    public Action<int> OnClick;

    public void Click()
    {
        OnClick?.Invoke(id);
    }
}
