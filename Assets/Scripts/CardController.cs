using System.Collections.Generic;
using Data;
using DG.Tweening;
using Managers;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("Debug")] public Lot parentLot;
    public bool isActive;
    [SerializeField] private CardData cardData;

    #region Starters
    public void Initialize(bool enable, Lot lot)
    {
        if (enable) Enable();
        else Disable();

        parentLot = lot;
    }

    public void Disable(bool includeRotating = true)
    {
        if (includeRotating) RotateSelf(180, .35f);
        isActive = false;
    }

    public void Enable()
    {
        RotateSelf(0, .35f);
        isActive = true;
    }

    #endregion
    
    private void OnMouseDown()
    {
        if (!isActive) return;
        var deckManager = DeckManager.instance;
        if (transform.parent.TryGetComponent(out Lot pLot))
        {
            if (pLot.GetLastCard() == this)
                deckManager.PerformPossibleSingleMove(clickedCard: this);
            else
            {
                #region Multiple Cards Movement

                // apply multiple movement logic here
                List<CardController> cards = new() { this };

                int myIndex = pLot.GetCardIndex(this);
                int lastCardIndex = pLot.GetCardIndex(pLot.GetLastCard());
                int forLoopIterate = // this value represents the amount of cards between
                    (lastCardIndex - myIndex) - 1; // the last and the clicked one

                for (int i = 0; i < forLoopIterate; i++)
                {
                    CardController card = pLot.GetCard(myIndex + i + 1);
                    cards.Add(card);
                }

                if (!cards.Contains(pLot.GetLastCard())) cards.Add(pLot.GetLastCard());

                bool triggerMultipleMove = true;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (i == cards.Count - 1) continue;

                    CardData current = cards[i].GetCardData();
                    CardData next = cards[i + 1].GetCardData();

                    if (!deckManager.GetSuitableSymbols(current.symbol).Contains(next.symbol)
                        || current.value - 1 != next.value)
                        triggerMultipleMove = false;
                }

                if (!triggerMultipleMove) return;

                deckManager.PerformMultipleCardsMove(cards: cards);
                foreach (var t in cards)
                {
                    Debug.Log("Card: " + t.gameObject.name);
                }

                #endregion
            }
        }
        else
        {
            deckManager.PerformPossibleSingleMove(clickedCard: this);
        }
    }

    #region MOVEMENT REGION

// ReSharper disable Unity.PerformanceAnalysis
    public void MoveForUndo(ICardStacker goBack)
    {
        RemoveSelfFromLists();

        transform.SetParent(goBack.GetTransform());
        parentLot = goBack.GetTransform().GetComponent<Lot>();

        Debug.LogWarning("Going back pos : " + goBack.GetPlacementPos());
        transform.DOLocalMove(goBack.GetPlacementPos()
            , 0.25f);

        goBack.AddCard(this);
        SetActivationStatus(true);
    }

    public void MoveNewLot(Lot newLot, List<CardController> cards = null)
    {
        RemoveSelfFromLists();

        transform.SetParent(newLot.transform);
        parentLot = newLot;

        transform.DOLocalMove(newLot.GetPlacementPos()
            , 0.25f).OnComplete((() =>
        {
            if (cards == null) return;
            foreach (var card in cards)
            {
                card.SetParentLot(newLot);
            }
        }));

        newLot.AddCard(this);
    }

    public void MoveAcePoint(AcePointController acePoint)
    {
        RemoveSelfFromLists();

        parentLot = null;

        acePoint.AddCard(this);
        transform.SetParent(acePoint.transform);

        transform.DOLocalMove(Vector3.zero,
            0.25f).OnComplete(acePoint.OnCardsListChanged);
    }

    #endregion
    public void RemoveSelfFromLists()
    {
        if (transform.parent.TryGetComponent(out AcePointController acePointController))
            acePointController.RemoveCard(this);
        else if (transform.parent.TryGetComponent(out ExtraCardsContainer ecc))
            ecc.RemoveCard(this);
        else
            parentLot.RemoveCard(this);
    }
    private void SetParentLot(Lot newLot)
    {
        newLot.AddCard(this);
        parentLot = newLot;
        transform.SetParent(newLot.transform);
    }
    public CardData GetCardData()
    {
        return cardData;
    }
    public void AssignCardData(CardData data)
    {
        cardData = data;
        name = cardData.symbol + "_" + cardData.value;
        SetMaterial();
    }
    private void SetMaterial()
    {
        CardMaterialSo cmso = DeckManager.instance.cardMaterialData;

        foreach (var materialData in cmso.materialData)
        {
            if (materialData.symbol != cardData.symbol) continue;
            Material newMat = materialData.materials[cardData.value - 1];

            GetComponentInChildren<MeshRenderer>().material = newMat;
        }
    }
    public void SetActivationStatus(bool status)
    {
        isActive = status;
    }
    private void RotateSelf(float degree, float duration)
    {
        Vector3 targetRotation = new Vector3(0, degree, 0);

        transform.DORotate(targetRotation, duration)
            .SetEase(Ease.Linear);
    }
}