using System;
using System.Collections;
using UnityEngine;

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
            StartCoroutine(FadeInRoutine(spriteRenderer));
        }

        isLit = true;
    }

    private IEnumerator FadeInRoutine(SpriteRenderer spriteRenderer)
    {
        var material = new Material(GameResources.Instance.variableLitShader);

        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fateInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FadeIn();
    }
}
