using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            var position = HelperUtilities.GetWorldMousePosition();

            DamagePopup.Create(position, 999);
        }
    }
}
