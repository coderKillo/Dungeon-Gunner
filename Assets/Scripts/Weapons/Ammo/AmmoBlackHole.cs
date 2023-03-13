using System.Net.WebSockets;
using System.Net;
using System.Collections.Specialized;
using System;
using System.Collections;
using System.Collections.Generic;
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

            var pullDirection = transform.position - hit.transform.position;
            var targetRigidbody = hit.GetComponent<Rigidbody2D>();

            if (targetRigidbody)
            {
                var force = _force * (1 - pullDirection.magnitude / _radius);
                force = Mathf.Clamp(force, 5f, _force);
                targetRigidbody.AddForce(new Vector2(pullDirection.x, pullDirection.y).normalized * force, ForceMode2D.Impulse);
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

    public void SetOnHitEffect(GameObject onHitEffect, int damage)
    {
    }
}
