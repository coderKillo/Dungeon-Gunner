using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        if (obj.room != instantiatedRoom.room)
        {
            return;
        }

        if (instantiatedRoom.room.isLit)
        {
            return;
        }

        FadeInRoom();
        FadeInDoors();

        instantiatedRoom.room.isLit = true;
    }

    private void FadeInDoors()
    {
        foreach (var doorLightingControl in GetComponentsInChildren<DoorLightingControl>())
        {
            doorLightingControl.FadeIn();
        }
    }

    private void FadeInRoom()
    {
        StartCoroutine(FadeInRoomRoutine(instantiatedRoom));
    }

    private IEnumerator FadeInRoomRoutine(InstantiatedRoom instantiatedRoom)
    {
        var material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.collisionTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorator1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorator2Tilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fateInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.collisionTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decorator1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decorator2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }
}
