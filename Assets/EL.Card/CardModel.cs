using System;
using EL.Res;

namespace EL.Card
{
    [Serializable]
    public struct CardModel : IEquatable<CardModel>
    {
        public Guid id;
        public CardDesign design;
        public int attackModificator;
        public int healthModificator;
        public int costModificator;

        public int Attack => design.attack + attackModificator;
        public int Health => design.health + healthModificator;
        public int Cost => design.cost + costModificator;

        public bool Equals(CardModel other)
        {
            return id.Equals(other.id) && design.Equals(other.design) && attackModificator == other.attackModificator &&
                   healthModificator == other.healthModificator && costModificator == other.costModificator;
        }

        public override bool Equals(object obj)
        {
            return obj is CardModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = id.GetHashCode();
                hashCode = (hashCode * 397) ^ design.GetHashCode();
                hashCode = (hashCode * 397) ^ attackModificator;
                hashCode = (hashCode * 397) ^ healthModificator;
                hashCode = (hashCode * 397) ^ costModificator;
                return hashCode;
            }
        }
    }
}