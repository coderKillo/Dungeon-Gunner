using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform characterSelector;
    [SerializeField] private float moveToSelectedCharacterTime = 1f;

    private float offset = 4f;
    private int selectedIndex = 0;

    private void Start()
    {
        var position = Vector3.zero;
        foreach (var playerDetails in GameResources.Instance.playerDetailsList)
        {
            var playerObject = GameObject.Instantiate(GameResources.Instance.playerSelectionPrefab, characterSelector);

            playerObject.transform.localPosition = position;
            position += Vector3.right * offset;

            var playerSelection = playerObject.GetComponent<PlayerSelectionUI>();

            playerSelection.playerHandSpriteRenderer.sprite = playerDetails.handSprite;
            playerSelection.playerHandNoWeaponSpriteRenderer.sprite = playerDetails.handSprite;
            playerSelection.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
        }

        UpdateCurrentPlayer();
    }

    public void SelectNext()
    {
        if ((selectedIndex + 1) >= GameResources.Instance.playerDetailsList.Count)
        {
            return;
        }

        selectedIndex++;

        UpdateCurrentPlayer();
    }

    public void SelectPrevious()
    {
        if ((selectedIndex - 1) < 0)
        {
            return;
        }

        selectedIndex--;

        UpdateCurrentPlayer();
    }

    private void UpdateCurrentPlayer()
    {
        GameResources.Instance.currentPlayer.playerDetails = GameResources.Instance.playerDetailsList[selectedIndex];

        MoveToSelectedCharacter();
    }

    private void MoveToSelectedCharacter()
    {
        characterSelector.DOLocalMoveX(selectedIndex * -offset, moveToSelectedCharacterTime);
    }
}
