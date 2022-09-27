using System.Diagnostics.Contracts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PoolManager : SingletonAbstract<PoolManager>
{
    [System.Serializable]
    public struct Pool
    {
        public int size;
        public GameObject prefab;
        public string componentTypeName;
    }

    [SerializeField] private Pool[] poolArray;

    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    private void Start()
    {
        objectPoolTransform = this.gameObject.transform;

        foreach (var pool in poolArray)
        {
            CreatePool(pool.size, pool.prefab, pool.componentTypeName);
        }
    }

    private void CreatePool(int size, GameObject prefab, string componentTypeName)
    {
        var instanceId = prefab.GetInstanceID();
        var componentType = Type.GetType(componentTypeName);

        if (poolDictionary.ContainsKey(instanceId))
        {
            Debug.Log("pool dictionary already contained pool for prefab: " + prefab.name);
            return;
        }

        var parentGameObject = new GameObject(prefab.name + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        poolDictionary.Add(instanceId, new Queue<Component>());

        for (int i = 0; i < size; i++)
        {
            var gameObject = GameObject.Instantiate(prefab, parentGameObject.transform);
            gameObject.SetActive(false);

            var component = gameObject.GetComponent(componentType);

            poolDictionary[instanceId].Enqueue(component);
        }
    }

    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int instanceId = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(instanceId))
        {
            Debug.Log("No pool found for prefab: " + prefab.name);
            return null;
        }

        var component = poolDictionary[instanceId].Dequeue();
        poolDictionary[instanceId].Enqueue(component);

        if (component.gameObject.activeSelf)
        {
            component.gameObject.SetActive(false);
        }

        component.transform.position = position;
        component.transform.rotation = rotation;
        component.transform.localScale = prefab.transform.localScale;

        return component;
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}
