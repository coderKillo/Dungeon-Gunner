
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPool_", menuName = "Scriptable Object/Enemy/Enemy Pool")]
public class EnemyPoolSO : ScriptableObject
{
    public List<EnemyDetailsSO> enemyList;
}