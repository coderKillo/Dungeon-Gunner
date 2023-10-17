using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentLightningControl : MonoBehaviour
{
    [SerializeField] Material _litMaterial;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.material = GameResources.Instance.dimmedMaterial;
    }

    public void FadeIn()
    {
        var material = new Material(GameResources.Instance.variableLitShader);

        _spriteRenderer.material = material;
        material
            .DOFloat(1f, "Alpha_Slider", Settings.fateInTime)
            .OnComplete(() =>
            {
                _spriteRenderer.material = _litMaterial;
            });

    }
}
