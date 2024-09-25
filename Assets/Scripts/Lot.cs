using System.Collections.Generic;
using Data;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class Lot : MonoBehaviour, ICardStacker
{
    [Header("References")] [SerializeField]
    private CardController cardPerfab;

    [Header("Debug")] [SerializeField] private List<CardController> cards;


    private void Start()
    {
        int lotIndex = DeckManager.instance.Lots.IndexOf(this);

        for (int i = 0; i < lotIndex + 1; i++)
        {
            CardController cloneCard = Instantiate(cardPerfab, Vector3.zero, Quaternion.identity, transform);
            var spawnPos = new Vector3(0, i * -0.25f, i * -0.05f);
            cloneCard.transform.localPosition = spawnPos;
            AddCard(cloneCard);

            cloneCard.Initialize(i == lotIndex || lotIndex == 0, this);
            DeckManager.instance.AddCards(cloneCard);
        }
    }


    #region ICardStacker

    public Vector3 GetPlacementPos()
    {
        return GetLastCard() != null
            ? GetLastCard().transform.localPosition + new Vector3(0, -0.25f, -0.05f)
            : Vector3.zero;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public CardController GetLastCard()
    {
        return cards.Count > 0 ? cards[^1] : null;
    }

    public void AddCard(CardController newCard)
    {
        if (cards.Contains(newCard)) return;
        cards.Add(newCard);
    }

    public void RemoveCard(CardController card)
    {
        if (!cards.Contains(card)) return;
        cards.Remove(card);

        OnCardRemoved();
    }

    #endregion

    void OnCardRemoved()
    {
        // activate the last element of the cards list if there is one

        if (GetLastCard() == null) return;

        GetLastCard().Enable();
    }


    public int GetCardIndex(CardController card)
    {
        return cards.Contains(card) ? cards.IndexOf(card) : -999;
    }

    public CardController GetCard(int index)
    {
        return cards[index];
    }
}