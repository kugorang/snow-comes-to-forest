#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class DropShadow : MonoBehaviour
    {
        public LayerMask levelLayermask;
        protected RaycastHit2D[] m_ContactCache = new RaycastHit2D[6];
        protected ContactFilter2D m_ContactFilter;
        protected Vector3 m_OriginalSize;

        protected SpriteRenderer m_SpriteRenderer;
        public float maxHeight = 3.0f;
        public float offset = -0.2f;
        public GameObject origin;
        public float originOffset = 0.4f;

        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_OriginalSize = transform.localScale;

            m_ContactFilter = new ContactFilter2D();
            m_ContactFilter.layerMask = levelLayermask;
            m_ContactFilter.useLayerMask = true;
        }

        private void LateUpdate()
        {
            var count = Physics2D.Raycast((Vector2) origin.transform.position + Vector2.up * originOffset, Vector2.down,
                m_ContactFilter, m_ContactCache);

            if (count > 0)
            {
                m_SpriteRenderer.enabled = true;
                transform.position = m_ContactCache[0].point + m_ContactCache[0].normal * offset;

                var height = Vector3.SqrMagnitude(origin.transform.position - transform.position);
                var ratio = Mathf.Clamp(1.0f - height / (maxHeight * maxHeight), 0.0f, 1.0f);

                transform.localScale = m_OriginalSize * ratio;
            }
            else
            {
                m_SpriteRenderer.enabled = false;
            }
        }
    }
}