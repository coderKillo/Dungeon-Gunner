using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
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
    [Header("FEEDBACK")]
    [SerializeField] private MMF_Player fireFeedback;
    [SerializeField] private MMF_Player reloadFeedback;
    #endregion

    private Player player;

    private List<GameObject> ammoIconList = new List<GameObject>();

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.CurrentWeapon);
    }

    #region EVENT HANDLER

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent arg1, WeaponReloadedEventArgs arg2)
    {
        UpdateAmmoText(arg2.weapon);
        UpdateAmmoLoadedIcons(arg2.weapon);

        reloadFeedback.PlayFeedbacks();
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent arg1, WeaponFiredEventArgs arg2)
    {
        if (arg2.weapon != null && arg2.weapon == player.activeWeapon.CurrentWeapon)
        {
            UpdateAmmoText(arg2.weapon);
            DecreaseAmmoLoadedIcons(arg2.weapon);

            fireFeedback.PlayFeedbacks();
        }
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2)
    {
        if (arg2.weapon == null)
        {
            return;
        }

        SetActiveWeapon(arg2.weapon);
    }
    #endregion

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateWeaponText(weapon);
        UpdateWeaponImage(weapon);

        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
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

    private void FillAmmoLoadedIcons(Weapon weapon)
    {
        for (int i = 0; i < weapon.weaponDetails.ammoClipCapacity; i++)
        {
            var anchoredPosition = new Vector2(-Settings.uiAmmoIconSpacing * i, 0f);

            var ammoIcon = GameObject.Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

            if (i < weapon.clipAmmo)
            {
                ammoIcon.GetComponent<AmmoIconUI>().Fill();
            }
            else
            {
                ammoIcon.GetComponent<AmmoIconUI>().Empty();
            }

            ammoIconList.Add(ammoIcon);
        }
    }

    private void DecreaseAmmoLoadedIcons(Weapon weapon)
    {
        if (weapon.clipAmmo >= ammoIconList.Count)
        {
            return;
        }

        ammoIconList[weapon.clipAmmo].GetComponent<AmmoIconUI>().BulletFired();
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        foreach (var ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();

        FillAmmoLoadedIcons(weapon);
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
    }
#endif
    #endregion
}
