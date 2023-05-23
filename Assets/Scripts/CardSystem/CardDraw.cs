using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CardSystemSettings))]
[RequireComponent(typeof(CardSystemLevel))]
[RequireComponent(typeof(CardHand))]
public class CardDraw : MonoBehaviour
{
    [Header("SOUND EFFECTS")]
    [SerializeField] private SoundEffectSO _drawCardSound;
    [SerializeField] private float _drawSoundInterval;

    public enum State
    {
        Idle,
        Draw,
        EndDraw,
        HandFull,
    };

    private List<Card> _draw = new List<Card>();

    [HideInInspector] public Action<Card[]> OnCardChange;
    [HideInInspector] public Action<State> OnStateChange;

    private CardSystemSettings _cardSystemSettings;
    private CardSystemLevel _cardSystemLevel;
    private CardSystem _cardSystem;
    private CardHand _cardHand;
    private Queue<CardRarity> _drawQueue = new Queue<CardRarity>();

    private State _currentState;

    private void Awake()
    {
        _cardSystemSettings = GetComponent<CardSystemSettings>();
        _cardSystemLevel = GetComponent<CardSystemLevel>();
        _cardSystem = GetComponent<CardSystem>();
        _cardHand = GetComponent<CardHand>();
    }

    private void Start()
    {
        _cardHand.OnCardUpdate += CardHand_Update;
    }

    private void CardHand_Update(Card[] obj)
    {
        if (_currentState == State.HandFull && !_cardHand.Full())
        {
            UpdateState(State.Draw);
        }
    }

    public void Draw(CardRarity priority = CardRarity.Common)
    {
        if (IsBusy())
        {
            _drawQueue.Enqueue(priority);
            return;
        }

        _draw.Clear();
        var cards = _cardSystem.CardDeck();

        for (int i = 0; i < Settings.cardDrawSize; i++)
        {
            var card = new Card();
            card.id = Guid.NewGuid();
            card.level = _cardSystemLevel.GetRandomCardSpawnLevel();
            card.details = GetUniqueDetails(cards);

            _draw.Add(card);
        }

        if (!ContainsRarity(priority))
        {
            _draw[UnityEngine.Random.Range(0, Settings.cardDrawSize)].details = _cardSystemSettings.PickRandomCardWithSpecificRarity(cards, priority);
        }

        StartCoroutine(PlayDrawSound());

        OnCardChange?.Invoke(_draw.ToArray());
        UpdateState(State.Draw);


        if (_cardHand.Full())
        {
            UpdateState(State.HandFull);
        }
    }

    public bool IsBusy()
    {
        return _currentState != State.Idle;
    }

    private IEnumerator PlayDrawSound()
    {
        for (int i = 0; i < Settings.cardDrawSize; i++)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_drawCardSound);
            yield return HelperUtilities.WaitForRealSeconds(_drawSoundInterval);
        }
    }

    private CardSO GetUniqueDetails(CardSO[] cards)
    {
        var details = _cardSystemSettings.PickRandomCard(cards);

        int maxRetry = 10;
        int j = 0;
        while (ContainsDetails(details) && j < maxRetry)
        {
            details = _cardSystemSettings.PickRandomCardWithSpecificRarity(cards, details.rarity);
            j++;
        }

        return details;
    }

    private bool ContainsRarity(CardRarity rarity)
    {
        return (_draw.Find((x) => x.details.rarity == rarity)) != null;
    }

    private bool ContainsDetails(CardSO details)
    {
        return (_draw.Find((x) => x.details == details)) != null;
    }

    public void CardSelected(int id)
    {
        _cardHand.Add(_draw[id]);

        Done();
    }

    public void Done()
    {
        _draw.Clear();
        UpdateState(State.EndDraw);
    }

    public void EndDrawDone()
    {
        UpdateState(State.Idle);
        NextQueueItem();
    }

    private void UpdateState(State state)
    {
        _currentState = state;
        OnStateChange?.Invoke(_currentState);
    }

    private void NextQueueItem()
    {
        if (_drawQueue.Count > 0)
        {
            Debug.Log("draw next");
            Draw(_drawQueue.Dequeue());
        }
    }

}
