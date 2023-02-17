using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemTestController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            CardSystem.Instance.Draw();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!CardSystem.Instance.IsVisiable())
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
