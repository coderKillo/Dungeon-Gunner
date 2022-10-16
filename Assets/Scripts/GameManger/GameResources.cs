using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    [Space(10)]
    [Header("DUNGEON")]
    public RoomNodeTypeListSO roomNodeTypeList;

    [Space(10)]
    [Header("PLAYER")]
    public CurrentPlayerSO currentPlayer;

    [Space(10)]
    [Header("MATERIALS")]
    public Material dimmedMaterial;
    public Material litMaterial;

    [Space(10)]
    [Header("SHADERS")]
    public Shader variableLitShader;

    [Space(10)]
    [Header("UI")]
    public GameObject ammoIconPrefab;

    [Space(10)]
    [Header("SOUND")]
    public AudioMixerGroup soundMasterAudioMixer;
    public SoundEffectSO doorOpenSoundEffect;

    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPath;
}
