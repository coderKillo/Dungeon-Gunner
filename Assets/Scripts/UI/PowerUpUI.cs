using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
    [SerializeField] private Transform _buffGroup;
    [SerializeField] private GameObject _buffPrefab;

    private void OnEnable()
    {
        GameManager.Instance.Player.buffEvent.OnAddBuff += BuffEvent_OnAddBuff;
    }

    private void OnDisable()
    {
        GameManager.Instance.Player.buffEvent.OnAddBuff -= BuffEvent_OnAddBuff;
    }

    private void BuffEvent_OnAddBuff(BuffEvent @event, BuffEventArgs args)
    {
        var buffObject = GameObject.Instantiate(_buffPrefab, Vector3.zero, Quaternion.identity, _buffGroup);
        var buff = buffObject.GetComponent<BuffUI>();

        buff.Initialize(args.icon, args.color, args.duration);
        buff.gameObject.SetActive(true);
    }
}
