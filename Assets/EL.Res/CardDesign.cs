using System;

namespace EL.Res
{
    [Serializable]
    public struct CardDesign : IEquatable<CardDesign>
    {
        public string id;
        public int attack;
        public int health;
        public int cost;

        public bool Equals(CardDesign other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is CardDesign other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}