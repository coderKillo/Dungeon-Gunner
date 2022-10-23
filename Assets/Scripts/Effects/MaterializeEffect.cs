using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader shader, Color color, float time, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
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
}
