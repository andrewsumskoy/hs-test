using UnityEngine;

namespace EL.Res
{
    [CreateAssetMenu(menuName = "EL/Card Design")]
    public class CardDesignLocalStored : ScriptableObject
    {
        public Sprite mainImage;
        public CardDesign data;
    }
}