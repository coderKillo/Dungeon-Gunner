using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(ParticleSystem))]
public class ElectricityEffect : MonoBehaviour
{
    [SerializeField] private float _travelTime = 0.1f;
    public float TravelTime { get { return _travelTime; } }

    [SerializeField] private float _bulletDuration = 1f;
    [SerializeField] private float _minDistance = 2f;

    private Vector3 _source;
    public Vector3 Source { set { _source = value; } }
    private Vector3 _target;
    public Vector3 Target { set { _target = value; } }

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Fire()
    {
        transform.position = _source;

        var targetWithMinDistance = CalculateTargetWithMinDistance();

        RotateToTarget();

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { gameObject.SetActive(true); });
        sequence.Append(transform.DOMove(targetWithMinDistance, _travelTime));
        sequence.AppendInterval(_bulletDuration);
        sequence.AppendCallback(() => { gameObject.SetActive(false); });
    }


    private void RotateToTarget()
    {
        var main = _particleSystem.main;
        var direction = _target - _source;
        var angle = HelperUtilities.GetAngleFromVector(direction);
        main.startRotation = -angle * Mathf.Deg2Rad;
    }

    // particle is only emitted after a specific distance
    // to ensure that the target is visual hit by the effect we set the corrected target behind the original target
    private Vector3 CalculateTargetWithMinDistance()
    {
        var direction = _target - _source;
        var distance = direction.magnitude;
        var correctedDistance = Mathf.Ceil(distance / _minDistance) * _minDistance;
        return (direction.normalized * correctedDistance) + transform.position;
    }
}
