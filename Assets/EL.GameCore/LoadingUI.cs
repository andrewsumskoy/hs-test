using System;
using TMPro;
using UnityEngine;

namespace EL.GameCore
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI error;

        private void Awake()
        {
            error.text = "";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetException(Exception ex)
        {
            error.text = ex.Message;
            Debug.LogException(ex);
        }
    }
}