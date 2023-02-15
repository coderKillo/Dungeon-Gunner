using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReloadBar : MonoBehaviour
{
    #region OBJECT REFERENCES
    [SerializeField] private RectTransform _bar;
    [SerializeField] private RectTransform _slider;
    [SerializeField] private float _barWith;
    #endregion

    private Player _player;

    private Coroutine _reloadWeaponCoroutine;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Start()
    {
        SetActiveWeapon(_player.activeWeapon.CurrentWeapon);
    }

    private void OnEnable()
    {
        _player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        _player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
        _player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
    }

    private void OnDisable()
    {
        _player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        _player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
        _player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
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

        _bar.gameObject.SetActive(false);
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

            _bar.gameObject.SetActive(true);
            _slider.localPosition = new Vector3(sliderPosition, 0f, 0f);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
