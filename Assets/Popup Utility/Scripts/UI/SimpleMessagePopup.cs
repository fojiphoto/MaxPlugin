using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopupUtility
{
    public class SimpleMessagePopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_MessageText;
        [SerializeField] private TextMeshProUGUI m_TitleText;

        [SerializeField] private Button m_OkayButton;
        [SerializeField] private Button m_CancelButton;

        private void OnDisable()
        {
            m_CancelButton.gameObject.SetActive(false);
        }

        public void Show(string title, string messageText, Action onOkayButton, Action onCancelButton)
        {
            m_TitleText.text = title;
            m_MessageText.text = messageText;

            ButtonAction(m_OkayButton, onOkayButton);

            if (onCancelButton is not null)
            {
                m_CancelButton.gameObject.SetActive(true);
                ButtonAction(m_CancelButton, onCancelButton);
            }

            gameObject.SetActive(true);
        }

        private void ButtonAction(Button button, Action action)
        {
            button.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                action?.Invoke();
            });
        }
    }
}
