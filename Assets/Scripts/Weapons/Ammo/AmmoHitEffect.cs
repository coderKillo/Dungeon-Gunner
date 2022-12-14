using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(ParticleSystem))]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem effectParticleSystem;

    private void Awake()
    {
        effectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void Setup(AmmoHitEffectSO effect)
    {
        SetupMainModule(effect);
        SetupColorOverLifeTimeModule(effect.colorGradient);
        SetupVelocityOverLifeTimeModule(effect.velocityOverLifetimeMin, effect.velocityOverLifetimeMax);
        SetupEmissionModule(effect.emissionRate, effect.burstParticleNumber);
        SetupTextureSheetModule(effect.sprite);
    }

    private void SetupMainModule(AmmoHitEffectSO effect)
    {
        var mainModule = effectParticleSystem.main;

        mainModule.duration = effect.duration;
        mainModule.startLifetime = effect.startLifetime;
        mainModule.startSize = effect.startParticleSize;
        mainModule.startSpeed = effect.startParticleSpeed;
        mainModule.maxParticles = effect.maxParticleNumber;
        mainModule.gravityModifier = effect.effectGravity;
    }

    private void SetupColorOverLifeTimeModule(Gradient colorGradient)
    {
        var colorOverLifeTimeModule = effectParticleSystem.colorOverLifetime;

        colorOverLifeTimeModule.color = colorGradient;
    }

    private void SetupVelocityOverLifeTimeModule(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        var velocityOverLifeTimeModule = effectParticleSystem.velocityOverLifetime;

        var minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifetimeMax.x;
        velocityOverLifeTimeModule.x = minMaxCurveX;

        var minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifetimeMax.y;
        velocityOverLifeTimeModule.y = minMaxCurveY;

        var minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifetimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifetimeMax.z;
        velocityOverLifeTimeModule.z = minMaxCurveZ;
    }

    private void SetupEmissionModule(int emissionRate, int burstParticleNumber)
    {
        var emissionModule = effectParticleSystem.emission;

        emissionModule.rateOverTime = emissionRate;

        emissionModule.SetBurst(0, new ParticleSystem.Burst(0, burstParticleNumber));
    }

    private void SetupTextureSheetModule(Sprite sprite)
    {
        var textureSheetModule = effectParticleSystem.textureSheetAnimation;

        textureSheetModule.SetSprite(0, sprite);
    }
}
