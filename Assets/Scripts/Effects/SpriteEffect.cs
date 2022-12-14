using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEffect : MonoBehaviour
{
    [SerializeField] private bool looping = false;
    [SerializeField] private Sprite[] spriteArray;
    [SerializeField] private float frameRate;

    private SpriteRenderer spriteRenderer;
    private Coroutine showEffectCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        while (true)
        {
            yield return AnimateSpriteArray();

            if (!looping)
            {
                break;
            }
        }

        gameObject.SetActive(false);
    }

    private IEnumerator AnimateSpriteArray()
    {
        foreach (var sprite in spriteArray)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1 / frameRate);
        }
    }
}
