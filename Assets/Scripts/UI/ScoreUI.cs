using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float scoreChangeInterval;

    private long currentScore = 0;
    private Coroutine scoreChangeRoutine;

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs args)
    {
        if (args.score <= 0)
        {
            currentScore = 0;
            UpdateScoreText();
            return;
        }

        if (scoreChangeRoutine != null)
        {
            StopCoroutine(scoreChangeRoutine);
        }

        scoreChangeRoutine = StartCoroutine(ScoreChangeRoutine(args.score));
    }

    private IEnumerator ScoreChangeRoutine(long targetScore)
    {
        while (currentScore > targetScore)
        {
            currentScore--;
            UpdateScoreText();
            yield return new WaitForSeconds(scoreChangeInterval);
        }

        while (currentScore < targetScore)
        {
            currentScore++;
            UpdateScoreText();
            yield return new WaitForSeconds(scoreChangeInterval);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }
}
