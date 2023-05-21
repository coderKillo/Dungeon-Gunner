using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _colorChangePeriod;

    private List<Color> _colors = new List<Color>();
    private int _currentColorIndex = 0;
    private float _timer = 0f;

    public void StartPowerUp(IEnumerator powerUpCoroutine, Color effectColor)
    {
        StartCoroutine(PowerUpCoroutine(powerUpCoroutine, effectColor));
    }

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _timer = _colorChangePeriod;

        if (_colors.Count > 0)
        {
            _currentColorIndex++;
            if (_currentColorIndex >= _colors.Count)
            {
                _currentColorIndex = 0;
            }
            Debug.AssertFormat(_colors.Count > _currentColorIndex, "Failed, list size ({0}) is smaller then index {1}", ((byte)_colors.Count), _currentColorIndex);
            SetColor(_colors[_currentColorIndex]);
        }
        else
        {
            ResetColor();
        }
    }

    private void ResetColor()
    {
        _spriteRenderer.material.SetColor("_Color", Color.white);
        _spriteRenderer.material.DisableKeyword("OUTBASE_ON");
        _spriteRenderer.material.DisableKeyword("GREYSCALE_ON");
    }

    private void SetColor(Color color)
    {
        _spriteRenderer.material.SetColor("_Color", color);
        _spriteRenderer.material.EnableKeyword("OUTBASE_ON");
        _spriteRenderer.material.EnableKeyword("GREYSCALE_ON");
    }

    public IEnumerator PowerUpCoroutine(IEnumerator powerUpCoroutine, Color effectColor)
    {
        _colors.Add(effectColor);

        yield return StartCoroutine(powerUpCoroutine);

        _colors.Remove(effectColor);
    }
}
