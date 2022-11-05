using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light2D light2D;

    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 lightIntensity = new Vector2(0.3f, 0.6f);
    private float RandomLightIntensity { get { return Random.Range(lightIntensity.x, lightIntensity.y); } }

    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 lightFlickerTime = new Vector2(0.15f, 0.35f);
    private float RandomLightFlickerTime { get { return Random.Range(lightFlickerTime.x, lightFlickerTime.y); } }


    private float lightFlickerTimer = 0f;

    void Update()
    {
        lightFlickerTimer -= Time.deltaTime;
        if (lightFlickerTimer >= 0)
        {
            lightFlickerTimer = RandomLightFlickerTime;

            light2D.intensity = RandomLightIntensity;
        }
    }
}
