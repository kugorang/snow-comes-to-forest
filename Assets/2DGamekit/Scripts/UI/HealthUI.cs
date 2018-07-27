#region

using System.Collections;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class HealthUI : MonoBehaviour
    {
        protected const float k_HeartIconAnchorWidth = 0.041f;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
        public GameObject healthIconPrefab;

        protected Animator[] m_HealthIconAnimators;
        public Damageable representedDamageable;

        private IEnumerator Start()
        {
            if (representedDamageable == null)
                yield break;

            yield return null;

            m_HealthIconAnimators = new Animator[representedDamageable.startingHealth];

            for (var i = 0; i < representedDamageable.startingHealth; i++)
            {
                var healthIcon = Instantiate(healthIconPrefab);
                healthIcon.transform.SetParent(transform);
                var healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

                if (representedDamageable.CurrentHealth < i + 1)
                {
                    m_HealthIconAnimators[i].Play(m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
                }
            }
        }

        public void ChangeHitPointUI(Damageable damageable)
        {
            if (m_HealthIconAnimators == null)
                return;

            for (var i = 0; i < m_HealthIconAnimators.Length; i++)
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.CurrentHealth >= i + 1);
        }
    }
}