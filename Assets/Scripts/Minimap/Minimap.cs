using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;

        var virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        virtualCamera.Follow = playerTransform;

        var spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.Instance.PlayerIcon;
    }

    void Update()
    {
        if (minimapPlayer != null && playerTransform != null)
        {
            minimapPlayer.transform.position = playerTransform.position;
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }
#endif
    #endregion
}
