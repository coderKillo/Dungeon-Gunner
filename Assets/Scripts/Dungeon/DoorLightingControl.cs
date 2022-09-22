using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private Door door;
    private bool isLit = false;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    public void FadeIn()
    {
        if (isLit)
        {
            return;
        }

        foreach (var spriteRenderer in GetComponentsInParent<SpriteRenderer>())
        {
            var material = new Material(GameResources.Instance.variableLitShader);

            spriteRenderer.material = material;
            material
                .DOFloat(1f, "Alpha_Slider", Settings.fateInTime)
                .OnComplete(() => { spriteRenderer.material = GameResources.Instance.litMaterial; });
        }

        isLit = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FadeIn();
    }
}
