using System;
using UnityEngine;

namespace PopupUtility
{
    public class PopupsManager : MonoSingleton<PopupsManager>
    {
        [SerializeField] private SimpleMessagePopup m_SimplePopup;
        [SerializeField] private SimpleToastCanvas m_SimpleToast;

        public void ShowSimpleMessage(string title, string message, Action onOkClick = null,
            Action onCancelClick = null)
        {
            m_SimplePopup.Show(title, message, onOkClick, onCancelClick);
        }

        public void ShowSimpleToastMessage(string message)
        {
            m_SimpleToast.ShowToast(message);
        }
    }
}
