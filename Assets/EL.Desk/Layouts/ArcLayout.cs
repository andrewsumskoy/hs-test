using System;
using System.Collections.Generic;
using System.Linq;
using EL.Card;
using UnityEngine;

namespace EL.Desk.Layouts
{
    [ExecuteInEditMode]
    public class ArcLayout : DeckLayout
    {
        [SerializeField] private Vector2 degBetweenRange = new Vector2(5f, 15f);
        [SerializeField] private Vector2 cardDegRange = new Vector2(5f, 15f);
        private float _arc;

        private Bounds _circleBounds;
        private Vector2 _circleCenter;
        private float _circleRadius;

        private void OnDrawGizmosSelected()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1f, .01f, 1f));

            Gizmos.DrawWireSphere(new Vector3(_circleCenter.x, 0f, _circleCenter.y), _circleRadius);
            Gizmos.matrix = oldMatrix;
        }

        public override LayoutElement[] ReLayout(IEnumerable<CardView> cards)
        {
            var cardsList = cards.Select(c => c.Collider).ToArray();
            CalculateCircle(GetAllowedSpace(cardsList.Length).bounds);
            return LayoutChildren(cardsList);
        }

        private LayoutElement[] LayoutChildren(BoxCollider[] children)
        {
            if (children == null || children.Length == 0)
                return Array.Empty<LayoutElement>();

            var result = new List<LayoutElement>();
            var radBetweenCards = Mathf.Clamp(_arc / children.Length, degBetweenRange.x * Mathf.Deg2Rad,
                degBetweenRange.y * Mathf.Deg2Rad);
            var start = radBetweenCards * (children.Length - 1f) / -2f + Mathf.PI / 2f;
            for (var i = 0; i < children.Length; i++)
            {
                var pointX = _circleCenter.x + _circleRadius * Mathf.Cos(start);
                var pointY = _circleCenter.y + _circleRadius * Mathf.Sin(start);
                var nextPosition = new Vector3(pointX, _circleBounds.center.y, pointY);

                var directionFromCenter =
                    Vector3.Normalize(nextPosition - new Vector3(_circleCenter.x, 0f, _circleCenter.y));
                var targetRotation = Quaternion.LookRotation(directionFromCenter, Vector3.up).eulerAngles;
                if (targetRotation.y > 180f) targetRotation.y -= 360f;
                var nextRotation =
                    Quaternion.Euler(0f, Mathf.Clamp(targetRotation.y, cardDegRange.x, cardDegRange.y), 0f);
                result.Add(new LayoutElement
                {
                    Transform = children[i].transform,
                    LocalPosition = nextPosition,
                    LocalRotation = nextRotation
                });
                start += radBetweenCards;
            }

            return result.ToArray();
        }

        private void CalculateCircle(Bounds bounds)
        {
            if (_circleBounds == bounds)
                return;

            _circleBounds = bounds;

            var min = transform.InverseTransformPoint(_circleBounds.min);
            var max = transform.InverseTransformPoint(_circleBounds.max);
            var center = transform.InverseTransformPoint(_circleBounds.center);

            var left = new Vector2(min.x, min.z);
            var right = new Vector2(max.x, min.z);
            var middle = new Vector2(center.x, max.z);

            (_circleCenter, _circleRadius) = FindCircle(left, right, middle);

            _arc = 2f * Mathf.Asin(Mathf.Sqrt(Mathf.Pow(right.x - left.x, 2f) + Mathf.Pow(right.y - left.y, 2f)) /
                                   (2f * _circleRadius));
        }

        private (Vector2, float) FindCircle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var offset = Mathf.Pow(p2.x, 2) + Mathf.Pow(p2.y, 2);
            var bc = (Mathf.Pow(p1.x, 2) + Mathf.Pow(p1.y, 2) - offset) / 2f;
            var cd = (offset - Mathf.Pow(p3.x, 2) - Mathf.Pow(p3.y, 2)) / 2f;
            var det = (p1.x - p2.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p2.y);

            var idet = 1 / det;

            var centerX = (bc * (p2.y - p3.y) - cd * (p1.y - p2.y)) * idet;
            var centerY = (cd * (p1.x - p2.x) - bc * (p2.x - p3.x)) * idet;
            var radius =
                Mathf.Sqrt(Mathf.Pow(p2.x - centerX, 2) + Mathf.Pow(p2.y - centerY, 2));

            return (new Vector2(centerX, centerY), radius);
        }
    }
}