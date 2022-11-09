using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SpriteRenderer))]
public class Chest : MonoBehaviour, IUseable
{
    [Space(10)]
    [Header("BASE")]
    [SerializeField] private float openDelay = 1f;

    [Space(10)]
    [Header("MATERIALIZE")]
    [ColorUsage(true, true)][SerializeField] private Color materializeColor;
    [SerializeField] private float materializeTime = 3f;

    [Space(10)]
    [Header("CHEST ITEMS")]
    [SerializeField] private Transform spawnPoint;

    [Space(10)]
    [Header("SPRITES")]
    [SerializeField] private Sprite healthSprite;
    [SerializeField] private Sprite ammoSprite;

    [Space(10)]
    [Header("SOUND EFFECTS")]
    [SerializeField] private SoundEffectSO chestSpawnSoundEffect;
    [SerializeField] private SoundEffectSO chestOpenSoundEffect;
    [SerializeField] private SoundEffectSO pickUpHealthSoundEffect;
    [SerializeField] private SoundEffectSO pickUpAmmoSoundEffect;
    [SerializeField] private SoundEffectSO pickUpWeaponSoundEffect;


    private int healthAmount = 0;
    private int ammoAmount = 0;
    private WeaponDetailsSO weaponDetail = null;

    private Animator animator;
    private MaterializeEffect materializeEffect;
    private SpriteRenderer spriteRenderer;

    private ChestState chestState = ChestState.init;
    private bool isEnabled = false;

    private GameObject chestItemGameObject;
    private ChestItem chestItem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void Initialize(int healthAmount, int ammoAmount, WeaponDetailsSO weaponDetail)
    {
        this.healthAmount = healthAmount;
        this.ammoAmount = ammoAmount;
        this.weaponDetail = weaponDetail;

        if (weaponDetail != null && GameManager.Instance.Player.HasWeapon(weaponDetail))
        {
            this.weaponDetail = null;
            this.ammoAmount = Math.Clamp(ammoAmount * 2, 0, 100);
            this.healthAmount = Math.Clamp(healthAmount * 2, 0, 100);
        }

        SoundEffectManager.Instance.PlaySoundEffect(chestSpawnSoundEffect);

        StartCoroutine(MaterializeChestRoutine());
    }

    private IEnumerator MaterializeChestRoutine()
    {
        yield return materializeEffect.MaterializeRoutine(
            GameResources.Instance.materializeShader,
            materializeColor,
            materializeTime,
            new SpriteRenderer[] { spriteRenderer },
            GameResources.Instance.litMaterial
        );

        UpdateChestState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != Settings.playerTag)
        {
            return;
        }

        Open();
    }

    public void useItem()
    {
        switch (chestState)
        {
            case ChestState.init:
                break;

            case ChestState.closed:
                break;

            case ChestState.collectAmmo:
            case ChestState.collectHealth:
            case ChestState.collectWeapon:
                Collect();
                break;

            case ChestState.empty:
                break;

            default:
                break;
        }
    }

    private void UpdateChestState()
    {
        if (chestState == ChestState.init)
        {
            chestState = ChestState.closed;
        }
        else if (healthAmount > 0)
        {
            InstantiateItem(healthSprite, healthAmount.ToString() + "%");
            chestState = ChestState.collectHealth;
        }
        else if (ammoAmount > 0)
        {
            InstantiateItem(ammoSprite, ammoAmount.ToString() + "%");
            chestState = ChestState.collectAmmo;
        }

        else if (weaponDetail != null)
        {
            InstantiateItem(weaponDetail.weaponSprite, weaponDetail.weaponName);
            chestState = ChestState.collectWeapon;
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    private void Collect()
    {
        if (chestItemGameObject == null || !chestItem.IsMaterialized)
        {
            return;
        }

        var player = GameManager.Instance.Player;

        switch (chestState)
        {
            case ChestState.collectHealth:

                SoundEffectManager.Instance.PlaySoundEffect(pickUpHealthSoundEffect);

                player.health.Heal((int)((float)player.health.StartingHealth * ((float)healthAmount / 100f)));

                healthAmount = 0;

                break;


            case ChestState.collectAmmo:

                SoundEffectManager.Instance.PlaySoundEffect(pickUpAmmoSoundEffect);

                player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.CurrentWeapon, ammoAmount);

                ammoAmount = 0;

                break;


            case ChestState.collectWeapon:

                SoundEffectManager.Instance.PlaySoundEffect(pickUpWeaponSoundEffect);

                if (!player.HasWeapon(weaponDetail))
                {
                    player.AddWeaponToPlayer(weaponDetail);
                }

                weaponDetail = null;

                break;

            default:
                break;
        }

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private void InstantiateItem(Sprite sprite, string text)
    {
        chestItemGameObject = GameObject.Instantiate(GameResources.Instance.chestItemPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        chestItem = chestItemGameObject.GetComponent<ChestItem>();

        chestItem.Initialize(sprite, text, materializeColor);
    }

    private void Open()
    {
        if (chestState != ChestState.closed)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(chestOpenSoundEffect);

        animator.SetBool(Animations.openChest, true);

        UpdateChestState();
    }
}
