#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class CameraShaker : MonoBehaviour
    {
        protected static CameraShaker s_Instance;


        protected Vector3 m_LastVector;
        protected float m_ShakeIntensity = 0.2f;
        protected float m_SinceShakeTime;

        private void OnEnable()
        {
            s_Instance = this;
        }

        private void OnPreRender()
        {
            if (m_SinceShakeTime > 0.0f)
            {
                m_LastVector = Random.insideUnitCircle * m_ShakeIntensity;
                transform.localPosition = transform.localPosition + m_LastVector;
            }
        }

        private void OnPostRender()
        {
            if (m_SinceShakeTime > 0.0f)
            {
                transform.localPosition = transform.localPosition - m_LastVector;
                m_SinceShakeTime -= Time.deltaTime;
            }
        }


        public static void Shake(float amount, float time)
        {
            if (s_Instance == null)
                return;

            s_Instance.m_ShakeIntensity = amount;
            s_Instance.m_SinceShakeTime = time;
        }
    }
}