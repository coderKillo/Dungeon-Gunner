using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void StartPowerUp(IEnumerator powerUpCoroutine, Color effectColor)
    {
        StartCoroutine(PowerUpCoroutine(powerUpCoroutine, effectColor));
    }

    public IEnumerator PowerUpCoroutine(IEnumerator powerUpCoroutine, Color effectColor)
    {
        _spriteRenderer.material.EnableKeyword("OUTBASE_ON");
        _spriteRenderer.material.EnableKeyword("GREYSCALE_ON");
        _spriteRenderer.material.SetColor("_Color", effectColor);

        yield return StartCoroutine(powerUpCoroutine);

        _spriteRenderer.material.DisableKeyword("OUTBASE_ON");
        _spriteRenderer.material.DisableKeyword("GREYSCALE_ON");
        _spriteRenderer.material.SetColor("_Color", Color.white);
    }
}
