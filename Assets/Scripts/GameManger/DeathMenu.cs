using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private GameObject _deathMenu;

    public void DisplayMenu()
    {
        _deathMenu.gameObject.SetActive(true);

        GameManager.Instance.Player.EnablePlayer(false);
    }

    public void ClearMenu()
    {
        _deathMenu.gameObject.SetActive(false);

        GameManager.Instance.Player.EnablePlayer(true);
    }
}
