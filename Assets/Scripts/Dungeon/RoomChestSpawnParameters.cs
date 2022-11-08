using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class RoomChestSpawnParameters : MonoBehaviour
{
    public DungeonLevelSO dungeonLevel;

    [Range(0, 100)]
    [SerializeField] private int spawnChance = 0;
    public int SpawnChance { get { return spawnChance; } }

    [MinMaxSlider(1, 3)]
    [SerializeField] private Vector2Int itemAmount;
    public int RandomItemAmount { get { return Random.Range(itemAmount.x, itemAmount.y); } }

    [MinMaxSlider(0, 100)]
    [SerializeField] private Vector2Int healthAmount;
    public int RandomHealthAmount { get { return Random.Range(healthAmount.x, healthAmount.y); } }

    [MinMaxSlider(0, 100)]
    [SerializeField] private Vector2Int ammoAmount;
    public int RandomAmmoAmount { get { return Random.Range(ammoAmount.x, ammoAmount.y); } }

    [SerializeField] private WeaponDetailsSO[] weaponList;
    public WeaponDetailsSO RandomWeapon { get { return weaponList[Random.Range(0, weaponList.Length)]; } }
};
