using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoIconUI : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void BulletFired()
    {
        _animator.Play(Animations.ammoIconFired);
    }

    public void Empty()
    {
        _bullet.SetActive(false);
    }

    public void Fill()
    {
        _bullet.SetActive(true);
        _animator.Play(Animations.ammoIconFilled);
    }
}
