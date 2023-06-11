using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoExplosionPattern : Ammo
{
    public enum Pattern
    {
        Line,
        Spiral,
        Arc
    }

    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _pointCount;
    [SerializeField] private GameObject _explosionPrefab;

    private float aimAngel;

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.isColliding = false;
        this.damage = damage;
        this.critChance = critChance;
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.aimAngel = aimAngel;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
        SetupTrail(ammoDetails);

        switch ((Pattern)UnityEngine.Random.Range(0, 3))
        {
            case Pattern.Line:
                StartCoroutine(SpawnExplosions(LinePoints()));
                break;

            case Pattern.Spiral:
                StartCoroutine(SpawnExplosions(SpiralPoints()));
                break;

            case Pattern.Arc:
                StartCoroutine(SpawnExplosions(ArcPoints()));
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void Update()
    {
    }

    private IEnumerator SpawnExplosions(List<Vector3> points)
    {
        foreach (var point in points)
        {
            var explosion = (Explosion)PoolManager.Instance.ReuseComponent(_explosionPrefab, point, Quaternion.identity);
            explosion.OnExplosionHit += OnExplosionHit;
            explosion.gameObject.SetActive(true);

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void OnExplosionHit(Collider2D[] colliders, Explosion explosion)
    {
        explosion.OnExplosionHit -= OnExplosionHit;

        foreach (var collider in colliders)
        {
            DealDamage(collider);
        }
    }

    private List<Vector3> LinePoints()
    {
        var points = new List<Vector3>();
        var arc = 20f;

        for (int i = 0; i < _pointCount; i += 2)
        {
            var percentToMax = (float)i / (float)_pointCount;
            points.Add(transform.position + (HelperUtilities.GetVectorFromAngle(aimAngel + (arc / 2)) * percentToMax * range));
            points.Add(transform.position + (HelperUtilities.GetVectorFromAngle(aimAngel - (arc / 2)) * percentToMax * range));
        }

        return points;
    }

    private List<Vector3> SpiralPoints()
    {
        var points = new List<Vector3>();

        for (int i = 0; i < _pointCount; i++)
        {
            var percentToMax = (float)i / (float)_pointCount;
            var pointOnLine = (fireDirectionVector.normalized * percentToMax * range);
            var x = pointOnLine.x + Mathf.Cos(i + aimAngel);
            var y = pointOnLine.y + Mathf.Sin(i + aimAngel);

            points.Add(transform.position + new Vector3(x, y, 0f));
        }

        return points;
    }

    private List<Vector3> ArcPoints()
    {
        var points = new List<Vector3>();
        var lineCount = 5;
        var arcAngle = 45f;

        for (int i = lineCount; i < _pointCount; i += lineCount)
        {
            var percentToMax = (float)i / (float)_pointCount;
            var angle = aimAngel - (arcAngle / 2f);

            for (int line = 0; line < lineCount; line++)
            {
                angle += (arcAngle / lineCount);
                var point = (HelperUtilities.GetVectorFromAngle(angle).normalized * percentToMax * range); ;

                points.Add(transform.position + point);
            }
        }

        return points;
    }
}

