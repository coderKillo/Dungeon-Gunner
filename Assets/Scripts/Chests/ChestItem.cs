using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    [SerializeField] private float materializeTime = 1f;

    private TextMeshPro textMesh;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;

    private bool isMaterialized = false;
    public bool IsMaterialized { get { return isMaterialized; } }

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void Initialize(Sprite sprite, string text, Color materializeColor)
    {
        spriteRenderer.sprite = sprite;
        textMesh.text = "";

        StartCoroutine(MaterializeItemRoutine(text, materializeColor));
    }

    private IEnumerator MaterializeItemRoutine(string text, Color materializeColor)
    {
        yield return materializeEffect.MaterializeRoutine(
            GameResources.Instance.materializeShader,
            materializeColor,
            materializeTime,
            new SpriteRenderer[] { spriteRenderer },
            GameResources.Instance.litMaterial
        );

        textMesh.text = text;

        isMaterialized = true;
    }
}