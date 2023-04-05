using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCorpse : MonoBehaviour
{
    [SerializeField] private float _delay;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (_delay > 0f)
        {
            _delay -= Time.deltaTime;
        }
        else
        {
            _spriteRenderer.enabled = true;
        }
    }

}
