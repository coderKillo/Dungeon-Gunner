using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private Slider _slider;
    public Slider Slider { get { return _slider; } }

    private void Start()
    {
        _text.text = Round(_slider.value).ToString();
    }

    public void SliderUpdate(float value)
    {
        _text.text = Round(value).ToString();
    }

    private float Round(float value)
    {
        return Mathf.Round(value * 10.0f) * 0.1f;
    }
}
