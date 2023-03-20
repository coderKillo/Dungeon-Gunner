using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class CardPickUp : MonoBehaviour
{
    [Space(10)]
    [Header("JUMP")]
    [SerializeField] private float _jumpDuration = 3f;
    [SerializeField] private float _jumpForce = 2f;
    [SerializeField] private int _jumpNumber = 3;

    private Light2D _light;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _light = GetComponentInChildren<Light2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        _light.color = color;
        _spriteRenderer.color = color;
    }

    private void OnEnable()
    {
        transform.DOJump(transform.position, _jumpForce, _jumpNumber, _jumpDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != Settings.playerTag)
        {
            CardSystem.Instance.Draw();
            gameObject.SetActive(false);
        }
    }
}
