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

    private CardRarity _ensuredCardRarity;
    public CardRarity EnsuredCardRarity { set { _ensuredCardRarity = value; } }

    private SpriteRenderer _spriteRenderer;
    private ParticleSystem.MainModule _particleSystemMain;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _particleSystemMain = GetComponentInChildren<ParticleSystem>().main;
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
        _particleSystemMain.startColor = color;
    }

    private void OnEnable()
    {
        transform.DOJump(transform.position, _jumpForce, _jumpNumber, _jumpDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Settings.playerTag)
        {
            CardSystem.Instance.Draw(_ensuredCardRarity);
            gameObject.SetActive(false);
        }
    }
}
