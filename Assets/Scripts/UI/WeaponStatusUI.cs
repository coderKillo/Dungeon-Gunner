using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponStatusUI : MonoBehaviour
{
    #region OBJECT REFERENCES
    [Space(10)]
    [Header("WEAPON")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Image weaponImage;

    [Space(10)]
    [Header("AMMO")]
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private Transform ammoHolderTransform;

    [Space(10)]
    [Header("RELOAD")]
    [SerializeField] private Image reloadBarImage;
    #endregion

    private Player player;

    private List<GameObject> ammoIconList = new List<GameObject>();

    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.CurrentWeapon);
    }

    #region EVENT HANDLER
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2)
    {
        UpdateWeaponReloadBar(arg2.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent arg1, WeaponReloadedEventArgs arg2)
    {
        UpdateAmmoText(arg2.weapon);
        UpdateAmmoLoadedIcons(arg2.weapon);
        ResetWeaponReloadBar();
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent arg1, WeaponFiredEventArgs arg2)
    {
        if (arg2.weapon == player.activeWeapon.CurrentWeapon)
        {
            UpdateAmmoText(arg2.weapon);
            UpdateAmmoLoadedIcons(arg2.weapon);
        }
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2)
    {
        SetActiveWeapon(arg2.weapon);
    }
    #endregion

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateWeaponText(weapon);
        UpdateWeaponImage(weapon);

        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }
    }

    private void UpdateWeaponImage(Weapon weapon)
    {
        weaponImage.sprite = weapon.weaponDetails.weaponSprite;
    }

    private void UpdateWeaponText(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.totalAmmo.ToString() + " / " + weapon.weaponDetails.ammoCapacity.ToString();
        }
    }

    private void ResetWeaponReloadBar()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadBarImage.transform.localScale = new Vector3(1f, 1f, 1f);
        reloadBarImage.color = Color.green;
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.clipAmmo; i++)
        {
            var anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            var ammoIcon = GameObject.Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        foreach (var ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
        {
            return;
        }

        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarCoroutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarCoroutine(Weapon weapon)
    {
        while (weapon.isReloading)
        {
            var barFill = weapon.reloadTimer / weapon.weaponDetails.reloadTime;

            reloadBarImage.color = Color.red;
            reloadBarImage.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBarImage), reloadBarImage);
    }
#endif
    #endregion
}
