using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class Tutorial : SerializedMonoBehaviour
{
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private Dictionary<Stage, String> _dialogTexts;
    [SerializeField] private Vector3 _dialogOffsetToPlayer;
    [SerializeField] private bool _skipTutorial = false;

    public enum Stage
    {
        Movement,
        Dash,
        Shot,
        Reload,
    }

    public Action OnTutorialDone;

    private Stage _currentStage;
    private GameObject _dialogObject;
    private Player _player;

    private void Update()
    {
        if (_dialogObject != null)
        {
            _dialogObject.transform.position = GameManager.Instance.PlayerPosition + _dialogOffsetToPlayer;
        }
    }

    private void OnEnable()
    {
        _player = GameManager.Instance.Player;

        _player.movementByVelocityEvent.OnMovementByVelocity += Player_OnMovementByVelocity;
        _player.movementToPositionEvent.OnMovementToPosition += Player_OnMovementByPosition;
        _player.reloadWeaponEvent.OnReloadWeapon += Player_OnReloadWeapon;
        _player.fireWeaponEvent.OnFireWeapon += Player_OnFireWeapon;

        StartTutorial();
    }

    private void OnDisable()
    {
        _player.movementByVelocityEvent.OnMovementByVelocity -= Player_OnMovementByVelocity;
        _player.movementToPositionEvent.OnMovementToPosition -= Player_OnMovementByPosition;
        _player.reloadWeaponEvent.OnReloadWeapon -= Player_OnReloadWeapon;
        _player.fireWeaponEvent.OnFireWeapon -= Player_OnFireWeapon;
    }

    private void StageDone(Stage stage)
    {
        if (stage != _currentStage)
        {
            return;
        }

        _dialogObject.SetActive(false);

        switch (_currentStage)
        {
            case Stage.Movement:
                NextDialog(Stage.Dash);
                break;

            case Stage.Dash:
                NextDialog(Stage.Shot);
                break;

            case Stage.Shot:
                NextDialog(Stage.Reload);
                break;

            case Stage.Reload:
                EndTutorial();
                break;

            default:
                break;
        }
    }

    private void NextDialog(Stage stage)
    {
        _currentStage = stage;
        _dialogObject.GetComponentInChildren<TextMeshPro>().text = _dialogTexts[stage];
        _dialogObject.SetActive(true);
    }

    private void StartTutorial()
    {
        _dialogObject = GameObject.Instantiate(_dialogPrefab, transform.position, Quaternion.identity);

        if (!_skipTutorial)
        {
            NextDialog(Stage.Movement);
        }
        else
        {
            EndTutorial();
        }
    }

    private void EndTutorial()
    {
        OnTutorialDone?.Invoke();
        Destroy(_dialogObject);
        this.enabled = false;
    }

    #region EVENT HANDLER
    private void Player_OnFireWeapon(FireWeaponEvent @event, FireWeaponEventArgs args)
    {
        StageDone(Stage.Shot);
    }

    private void Player_OnReloadWeapon(ReloadWeaponEvent @event, ReloadWeaponEventArgs args)
    {
        StageDone(Stage.Reload);
    }

    private void Player_OnMovementByVelocity(MovementByVelocityEvent @event, MovementByVelocityArgs args)
    {
        StageDone(Stage.Movement);
    }

    private void Player_OnMovementByPosition(MovementToPositionEvent @event, MovementToPositionArgs args)
    {
        StageDone(Stage.Dash);
    }

    #endregion
}
