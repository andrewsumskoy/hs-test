using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace EL.UI.Components
{
    [RequireComponent(typeof(TextMeshPro))]
    public class AnimatedIntTextMeshPro : MonoBehaviour
    {
        [SerializeField] private float animationDuration = .2f;
        [SerializeField] private int initValue;
        private Tweener _animation;

        private int _value;

        public TextMeshPro Text { get; private set; }

        public int Value
        {
            get => _value;
            set => SetValue(value);
        }

        private void Awake()
        {
            Text = GetComponent<TextMeshPro>();
            SetInitValue(initValue);
        }

        public void SetInitValue(int value)
        {
            if (_value == value)
                return;
            StopAnimation();
            _value = value;
            Text.text = value.ToString();
        }

        public void SetValue(int value)
        {
            if (_value == value)
                return;
            StopAnimation();
            var currentValue = _value;
            _value = value;
            _animation = DOTween.To(() => currentValue, v =>
            {
                currentValue = v;
                Text.text = v.ToString();
            }, _value, animationDuration);
            _animation.onComplete += StopAnimation;
        }

        public Task WhenAnimationComplete()
        {
            if (_animation == null || !_animation.active)
                return Task.CompletedTask;
            return _animation.ToUniTask().AsTask();
        }

        private void StopAnimation()
        {
            if (_animation == null)
                return;
            if (_animation.active)
                DOTween.Kill(_animation);
            _animation = null;
        }
    }
}