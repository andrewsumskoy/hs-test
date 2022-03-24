using System;
using System.Collections.Generic;
using DG.Tweening;
using EL.Card;
using EL.Common;
using UnityEngine;

namespace EL.Desk
{
    public class HandContainer : DeckContainer
    {
        private Dictionary<Guid, Card.Card> _cardById;
        private Dictionary<Guid, Subscription> _enableSubscriptions;

        protected override void Awake()
        {
            base.Awake();
            _cardById = new Dictionary<Guid, Card.Card>();
            _enableSubscriptions = new Dictionary<Guid, Subscription>();
        }

        public override void AddCard(Card.Card item)
        {
            _cardById[item.Model.id] = item;
            base.AddCard(item);
            _enableSubscriptions.Add(
                item.Model.id,
                item.AllowDragAndDrop(new DragAndDropState
                {
                    OnCancel = OnCardDragCancel,
                    OnDrop = OnCardDrop,
                    OnStart = OnCardDragStart
                }));
        }

        public override void RemoveCard(Card.Card card)
        {
            base.RemoveCard(card);
            CleanCardSubscriptions(card);
        }

        private void OnCardDragStart(Card.Card item)
        {
            item.View.DetachView();
            item.View.OrderLayout = 5;
            item.View.IsSelected = true;
            item.View.transform.DOLocalMoveY(2.5f, .1f);
            item.View.transform.DOLocalMoveZ(item.View.transform.localPosition.z + 1f, .1f);
            item.View.transform.DORotate(Vector3.zero, .1f);
        }

        private void OnCardDragCancel(Card.Card item)
        {
            item.View.IsSelected = false;
            item.View.AttachView(this);
        }

        private void OnCardDrop(Card.Card item)
        {
            CleanCardSubscriptions(item);
        }

        private void CleanCardSubscriptions(Card.Card item)
        {
            if (_enableSubscriptions.TryGetValue(item.Model.id, out var sub))
                sub.Dispose();
            _enableSubscriptions.Remove(item.Model.id);
        }
    }
}