using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    private Dictionary<CardPowerUp, float> _powerUpTimer = new Dictionary<CardPowerUp, float>();
    private Dictionary<CardPowerUp, ICardPowerUp> _activePowerUps = new Dictionary<CardPowerUp, ICardPowerUp>();

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public void AddPowerUp(CardPowerUp type, ICardPowerUp powerUp)
    {
        if (_activePowerUps.ContainsKey(type))
        {
            _powerUpTimer[type] = powerUp.Duration();
            _player.buffEvent.CallRefreshBuff(type, powerUp.Duration());

            _activePowerUps[type].Deactivate(_player);
            _activePowerUps[type].Activate(_player);
        }
        else
        {
            _powerUpTimer.Add(type, powerUp.Duration());
            _activePowerUps.Add(type, powerUp);
            _player.buffEvent.CallAddBuff(powerUp.Details.icon, type, powerUp.Details.powerUpColor, powerUp.Duration());

            _activePowerUps[type].Activate(_player);
        }
    }

    private void Update()
    {
        var powerUpsToRemove = new List<CardPowerUp>();

        foreach (var type in _powerUpTimer.Keys.ToList())
        {
            _powerUpTimer[type] -= Time.deltaTime;
            if (_powerUpTimer[type] < 0f)
            {
                powerUpsToRemove.Add(type);
            }
        }

        foreach (var type in powerUpsToRemove)
        {
            _activePowerUps[type].Deactivate(_player);

            _activePowerUps.Remove(type);
            _powerUpTimer.Remove(type);
        }
    }
}
