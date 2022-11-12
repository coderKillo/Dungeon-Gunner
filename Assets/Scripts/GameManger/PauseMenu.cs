using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public void DisplayPauseMenu()
    {
        _pauseMenu.gameObject.SetActive(true);

        GameManager.Instance.Player.EnablePlayer(false);
    }

    public void ClearPauseMenu()
    {
        _pauseMenu.gameObject.SetActive(false);

        GameManager.Instance.Player.EnablePlayer(true);
    }
}
