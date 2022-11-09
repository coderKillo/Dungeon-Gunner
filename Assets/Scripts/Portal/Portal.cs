using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Portal : MonoBehaviour, IUseable
{
    [SerializeField] private float spawnTime = 0.5f;

    private bool isEnabled = false;

    public static void SpawnPortal(Vector3 position)
    {
        var portalObject = GameObject.Instantiate(
            GameResources.Instance.portalPrefab,
            position,
            Quaternion.identity,
            GameManager.Instance.CurrentRoom.instantiatedRoom.transform);
    }

    private void Start()
    {
        transform.localScale = new Vector3(0f, 1f, 1f);
        transform
            .DOScaleX(1, spawnTime)
            .OnComplete(() => { isEnabled = true; });
    }

    public void useItem()
    {
        if (!isEnabled)
        {
            return;
        }

        GameManager.Instance.SetGameState(GameState.levelCompleted);
    }
}
