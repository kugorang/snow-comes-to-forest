#region

using System.Collections;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class SpriteColorChange : MonoBehaviour
    {
        private Color m_InitialColor;
        private SpriteRenderer m_SpriteRenderer;

        public Color newColor = Color.white;
        public float timer = 0.2f;

        private void OnEnable()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_InitialColor = m_SpriteRenderer.color;
            StartCoroutine("ChangeColor");
        }

        private IEnumerator ChangeColor()
        {
            m_SpriteRenderer.color = newColor;
            yield return new WaitForSeconds(timer);
            m_SpriteRenderer.color = m_InitialColor;
            enabled = false;
        }
    }
}