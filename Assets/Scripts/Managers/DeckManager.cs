using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class DeckManager : MonoSingleton<DeckManager>
    {
        public event Action<MovePathData> NewMovementPerformedEvent;

        [Header("References")] public CardMaterialSo cardMaterialData;
        public List<Lot> Lots;
        [Header("Debug")] public List<CardController> spawnedCards;

        public void AddCards(CardController newCard)
        {
            spawnedCards.Add(newCard);
            if (spawnedCards.Count == 28)
            {
                // assign cards data
                AssignShuffledCardData();
            }
        }

        public void PerformPossibleSingleMove(CardController clickedCard)
        {
            MovePathData data = new MovePathData();
            CardData clickedData = clickedCard.GetCardData();

            #region Special Cards Movement Logic

            switch (clickedData.value)
            {
                case 1:
                {
                    #region Ace Points Logic

                    // this means the clicked card is an ACE
                    AcePointController acePoint =
                        AcePointsManager.instance.GetAvailablePointBySymbol(clickedData.symbol);
                    data.from = clickedCard.parentLot != null
                        ? clickedCard.parentLot
                        : ExtraCardsContainer.instance;
                    data.to = acePoint;
                    data.card = clickedCard;
                    NewMovementPerformedEvent?.Invoke(data);

                    clickedCard.MoveAcePoint(acePoint);
                    clickedCard.Disable(false);

                    return;

                    #endregion
                }
                case 13:
                {
                    #region Kings Logic

                    // this means the clicked card is a KING
                    foreach (var lot in Lots)
                    {
                        if (lot.GetLastCard() != null) continue;

                        data.from = clickedCard.parentLot != null
                            ? clickedCard.parentLot
                            : ExtraCardsContainer.instance;
                        data.to = lot;
                        data.card = clickedCard;
                        NewMovementPerformedEvent?.Invoke(data);

                        clickedCard.MoveNewLot(newLot: lot);
                        break;
                    }

                    return;

                    #endregion
                }
            }

            #endregion
            #region Default Cards Movement Logic

            #region Check for Ace points

            AcePointController sameSymbolAcePoint =
                AcePointsManager.instance.GetAvailablePointBySymbol(clickedData.symbol);
            if (sameSymbolAcePoint.GetLastCard() != null &&
                sameSymbolAcePoint.GetLastCard().GetCardData().value == clickedData.value - 1)
            {
                // same symbol ace is already clicked
                // and one less value already replaced too

                data.from =clickedCard.parentLot != null
                    ? clickedCard.parentLot
                    : ExtraCardsContainer.instance;
                data.to = sameSymbolAcePoint;
                data.card = clickedCard;
                NewMovementPerformedEvent?.Invoke(data);

                clickedCard.MoveAcePoint(sameSymbolAcePoint);
                return;
            }

            #endregion

            var matchedCards = new List<CardController>();
            foreach (var lot in Lots)
            {
                if (lot.GetLastCard() == null) continue;

                CardData lastCardData = lot.GetLastCard().GetCardData();
                if (GetSuitableSymbols(clickedData.symbol).Contains(lastCardData.symbol))
                {
                    if (lastCardData.value == clickedData.value + 1)
                        matchedCards.Add(lot.GetLastCard());
                }
            }


            if (matchedCards.Count != 0)
            {
                data.from = clickedCard.parentLot != null
                    ? clickedCard.parentLot
                    : ExtraCardsContainer.instance;
                data.to = matchedCards[0].parentLot;
                data.card = clickedCard;
                NewMovementPerformedEvent?.Invoke(data);

                clickedCard.MoveNewLot(newLot: matchedCards[0].parentLot);
            }

            #endregion
        }

        public void PerformMultipleCardsMove(List<CardController> cards)
        {
            CardController firstCard = cards[0];
            for (int i = 1; i < cards.Count; i++)
            {
                cards[i].RemoveSelfFromLists();
                cards[i].transform.SetParent(firstCard.transform);
            }

            #region Default Cards Movement Logic

            var matchedCards = new List<CardController>();
            foreach (var lot in Lots)
            {
                if (lot.GetLastCard() == null) continue;

                CardData lastCardData = lot.GetLastCard().GetCardData();
                if (GetSuitableSymbols(firstCard.GetCardData().symbol).Contains(lastCardData.symbol))
                {
                    if (lastCardData.value == firstCard.GetCardData().value + 1)
                        matchedCards.Add(lot.GetLastCard());
                }
            }


            if (matchedCards.Count == 0) return;

            firstCard.MoveNewLot(newLot: matchedCards[0].parentLot, cards);

            #endregion
        }

        public List<CardSymbol> GetSuitableSymbols(CardSymbol targetSymbol)
        {
            var availableSymbolsDict = new Dictionary<CardSymbol, List<CardSymbol>>
            {
                { CardSymbol.Hearts, new List<CardSymbol> { CardSymbol.Clubs, CardSymbol.Spades } },
                { CardSymbol.Clubs, new List<CardSymbol> { CardSymbol.Diamonds, CardSymbol.Hearts } },
                { CardSymbol.Diamonds, new List<CardSymbol> { CardSymbol.Clubs, CardSymbol.Spades } },
                { CardSymbol.Spades, new List<CardSymbol> { CardSymbol.Diamonds, CardSymbol.Hearts } }
            };

            if (availableSymbolsDict.TryGetValue(targetSymbol, out var availableSymbols))
            {
                return availableSymbols;
            }

            throw new ArgumentOutOfRangeException(nameof(targetSymbol), targetSymbol, null);
        }
    
        #region Data Assigning

        void AssignShuffledCardData()
        {
            List<CardData> shuffledCardData = GenerateAndShuffleDeck();

            for (int i = 0; i < spawnedCards.Count; i++)
            {
                spawnedCards[i].AssignCardData(shuffledCardData[i]);
            }

            for (int i = 28; i < 52; i++)
            {
                ExtraCardsContainer.instance.cardData[i - 28] = shuffledCardData[i];
            }
        }

        List<CardData> GenerateAndShuffleDeck()
        {
            List<CardData> cardDataList = new List<CardData>();

            foreach (CardSymbol symbol in System.Enum.GetValues(typeof(CardSymbol)))
            {
                for (int value = 1; value <= 13; value++)
                {
                    cardDataList.Add(new CardData { symbol = symbol, value = value });
                }
            }

            // Shuffle the card data
            cardDataList = cardDataList.OrderBy(card => Random.value).ToList();

            return cardDataList;
        }

        #endregion
    }
}