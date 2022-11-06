using System.Xml.Serialization;
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    [Space(10)]
    [Header("SOUND")]
    [SerializeField] private SoundEffectSO moveSoundEffect;

    private BoxCollider2D boxCollider;
    public BoxCollider2D BoxCollider { get { return boxCollider; } }

    private Rigidbody2D rigidBody;
    private InstantiatedRoom instantiatedRoom;

    private Vector3 previousPosition;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        instantiatedRoom.moveableItemList.Add(this);
    }

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void OnCollisionStay2D(Collision2D other)
    {

        UpdateObstacles();
    }

    private void UpdateObstacles()
    {
        if (IsOutOfBounce())
        {
            transform.position = previousPosition;
            return;
        }

        instantiatedRoom.UpdateMoveableObjects();

        if (rigidBody.velocity.magnitude > 0.001f)
        {
            PlayMovingSound();
        }
    }

    private void PlayMovingSound()
    {
        if (moveSoundEffect == null)
        {
            return;
        }

        if (Time.frameCount % 10 != 0)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
    }

    private bool IsOutOfBounce()
    {
        var itemBounds = boxCollider.bounds;
        var roomBounds = instantiatedRoom.roomColliderBounds;

        if (roomBounds.Contains(itemBounds.min) && roomBounds.Contains(itemBounds.max))
        {
            return false;
        }

        return true;
    }
}
