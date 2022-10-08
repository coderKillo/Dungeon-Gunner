using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoom = false;

    private BoxCollider2D doorTrigger;
    private Animator animator;

    private bool isOpen = false;
    private bool previouslyOpen = false;

    private void Awake()
    {
        doorCollider.enabled = false;

        doorTrigger = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.SetBool(Animations.open, isOpen); // restore last state
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == Settings.playerTag || other.gameObject.tag == Settings.playerWeaponTag)
        {
            Open();
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpen = true;

            doorTrigger.enabled = false;
            doorCollider.enabled = false;

            animator.SetBool(Animations.open, true);
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenSoundEffect);
        }
    }

    private void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            animator.SetBool(Animations.open, false);
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenSoundEffect);
        }
    }

    public void Lock()
    {
        doorTrigger.enabled = false;
        doorCollider.enabled = true;

        Close();
    }

    public void Unlock()
    {
        doorTrigger.enabled = true;
        doorCollider.enabled = false;

        if (previouslyOpen)
        {
            isOpen = false;
            Open();
        }
    }


    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}
