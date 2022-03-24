using System.Threading.Tasks;
using EL.Common;
using EL.Res;
using UnityEngine;

namespace EL.Card
{
    public class Card : IDraggable
    {
        private CardModel _model;

        public Card(
            CardView cardView,
            CardModel model,
            IGameResources gameResources,
            ITranslate translate,
            Camera cam)
        {
            View = cardView;
            cardView.Link(this, gameResources, translate, cam);
            SetModel(model);
        }

        public CardView View { get; }

        public CardModel Model => _model;

        public Task SetModel(CardModel value)
        {
            if (_model.Equals(value))
                return Task.CompletedTask;
            _model = value;
            return View.OnModelUpdated(_model);
        }

        public Subscription AllowDragAndDrop(DragAndDropState state)
        {
            void Start()
            {
                state.OnStart(this);
            }

            void Cancel()
            {
                state.OnCancel(this);
            }

            void Drop()
            {
                state.OnDrop(this);
            }

            View.Drag.AllowDrag = true;
            View.Drag.OnDragStart += Start;
            View.Drag.OnDragCancel += Cancel;
            View.Drag.OnDrop += Drop;
            return new Subscription(() =>
            {
                View.Drag.AllowDrag = false;
                View.Drag.OnDragStart -= Start;
                View.Drag.OnDragCancel -= Cancel;
                View.Drag.OnDrop -= Drop;
            });
        }
    }
}