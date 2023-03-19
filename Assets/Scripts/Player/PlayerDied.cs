using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class PlayerDied : MonoBehaviour
{
    [SerializeField] private SpriteEffectSO deadEffect;

    private DestroyedEvent destroyedEvent;

    private void Awake()
    {
        destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable()
    {
        destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent obj, DestroyedEventArgs args)
    {
        var deadEffect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        deadEffect.Initialize(this.deadEffect);
        deadEffect.keepActiveAfterComplete = true;
        deadEffect.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }
}