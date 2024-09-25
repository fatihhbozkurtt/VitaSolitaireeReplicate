using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class AcePointController : MonoBehaviour, ICardStacker
{
    [SerializeField] private List<CardController> cards;
    public CardSymbol symbol;

    public void OnCardsListChanged()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.SetActive(i == cards.Count - 1);
        }
    }

    #region ICardStacker

    public void AddCard(CardController newCard)
    {
        if (cards.Contains(newCard)) return;

        cards.Add(newCard);
        OnCardsListChanged();
    }

    public void RemoveCard(CardController card)
    {
        if (!cards.Contains(card)) return;

        cards.Remove(card);
        OnCardsListChanged();
    }

    public CardController GetLastCard()
    {
        return cards.Count > 0 ? cards[^1] : null;
    }

    public Vector3 GetPlacementPos()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    #endregion
}