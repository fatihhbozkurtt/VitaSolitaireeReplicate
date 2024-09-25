using System.Collections.Generic;

namespace Data
{
    [System.Serializable]
    public class MovePathData
    {
        public ICardStacker from;
        public ICardStacker to;
        public CardController card;
        public List<CardController> followerCards;
    }
}