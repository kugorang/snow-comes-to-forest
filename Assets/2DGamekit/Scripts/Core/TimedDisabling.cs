#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class TimedDisabling : MonoBehaviour
    {
        private float m_Timer;

        public float timeBeforeDisable = 1.0f;

        private void OnEnable()
        {
            m_Timer = timeBeforeDisable;
        }

        private void Update()
        {
            m_Timer -= Time.deltaTime;

            if (m_Timer < 0.0f) gameObject.SetActive(false);
        }
    }
}