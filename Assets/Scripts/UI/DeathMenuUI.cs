using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DeathMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _scoreSourceText;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        CopyScoreText();
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
}
