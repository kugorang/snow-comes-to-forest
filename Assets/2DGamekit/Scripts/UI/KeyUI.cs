#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class KeyUI : MonoBehaviour
    {
        protected const float k_KeyIconAnchorWidth = 0.041f;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");

        public GameObject keyIconPrefab;
        public string[] keyNames;

        protected Animator[] m_KeyIconAnimators;
        public static KeyUI Instance { get; protected set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetInitialKeyCount();
        }

        public void SetInitialKeyCount()
        {
            if (m_KeyIconAnimators != null && m_KeyIconAnimators.Length == keyNames.Length)
                return;

            m_KeyIconAnimators = new Animator[keyNames.Length];

            for (var i = 0; i < m_KeyIconAnimators.Length; i++)
            {
                var healthIcon = Instantiate(keyIconPrefab);
                healthIcon.transform.SetParent(transform);
                var healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin -= new Vector2(k_KeyIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax -= new Vector2(k_KeyIconAnchorWidth, 0f) * i;
                m_KeyIconAnimators[i] = healthIcon.GetComponent<Animator>();
            }
        }

        public void ChangeKeyUI(InventoryController controller)
        {
            for (var i = 0; i < keyNames.Length; i++)
                m_KeyIconAnimators[i].SetBool(m_HashActivePara, controller.HasItem(keyNames[i]));
        }
    }
}