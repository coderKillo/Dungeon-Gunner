using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class AmmoElectronicOrbPattern : Ammo
{
    [Header("References")]
    [SerializeField] private List<Transform> _orbs;
    [SerializeField] private GameObject _electricityBulletPrefab;

    [Header("Connections")]
    [SerializeField] private float _radius;
    [SerializeField] private float _lineFill;

    [Header("Spawn")]
    [SerializeField] private float _spawnInterval = 0.1f;
    [SerializeField] private float _triggerDelay = 1.0f;

    [Header("Raycast")]
    [SerializeField] private float _raycastRadius = 0.5f;
    [SerializeField] private LayerMask _mask;

    [Header("Generated Points")]
    [SerializeField] Vector2 _clusterOrigin;
    [SerializeField] Vector2 _clusterSize;
    [SerializeField] float _minDistance;

    private int MAX_LOOP_COUNT = 3000;

    [Title("Actions")]
    [Button()]
    public void GeneratePattern()
    {
        var points = GeneratePointCluster(_orbs.Count);
        for (int i = 0; i < _orbs.Count; i++)
        {
            _orbs[i].position = transform.position;

            if (i < points.Count)
            {
                _orbs[i].position += points[i];
                _orbs[i].RotateAround(transform.position, Vector3.forward, transform.eulerAngles.z);
            }
        }
    }

    private void Start()
    {
        DisableOrbs();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void Update()
    {
    }

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.isColliding = false;
        this.damage = damage;
        this.critChance = critChance;
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.onHitEffects.Clear();

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
        SetupTrail(ammoDetails);

        DisableOrbs();

        GeneratePattern();

        var orbs = ActiveOrbs();
        var lines = BuildLinesBetweenOrbs(orbs);
        StartCoroutine(SpawnOrbs(orbs, lines));
    }

    private List<Transform> ActiveOrbs()
    {
        return _orbs.FindAll(x => x.transform.position != transform.position);
    }

    private void DisableOrbs()
    {
        foreach (var orb in _orbs)
        {
            orb.gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnOrbs(List<Transform> orbs, List<Line> lines)
    {
        foreach (var orb in orbs)
        {
            orb.gameObject.SetActive(true);
            yield return new WaitForSeconds(_spawnInterval);
        }

        yield return new WaitForSeconds(_triggerDelay);

        foreach (var line in lines)
        {
            var hits = Physics2D.CircleCastAll(line.start, _raycastRadius, line.direction(), line.distance(), _mask);
            foreach (var hit in hits)
            {
                DealDamage(hit.collider);
            }

            var bullet = (ElectricityEffect)PoolManager.Instance.ReuseComponent(_electricityBulletPrefab, line.start, Quaternion.identity);
            bullet.Target = line.end;
            bullet.Source = line.start;
            bullet.gameObject.SetActive(true);
            bullet.Fire();

            PlayHitSound();

            // yield return new WaitForSeconds(bullet.TravelTime);
        }

        DisableOrbs();
    }

    private List<Line> BuildLinesBetweenOrbs(List<Transform> orbs)
    {
        var availableOrbs = new List<Transform>(orbs);
        var results = new List<Line>();

        foreach (var orb in orbs)
        {
            availableOrbs.Remove(orb);

            foreach (var otherOrb in availableOrbs)
            {
                if (Vector3.Distance(orb.position, otherOrb.position) > _radius)
                {
                    continue;
                }

                var line = CreateLine(orb.position, otherOrb.position);


                if (Line.IntersectAny(line, results))
                {
                    continue;
                }

                results.Add(line);
            }
        }

        return results;
    }

    private Line CreateLine(Vector3 start, Vector3 end)
    {
        var line = new Line(start, end);

        // offset start and end to prevent intersection on these points
        var direction = line.end - line.start;
        line.start += direction * _lineFill;
        line.end -= direction * _lineFill;

        return line;
    }

    private List<Vector3> GeneratePointCluster(int count)
    {
        var points = new List<Vector3>();

        for (int i = 0; i < MAX_LOOP_COUNT; i++)
        {
            var point = new Vector3();
            point.x = Random.Range(_clusterOrigin.x, _clusterOrigin.x + _clusterSize.x);
            point.y = Random.Range(_clusterOrigin.y, _clusterOrigin.y + _clusterSize.y);

            if (AnyDistanceSmaller(point, points))
            {
                continue;
            }

            points.Add(point);

            if (points.Count >= count)
            {
                return points;
            }
        }

        return points;
    }

    private bool AnyDistanceSmaller(Vector3 point, List<Vector3> points)
    {
        foreach (var p in points)
        {
            if (Vector3.Distance(point, p) < _minDistance)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube((Vector3)_clusterOrigin + (Vector3)_clusterSize / 2, (Vector3)_clusterSize);
        Gizmos.matrix = matrix;

        Gizmos.color = Color.white;
        foreach (var line in BuildLinesBetweenOrbs(ActiveOrbs()))
        {
            Gizmos.DrawLine(line.start, line.end);
        }
    }
}
