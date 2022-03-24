using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EL.Res;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace EL.Card
{
    public class CardView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private CardDrag drag;
        [SerializeField] private BoxCollider boundsCollider;
        [SerializeField] private CardStatTextValue costValueView;
        [SerializeField] private CardStatTextValue attackValueView;
        [SerializeField] private CardStatTextValue healthValueView;
        [SerializeField] private TextMeshPro titleValueView;
        [SerializeField] private TextMeshPro descriptionValueView;
        [SerializeField] private SpriteRenderer mainImage;
        [SerializeField] private ParticleSystem isSelectedView;
        [SerializeField] private SortingGroup sortingGroup;

        private CardModel _currentViewCardModel;
        private ICardViewContainer _currentViewContainer;
        private IGameResources _gameResources;
        private ITranslate _translate;

        internal CardDrag Drag => drag;

        public Card Card { get; private set; }

        public BoxCollider Collider => boundsCollider;

        public bool IsSelected
        {
            get => !isSelectedView.isStopped;
            set
            {
                if (value)
                    isSelectedView.Play(true);
                else
                    isSelectedView.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public int OrderLayout
        {
            get => sortingGroup.sortingOrder;
            set => sortingGroup.sortingOrder = value;
        }

        private void Awake()
        {
            IsSelected = false;
            drag.AllowDrag = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(Card);
        }

        public event Action<Card> OnClick;

        internal void Link(Card card, IGameResources gameResources, ITranslate translate, Camera cam)
        {
            Card = card;
            _gameResources = gameResources;
            _translate = translate;
            drag.Self = card;
            drag.Cam = cam;
        }

        internal Task OnModelUpdated(CardModel cardModel)
        {
            var changeDesignData = !_currentViewCardModel.design.Equals(cardModel.design);
            var changeCost = changeDesignData || _currentViewCardModel.costModificator != cardModel.costModificator;
            var changeAttack =
                changeDesignData || _currentViewCardModel.attackModificator != cardModel.attackModificator;
            var changeHealth =
                changeDesignData || _currentViewCardModel.healthModificator != cardModel.healthModificator;

            var awaitAnimations = new List<Task>();

            if (changeCost)
                awaitAnimations.Add(
                    UpdateLabel(
                        costValueView,
                        cardModel.Cost,
                        cardModel.costModificator == 0 ? CardStatTextValue.ValueType.Default :
                        cardModel.costModificator > 0 ? CardStatTextValue.ValueType.Negative :
                        CardStatTextValue.ValueType.Positive));

            if (changeAttack)
                awaitAnimations.Add(
                    UpdateLabel(attackValueView, cardModel.Attack,
                        cardModel.costModificator == 0 ? CardStatTextValue.ValueType.Default :
                        cardModel.costModificator > 0 ? CardStatTextValue.ValueType.Positive :
                        CardStatTextValue.ValueType.Negative));

            if (changeHealth)
                awaitAnimations.Add(
                    UpdateLabel(healthValueView, cardModel.Health,
                        cardModel.costModificator == 0 ? CardStatTextValue.ValueType.Default :
                        cardModel.costModificator > 0 ? CardStatTextValue.ValueType.Positive :
                        CardStatTextValue.ValueType.Negative));

            if (changeDesignData)
            {
                titleValueView.text = _translate.Translate($"card.{cardModel.design.id}.title");
                descriptionValueView.text = _translate.Translate($"card.{cardModel.design.id}.description");
                mainImage.sprite = _gameResources.GetCardSprite(cardModel.design.id);
                gameObject.name = "Card_" + cardModel.id;
            }

            _currentViewCardModel = cardModel;
            return Task.WhenAll(awaitAnimations);
        }

        private Task UpdateLabel(CardStatTextValue label, int value, CardStatTextValue.ValueType valueType)
        {
            label.Value = value;
            label.CurrentValueType = valueType;
            return label.WhenAnimationComplete();
        }

        public void AttachView(ICardViewContainer to)
        {
            _currentViewContainer?.UnRegisterCardView(this);
            _currentViewContainer = to;
            transform.SetParent(_currentViewContainer.Container, true);
            _currentViewContainer.RegisterCardView(this);
        }

        public void DetachView()
        {
            _currentViewContainer?.UnRegisterCardView(this);
            _currentViewContainer = null;
            OrderLayout = 0;
            IsSelected = false;
            transform.SetParent(null, true);
        }
    }
}