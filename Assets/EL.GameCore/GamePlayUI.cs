using System;
using TMPro;
using UnityEngine;

namespace EL.GameCore
{
    public class GamePlayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        public string NextStageName
        {
            get => label.text;
            set => label.text = value;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public event Action OnTestClick;

        public void HandleTestClick()
        {
            OnTestClick?.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}