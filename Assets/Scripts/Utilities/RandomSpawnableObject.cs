using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        T spawnableObject = default(T);
        List<T> spawnableObjectLookup = new List<T>();
        List<int> spawnableObjectLookupIndexList = new List<int>();

        foreach (var spawnableObjectsByLevel in spawnableObjectsByLevelList)
        {
            if (spawnableObjectsByLevel.dungeonLevel != GameManager.Instance.CurrentLevel)
            {
                continue;
            }

            foreach (var spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
            {
                int index = spawnableObjectLookup.Count;
                int ratio = spawnableObjectRatio.ratio;

                spawnableObjectLookup.Add(spawnableObjectRatio.dungeonObject);

                for (int i = 0; i < spawnableObjectRatio.ratio; i++)
                {
                    spawnableObjectLookupIndexList.Add(index);
                }
            }
        }

        if (spawnableObjectLookupIndexList.Count <= 0)
        {
            return spawnableObject;
        }

        int lookUpValue = Random.Range(0, spawnableObjectLookupIndexList.Count);

        spawnableObject = spawnableObjectLookup[spawnableObjectLookupIndexList[lookUpValue]];

        return spawnableObject;
    }
}
