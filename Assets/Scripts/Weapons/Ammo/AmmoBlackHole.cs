using UnityEngine;

public class AmmoBlackHole : MonoBehaviour, IFireable
{
    private float _radius = 4f;
    private float _duration = 4f;
    private float _force = 100f;
    private float _timer = 0f;

    private void FixedUpdate()
    {
        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0)
        {
            gameObject.SetActive(false);
        }

        PullEnemies();
    }

    private void PullEnemies()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, _radius);

        foreach (var hit in colliders)
        {
            if (hit.tag == "Player")
            {
                continue;
            }

            var movementToPositionEvent = hit.GetComponent<MovementToPositionEvent>();

            if (movementToPositionEvent)
            {
                var currentPosition = hit.transform.position;
                var targetPosition = transform.position;
                var direction = (targetPosition - currentPosition).normalized;

                movementToPositionEvent.CallMovementToPositionEvent(currentPosition, targetPosition, _force, direction, false);
            }
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        _radius = ammoDetails.range;
        _force = ammoDetails.damage;

        transform.position = HelperUtilities.GetWorldMousePosition();
        transform.localScale = Vector3.one * (_radius / 5f);

        _timer = _duration;

        gameObject.SetActive(true);
    }

    public void SetOnHitEffect(GameObject onHitEffect, int damage, float radius)
    {
    }
}
