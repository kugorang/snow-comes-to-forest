#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class GunnerShieldEffect : MonoBehaviour
    {
        private const int count = 2;
        protected float m_Intensity;
        protected Material m_Material;

        private void Start()
        {
            var renderer = GetComponent<SpriteRenderer>();
            m_Material = renderer.material;
            m_Intensity = 0.0f;
        }

        public void ShieldHit(Damager damager, Damageable damageable)
        {
            var localPosition = transform.InverseTransformPoint(damager.transform.position);

            m_Material.SetVector("_HitPosition", localPosition);
            m_Intensity = 1.0f;
        }

        private void Update()
        {
            if (m_Intensity > 0.0f) m_Intensity = Mathf.Clamp(m_Intensity - Time.deltaTime, 0, 1);

            m_Material.SetFloat("_HitIntensity", m_Intensity);
        }
    }
}