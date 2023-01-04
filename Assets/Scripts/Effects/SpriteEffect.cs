using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEffect : MonoBehaviour
{
    private Sprite[] spriteArray;
    private float frameRate;
    private SpriteRenderer spriteRenderer;
    private Coroutine showEffectCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteArray = new Sprite[0];
    }

    public void Initialize(SpriteEffectSO spriteEffect)
    {
        transform.localScale = new Vector3(spriteEffect.scale, spriteEffect.scale, spriteEffect.scale);
        transform.position += spriteEffect.offset;

        spriteArray = spriteEffect.spriteArray;
        frameRate = spriteEffect.frameRate;
    }

    private void OnEnable()
    {
        showEffectCoroutine = StartCoroutine(ShowEffect());
    }

    private void OnDisable()
    {
        if (showEffectCoroutine != null)
        {
            StopCoroutine(showEffectCoroutine);
        }
    }

    private IEnumerator ShowEffect()
    {
        foreach (var sprite in spriteArray)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1 / frameRate);
        }

        gameObject.SetActive(false);
    }
}
