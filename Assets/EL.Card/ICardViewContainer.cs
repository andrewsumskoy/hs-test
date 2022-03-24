using UnityEngine;

namespace EL.Card
{
    public interface ICardViewContainer
    {
        Transform Container { get; }

        void RegisterCardView(CardView view);
        void UnRegisterCardView(CardView view);
    }
}