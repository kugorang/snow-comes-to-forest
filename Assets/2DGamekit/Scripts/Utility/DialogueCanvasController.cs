#region

using System.Collections;
using TMPro;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class DialogueCanvasController : MonoBehaviour
    {
        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        public Animator animator;

        protected Coroutine m_DeactivationCoroutine;
        public TextMeshProUGUI textMeshProUGUI;

        private IEnumerator SetAnimatorParameterWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetBool(m_HashActivePara, false);
        }

        public void ActivateCanvasWithText(string text)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive(true);
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = text;
        }

        public void ActivateCanvasWithTranslatedText(string phraseKey)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive(true);
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = Translator.Instance[phraseKey];
        }

        public void DeactivateCanvasWithDelay(float delay)
        {
            m_DeactivationCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));
        }
    }
}