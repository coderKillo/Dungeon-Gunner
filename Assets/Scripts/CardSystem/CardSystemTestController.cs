using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemTestController : MonoBehaviour
{
    private bool toggleShow = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            CardSystem.Instance.Draw();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleShow = !toggleShow;
            if (toggleShow)
            {
                CardSystem.Instance.Show();
            }
            else
            {
                CardSystem.Instance.Hide();
            }
        }
    }

}
