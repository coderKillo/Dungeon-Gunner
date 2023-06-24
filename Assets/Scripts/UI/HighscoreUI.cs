using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highscoreText;

    void Start()
    {
        _highscoreText.text = "Highscore: " + PlayerPrefs.GetInt(PrefKeys.highscore, 0);
    }
}
