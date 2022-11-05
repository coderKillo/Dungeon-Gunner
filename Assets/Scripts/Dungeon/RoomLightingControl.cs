using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

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
        FadeInEnvironment();

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
        var material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.collisionTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorator1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decorator2Tilemap.GetComponent<TilemapRenderer>().material = material;

        material
            .DOFloat(1f, "Alpha_Slider", Settings.fateInTime)
            .OnComplete(() =>
            {
                instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                instantiatedRoom.collisionTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                instantiatedRoom.decorator1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
                instantiatedRoom.decorator2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            });

    }

    private void FadeInEnvironment()
    {

        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (!renderer.CompareTag("Environment"))
            {
                continue;
            }

            var material = new Material(GameResources.Instance.variableLitShader);

            renderer.material = material;
            material
                .DOFloat(1f, "Alpha_Slider", Settings.fateInTime)
                .OnComplete(() =>
                {
                    renderer.material = GameResources.Instance.litMaterial;
                });
        }


    }
}
