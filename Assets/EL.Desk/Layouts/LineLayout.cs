using System.Collections.Generic;
using System.Linq;
using EL.Card;
using UnityEngine;

namespace EL.Desk.Layouts
{
    public class LineLayout : DeckLayout
    {
        [SerializeField] private float spaceBetween = .1f;

        public override LayoutElement[] ReLayout(IEnumerable<CardView> cards)
        {
            var result = new List<LayoutElement>();

            var toLayout = cards.ToArray();
            var totalWidth = toLayout.Sum(el => el.Collider.bounds.size.x) + spaceBetween * (toLayout.Length - 1);
            var center = GetAllowedSpace(toLayout.Length).bounds.center;
            var currentOffset = -totalWidth / 2f;
            for (var i = 0; i < toLayout.Length; i++)
            {
                var cardItem = toLayout[i];
                var cardBounds = cardItem.Collider.bounds;
                if (i == 0)
                    currentOffset += cardBounds.size.x / 2f;
                result.Add(new LayoutElement
                {
                    Transform = cardItem.transform,
                    LocalPosition = new Vector3(center.x + currentOffset, center.y, 0f),
                    LocalRotation = Quaternion.identity
                });
                currentOffset += cardBounds.size.x + spaceBetween;
            }

            return result.ToArray();
        }
    }
}