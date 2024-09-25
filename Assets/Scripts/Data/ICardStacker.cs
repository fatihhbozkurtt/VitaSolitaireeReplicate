using UnityEngine;

namespace Data
{
    public interface ICardStacker
    {
        public void AddCard(CardController newCard);
        public void RemoveCard(CardController card);
        public CardController GetLastCard();

        public Vector3 GetPlacementPos();
    
        public Transform GetTransform();
    }
}