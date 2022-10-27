using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthEvent))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private Image fillBarImage;

    private HealthEvent healthEvent;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        SetHealthBarFill(arg2.healthPercent);
        SetHealthBarColor(arg2.healthPercent); ;
    }

    private void SetHealthBarColor(float healthPercent)
    {
        fillBarImage.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }

    private void SetHealthBarFill(float healthPercent)
    {
        fillBarImage.color = colorGradient.Evaluate(healthPercent);
    }
}
