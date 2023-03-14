using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReloadBar : MonoBehaviour
{
    #region OBJECT REFERENCES
    [SerializeField] private RectTransform _bar;
    [SerializeField] private RectTransform _barBackground;
    [SerializeField] private RectTransform _slider;
    [SerializeField] private RectTransform _sliderCharging;
    [SerializeField] private float _barWith;
    #endregion

    private Player _player;

    private Coroutine _reloadWeaponCoroutine;
    private Coroutine _chargeWeaponCoroutine;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Start()
    {
        _barBackground.gameObject.SetActive(false);
        _slider.gameObject.SetActive(false);
        _sliderCharging.gameObject.SetActive(false);

        SetActiveWeapon(_player.activeWeapon.CurrentWeapon);
    }

    private void OnEnable()
    {
        _player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        _player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
        _player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        _player.chargeWeaponEvent.OnChargeWeapon += ChargeWeaponEvent_OnChargeWeapon;
    }

    private void OnDisable()
    {
        _player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        _player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
        _player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        _player.chargeWeaponEvent.OnChargeWeapon -= ChargeWeaponEvent_OnChargeWeapon;
    }

    #region EVENT HANDLER
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2)
    {
        UpdateWeaponReloadBar(arg2.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent arg1, WeaponReloadedEventArgs arg2)
    {
        ResetWeaponReloadBar();
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2)
    {
        SetActiveWeapon(arg2.weapon);
    }

    private void ChargeWeaponEvent_OnChargeWeapon(ChargeWeaponEvent arg1, ChargeWeaponEventArgs arg2)
    {
        if (arg2.active)
        {
            _sliderCharging.gameObject.SetActive(true);
            _chargeWeaponCoroutine = StartCoroutine(ChargingBarCoroutine(arg2.chargeTime));
        }
        else
        {
            _sliderCharging.gameObject.SetActive(false);
            if (_chargeWeaponCoroutine != null)
            {
                StopCoroutine(_chargeWeaponCoroutine);
            }
        }
    }

    #endregion

    private void SetActiveWeapon(Weapon weapon)
    {
        if (weapon.isReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }
    }

    private void ResetWeaponReloadBar()
    {
        if (_reloadWeaponCoroutine != null)
        {
            StopCoroutine(_reloadWeaponCoroutine);
        }

        _slider.gameObject.SetActive(false);
        _slider.localPosition = Vector3.zero;
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
        {
            return;
        }

        if (_reloadWeaponCoroutine != null)
        {
            StopCoroutine(_reloadWeaponCoroutine);
        }

        _reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarCoroutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarCoroutine(Weapon weapon)
    {
        while (weapon.isReloading)
        {
            var sliderPosition = (0.5f - (weapon.reloadTimer / weapon.weaponDetails.reloadTime)) * _bar.rect.width;

            _slider.gameObject.SetActive(true);
            _slider.localPosition = new Vector3(sliderPosition, 0f, 0f);

            yield return null;
        }
    }

    private IEnumerator ChargingBarCoroutine(float chargeTime)
    {
        Vector3 start = new Vector3(_bar.rect.width * 0.5f, 0f, 0f);
        Vector3 end = new Vector3(-_bar.rect.width * 0.5f, 0f, 0f);
        float step = (1 / chargeTime) * Time.fixedDeltaTime;

        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            _sliderCharging.localPosition = Vector3.Lerp(start, end, t);
            yield return new WaitForFixedUpdate();
        }
    }

    private void Update()
    {
        if (_slider.gameObject.activeInHierarchy || _sliderCharging.gameObject.activeInHierarchy)
        {
            _barBackground.gameObject.SetActive(true);
        }
        else
        {
            _barBackground.gameObject.SetActive(false);
        }
    }
}
