using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private PlayerDetailsSO _playerDetails;

    void Start()
    {
        var playerGameObject = Instantiate(_playerDetails.prefab);
        var player = playerGameObject.GetComponent<Player>();

        player.Initialize(_playerDetails);
    }

}
