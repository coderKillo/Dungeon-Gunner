using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    [SerializeField] private SpriteEffectSO _effect;

    public IEnumerator MaterializeRoutine(Shader shader, Color color, float time, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        SpawnEffect();

        var material = new Material(shader);

        material.SetColor("_EmissionColor", color);

        foreach (var renderer in spriteRendererArray)
        {
            renderer.material = material;
        }

        float dissolveAmount = 0;

        while (dissolveAmount < 1)
        {
            dissolveAmount += Time.deltaTime / time;

            material.SetFloat("_DissolveAmount", dissolveAmount);

            yield return null;
        }

        foreach (var renderer in spriteRendererArray)
        {
            renderer.material = normalMaterial;
        }
    }

    public IEnumerator MaterializeRoutineAllIn1Shader(Color color, float time, SpriteRenderer[] spriteRendererArray)
    {
        SpawnEffect();

        foreach (var renderer in spriteRendererArray)
        {
            renderer.material.EnableKeyword("FADE_ON");
            renderer.material.SetColor("_FadeBurnColor", color);
        }

        float dissolveAmount = 1;

        while (dissolveAmount >= -0.1f)
        {
            dissolveAmount -= Time.deltaTime / time;

            foreach (var renderer in spriteRendererArray)
            {
                renderer.material.SetFloat("_FadeAmount", dissolveAmount);
            }

            yield return null;
        }

        foreach (var renderer in spriteRendererArray)
        {
            renderer.material.DisableKeyword("FADE_ON");
        }
    }

    public void SpawnEffect()
    {
        if (_effect == null)
        {
            return;
        }

        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        effect.Initialize(_effect);
        effect.gameObject.SetActive(true);
    }
}
