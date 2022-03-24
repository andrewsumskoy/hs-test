using System;
using UnityEngine;

namespace EL.Res
{
    [CreateAssetMenu(menuName = "EL/GamePrefabs")]
    public class GamePrefabs : ScriptableObject
    {
        public enum ItemType
        {
            Card,
            SelectHandDialog
        }

        [SerializeField] private GameObject card;
        [SerializeField] private GameObject selectHand;

        public T Get<T>(ItemType item)
        {
            var pref = item switch
            {
                ItemType.Card => card,
                ItemType.SelectHandDialog => selectHand,
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
            return Instantiate(pref).GetComponent<T>();
        }

        public void Return(GameObject go)
        {
            Destroy(go);
        }
    }
}