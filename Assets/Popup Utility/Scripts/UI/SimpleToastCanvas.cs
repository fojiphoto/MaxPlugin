using TMPro;
using UnityEngine;

namespace PopupUtility
{
    public class SimpleToastCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ToastText;
        [SerializeField] private Animator m_Animator;

        [SerializeField] private float m_ToastDuration = 3f;

        private readonly int m_PopShowAnimateParameter = Animator.StringToHash("Show");
        private readonly int m_PopHideAnimateParameter = Animator.StringToHash("Hide");

        public void ShowToast(string toastText)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (IsInvoking(nameof(HideToast)))
                CancelInvoke(nameof(HideToast));

            m_ToastText.text = toastText;
            SetAnimatorState(m_PopShowAnimateParameter);

            Invoke("HideToast", m_ToastDuration);
        }

        public void HideToast()
        {
            SetAnimatorState(m_PopHideAnimateParameter);
        }

        private void SetAnimatorState(int parameter)
        {
            m_Animator.SetTrigger(parameter);
        }
    }
}