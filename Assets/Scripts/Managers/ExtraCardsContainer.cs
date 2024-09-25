using System.Collections.Generic;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class ExtraCardsContainer : MonoSingleton<ExtraCardsContainer>, ICardStacker
    {
        [Header("References")] [SerializeField]
        private CardController cardPerfab;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject refreshPanel;
        [SerializeField] private TextMeshProUGUI text;

        [Header("Debug")] public List<CardData> cardData;
        [SerializeField] private List<CardController> displayingCards;
        [SerializeField] private List<CardController> hiddenCards;
        [SerializeField] private int clickedCount;
        [SerializeField] private int removedCardsCount;
        private int maxCardCount;

        private void Start()
        {
            maxCardCount = 24;
        }

        public void GatherAllCards()
        {
            Debug.Log("gathering");
            foreach (var card in displayingCards)
            {
                card.gameObject.SetActive(true);
                Sequence sq = DOTween.Sequence();
                sq.Append(card.transform.DOLocalMove(Vector3.zero, 0.4f));
                sq.Join(card.transform.DOScale(Vector3.zero, 0.1f));

                sq.OnComplete(() => Destroy(card.gameObject));
            }

            maxCardCount = 24 - removedCardsCount;
            ResetParams();
        }

        void OnAllCardsDisplayed()
        {
            mesh.SetActive(false);
            refreshPanel.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (cardData.Count < 1) return;
            if (clickedCount == maxCardCount)
            {
                GatherAllCards();
                return;
            }

            MoveCountManager.instance.IncrementMoveCount();
            clickedCount++;

            if (clickedCount != 0 && clickedCount > 3)
            {
                for (int i = 0; i < displayingCards.Count - 2; i++)
                {
                    if (hiddenCards.Contains(displayingCards[i])) continue;
                    hiddenCards.Add(displayingCards[i]);
                }

                // hiding the leftmost card 
                foreach (var c in hiddenCards)
                {
                    c.transform.DOScale(Vector3.zero, 0.25f).SetDelay(0.1f);
                }
            }

            text.text = (maxCardCount - clickedCount).ToString();

            CardController cloneCard = Instantiate(cardPerfab, transform.position,
                Quaternion.identity, transform);

            cloneCard.Enable();
            cloneCard.AssignCardData(cardData[clickedCount - 1]);
            displayingCards.Add(cloneCard);

            for (var i = 0; i < displayingCards.Count; i++)
            {
                CardController card = displayingCards[i];
                if (card != cloneCard) card.SetActivationStatus(false); // set inactive the previously spawned cards

                var value = Mathf.Abs(i - displayingCards.Count);
                var movePos = new Vector3(value * -.4f, 0, value * 0.01f);
                if (!hiddenCards.Contains(card))
                    card.transform.DOLocalMove(movePos, 0.25f);
            }

            if (clickedCount == maxCardCount)
            {
                OnAllCardsDisplayed();
            }
        }

        private void ResetParams()
        {
            displayingCards.Clear();
            hiddenCards.Clear();
            clickedCount = 0;

            mesh.SetActive(true);
            refreshPanel.SetActive(false);
            text.text = maxCardCount.ToString();
        }

        private void RearrangeCardsPositions()
        {
            if (displayingCards.Count >= 3)
            {
                if (!hiddenCards[^1]) return;

                hiddenCards[^1].transform.DOScale(Vector3.one, 0.15f);
                hiddenCards.Remove(hiddenCards[^1]);
            }

            int multiplier = 1;
            for (int i = displayingCards.Count - 1; i >= 0; i--)
            {
                var movePos = new Vector3(multiplier * -.4f, 0, multiplier * 0.01f);
                displayingCards[i].transform.DOLocalMove(movePos, 0.25f);

                multiplier++;
            }

            // Activate the rightmost card
            if (displayingCards.Count > 0)
                displayingCards[^1].SetActivationStatus(true);
        }

        #region ICardStacker

        public void AddCard(CardController newCard)
        {
            // when a card is added because of UNDO clicked
            if (displayingCards.Contains(newCard)) return;

            displayingCards.Add(newCard);
            removedCardsCount--;


            if (displayingCards[^4] && !hiddenCards.Contains(displayingCards[^4]))
            {
                CardController cardWillHidden = displayingCards[^4];
                hiddenCards.Add(cardWillHidden);
                cardWillHidden.transform.DOScale(Vector3.zero, 0.25f).SetDelay(0.1f);
            }

            RearrangeCardsPositions();
        }

        public void RemoveCard(CardController card)
        {
            if (!displayingCards.Contains(card)) return;
            displayingCards.Remove(card);
            removedCardsCount++;

            RearrangeCardsPositions();
        }

        public CardController GetLastCard()
        {
            return displayingCards[^1];
        }

        public Vector3 GetPlacementPos()
        {
            return new Vector3(-.4f, 0, 0.01f);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        #endregion
    }
}