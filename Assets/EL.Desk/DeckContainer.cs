using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EL.Card;
using EL.Desk.Layouts;
using UnityEngine;

namespace EL.Desk
{
    public class DeckContainer : MonoBehaviour, ICardViewContainer
    {
        [SerializeField] protected DeckLayout cardLayout;

        [SerializeField] protected float animationMoveDuration = .3f;

        protected List<Card.Card> _cards;
        protected Sequence _reAlignAnimation;
        protected List<CardView> _views;
        public Card.Card[] Cards => _cards.ToArray();
        public Card.Card[] AttachedCards => _views.Select(v => v.Card).ToArray();

        public float AnimationMoveDuration => animationMoveDuration;

        protected virtual void Awake()
        {
            _cards = new List<Card.Card>();
            _views = new List<CardView>();
        }

        public Transform Container => cardLayout.transform;

        public void RegisterCardView(CardView view)
        {
            _views.Add(view);
            for (var i = 0; i < _views.Count; i++)
                _views[i].OrderLayout = _views.Count - i;
            ReAlign();
        }

        public void UnRegisterCardView(CardView view)
        {
            _views.Remove(view);
            ReAlign();
        }

        public void AddCard(IEnumerable<Card.Card> items)
        {
            foreach (var item in items)
                AddCard(item);
        }

        public virtual void AddCard(Card.Card card)
        {
            _cards.Add(card);
            card.View.AttachView(this);
        }

        public virtual void RemoveCard(Card.Card card)
        {
            if (!_cards.Contains(card))
                return;
            _cards.Remove(card);
            card.View.DetachView();
        }

        public virtual void RemoveAllCards()
        {
            foreach (var cardView in _cards.ToArray())
                RemoveCard(cardView);
        }

        public DeckLayout.LayoutElement[] CalculateReLayout()
        {
            return cardLayout.ReLayout(_views);
        }

        protected virtual void ReAlign()
        {
            if (_reAlignAnimation?.active ?? false)
                DOTween.Kill(_reAlignAnimation);
            _reAlignAnimation = DOTween.Sequence();
            foreach (var layoutElement in CalculateReLayout())
                _reAlignAnimation
                    .Join(layoutElement.Transform.DOLocalMove(layoutElement.LocalPosition, animationMoveDuration))
                    .Join(layoutElement.Transform.DOLocalRotateQuaternion(layoutElement.LocalRotation,
                        animationMoveDuration));
        }
    }
}