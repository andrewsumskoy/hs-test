using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EL.Desk;
using UnityEngine;

namespace EL.Popups
{
    public class SelectCardsView : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private DeckContainer container;
        [SerializeField] private Transform spawnPoint;
        public bool IsComplete { get; private set; }

        public void Reset()
        {
            IsComplete = false;
            container.RemoveAllCards();
        }

        public void Show(Camera cam)
        {
            canvas.worldCamera = cam;
        }

        public async Task SetViews(IEnumerable<Card.Card> cards)
        {
            foreach (var cardItem in cards)
            {
                cardItem.View.transform.position = spawnPoint.position;
                container.AddCard(cardItem);
            }

            var toMoveElements = container.CalculateReLayout();
            for (var i = 0; i < toMoveElements.Length; i++)
            {
                if (i != 0)
                    await UniTask.Delay(Mathf.FloorToInt(container.AnimationMoveDuration * 1000 / 3f));
                var layoutElement = toMoveElements[i];
                var moveTween =
                    layoutElement.Transform.DOLocalMove(layoutElement.LocalPosition, container.AnimationMoveDuration);
                layoutElement.Transform.DOLocalRotateQuaternion(layoutElement.LocalRotation,
                    container.AnimationMoveDuration);
                if (i + 1 == toMoveElements.Length)
                    await moveTween;
            }
        }

        public void DoComplete()
        {
            IsComplete = true;
        }
    }
}