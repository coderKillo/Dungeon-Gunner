using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class DeathMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _scoreSourceText;
    [SerializeField] private TextMeshProUGUI _highscoreText;
    [SerializeField] private GameObject _newHighscoreBanner;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        CopyScoreText();
        SetHighscoreText();
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void CopyScoreText()
    {
        _scoreText.text = _scoreSourceText.text;
    }

    private void SetHighscoreText()
    {
        var highscore = PlayerPrefs.GetInt(PrefKeys.highscore, 0);
        var score = GetScoreFromText(_scoreText.text);

        Debug.Log($"score: {score}, highscore: {highscore}");

        if (score > highscore)
        {
            _newHighscoreBanner.SetActive(true);
            highscore = score;
            PlayerPrefs.SetInt(PrefKeys.highscore, highscore);
        }
        else
        {
            _newHighscoreBanner.SetActive(false);
        }

        _highscoreText.text = "Highscore: " + highscore.ToString();
    }

    private int GetScoreFromText(string text)
    {
        string score = Regex.Match(text, @"\d+").Value;
        Debug.Log(score);

        int scoreNumber;
        if (int.TryParse(score, out scoreNumber))
        {
            return scoreNumber;
        }
        return 0;
    }

}
