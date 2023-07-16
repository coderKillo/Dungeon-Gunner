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
        GameManager.Instance.Player.buffEvent.OnRefreshBuff += BuffEvent_OnRefreshBuff;
    }

    private void OnDisable()
    {
        GameManager.Instance.Player.buffEvent.OnAddBuff -= BuffEvent_OnAddBuff;
        GameManager.Instance.Player.buffEvent.OnRefreshBuff -= BuffEvent_OnRefreshBuff;
    }

    private void BuffEvent_OnAddBuff(BuffEvent @event, AddBuffEventArgs args)
    {
        var buffObject = GameObject.Instantiate(_buffPrefab, Vector3.zero, Quaternion.identity, _buffGroup);
        var buff = buffObject.GetComponent<BuffUI>();

        buff.Initialize(args.icon, args.type, args.color, args.duration);
        buff.gameObject.SetActive(true);
    }

    private void BuffEvent_OnRefreshBuff(BuffEvent @event, RefreshBuffEventArgs args)
    {
        foreach (var buff in _buffGroup.GetComponentsInChildren<BuffUI>())
        {
            if (buff.Type == args.type)
            {
                buff.StartDurationTimer(args.duration);
            }
        }
    }

}
