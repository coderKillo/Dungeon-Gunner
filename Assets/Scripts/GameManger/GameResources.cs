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
    [Header("PLAYER SELECTION")]
    public GameObject playerSelectionPrefab;

    [Space(10)]
    [Header("PLAYER")]
    public CurrentPlayerSO currentPlayer;
    public List<PlayerDetailsSO> playerDetailsList;

    [Space(10)]
    [Header("MATERIALS")]
    public Material dimmedMaterial;
    public Material litMaterial;

    [Space(10)]
    [Header("SHADERS")]
    public Shader variableLitShader;
    public Shader materializeShader;

    [Space(10)]
    [Header("UI")]
    public GameObject ammoIconPrefab;
    public GameObject damagePopupPrefab;

    [Space(10)]
    [Header("CHESTS")]
    public GameObject chestPrefab;
    public GameObject chestItemPrefab;

    [Space(10)]
    [Header("PORTAL")]
    public GameObject portalPrefab;

    [Space(10)]
    [Header("SOUND EFFECTS")]
    public AudioMixerGroup soundMasterAudioMixer;
    public SoundEffectSO doorOpenSoundEffect;
    public SoundEffectSO tableFlipSoundEffect;

    [Space(10)]
    [Header("MUSIC")]
    public AudioMixerGroup musicMasterAudioMixer;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;
    public MusicTrackSO mainMenuTrack;

    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPath;
}
