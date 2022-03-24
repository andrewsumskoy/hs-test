using System;
using System.Collections.Generic;
using EL.Card;
using UnityEngine;

namespace EL.Desk.Layouts
{
    public abstract class DeckLayout : MonoBehaviour
    {
        [SerializeField] protected SpaceByCount[] colliders;
        public abstract LayoutElement[] ReLayout(IEnumerable<CardView> cards);

        protected BoxCollider GetAllowedSpace(int count)
        {
            if (colliders == null || colliders.Length == 0)
                throw new ArgumentException("No colliders set");
            foreach (var byCount in colliders)
                if (count >= byCount.min && byCount.max >= count)
                    return byCount.boxCollider;

            return colliders[0].boxCollider;
        }

        public struct LayoutElement
        {
            public Transform Transform;
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
        }

        [Serializable]
        public struct SpaceByCount
        {
            public int min;
            public int max;
            public BoxCollider boxCollider;
        }
    }
}