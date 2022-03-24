using EL.UI.Components;
using UnityEngine;

namespace EL.Card
{
    public class CardStatTextValue : AnimatedIntTextMeshPro
    {
        public enum ValueType
        {
            Negative,
            Default,
            Positive
        }

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color negativeColor;
        [SerializeField] private Color positiveColor;
        private ValueType _valueTypeType = ValueType.Default;

        public ValueType CurrentValueType
        {
            get => _valueTypeType;
            set
            {
                if (_valueTypeType == value)
                    return;
                _valueTypeType = value;
                Text.color = _valueTypeType switch
                {
                    ValueType.Default => defaultColor,
                    ValueType.Positive => positiveColor,
                    _ => negativeColor
                };
            }
        }
    }
}