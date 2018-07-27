#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class GunnerProjectile : MonoBehaviour
    {
        public GameObject explosion;
        public float explosionTimer = 3;
        public float fuse = 0.01f;
        public Vector2 initialForce;

        protected GameObject m_HitEffect;
        private new Rigidbody2D rigidbody;
        public float timer = 1;

        private void OnEnable()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Destroy(gameObject, timer);

            m_HitEffect = Instantiate(explosion);
            m_HitEffect.SetActive(false);
        }

        public void Destroy()
        {
            m_HitEffect.transform.position = transform.position;
            m_HitEffect.SetActive(true);
            Destroy(m_HitEffect, explosionTimer);
            Destroy(gameObject);
        }

        private void Start()
        {
            rigidbody.AddForce(initialForce);
        }
    }
}