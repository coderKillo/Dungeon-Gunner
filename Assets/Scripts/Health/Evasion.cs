using System;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class Evasion : MonoBehaviour
{
    public bool evadeIncomingAttacks = false;

    [SerializeField] private float _evasionAngle = 30f;

    private AimWeaponEvent _aimWeaponEvent;
    private Player _player;
    private Enemy _enemy;

    private Vector3 _aimDirection = new Vector3();

    private void Awake()
    {
        _aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void Start()
    {
        _player = GetComponent<Player>();
        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        _aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        _aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent @event, AimWeaponEventArgs args)
    {
        _aimDirection = HelperUtilities.GetVectorFromAngle(args.aimAngle);
    }

    public bool EvadeAttack(Vector3 bulletDirection)
    {
        if (_player && _player.IsDashing())
        {
            return true;
        }

        if (evadeIncomingAttacks && IsFacingBullet(bulletDirection))
        {
            DamagePopup.Create(transform.position, "EVADE", Color.white, 4f);
            return true;
        }

        return false;
    }

    private bool IsFacingBullet(Vector3 direction)
    {
        return Vector3.Angle(direction, _aimDirection) > (180 - _evasionAngle);
    }
}