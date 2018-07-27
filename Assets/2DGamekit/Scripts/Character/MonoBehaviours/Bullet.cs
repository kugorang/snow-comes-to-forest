#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Damager))]
    public class Bullet : MonoBehaviour
    {
        private const float k_OffScreenError = 0.01f;
        private static readonly int VFX_HASH = VFXController.StringToHash("BulletImpact");

        [HideInInspector] public BulletObject bulletPoolObject;

        public bool destroyWhenOutOfView = true;

        protected SpriteRenderer m_SpriteRenderer;

        protected float m_Timer;

        [HideInInspector] public Camera mainCamera;

        public bool spriteOriginallyFacesLeft;

        [Tooltip("If -1 never auto destroy, otherwise bullet is return to pool when that time is reached")]
        public float timeBeforeAutodestruct = -1.0f;

        private void OnEnable()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Timer = 0.0f;
        }

        public void ReturnToPool()
        {
            bulletPoolObject.ReturnToPool();
        }

        private void FixedUpdate()
        {
            if (destroyWhenOutOfView)
            {
                var screenPoint = mainCamera.WorldToViewportPoint(transform.position);
                var onScreen = screenPoint.z > 0 && screenPoint.x > -k_OffScreenError &&
                               screenPoint.x < 1 + k_OffScreenError && screenPoint.y > -k_OffScreenError &&
                               screenPoint.y < 1 + k_OffScreenError;
                if (!onScreen)
                    bulletPoolObject.ReturnToPool();
            }

            if (timeBeforeAutodestruct > 0)
            {
                m_Timer += Time.deltaTime;
                if (m_Timer > timeBeforeAutodestruct) bulletPoolObject.ReturnToPool();
            }
        }

        public void OnHitDamageable(Damager origin, Damageable damageable)
        {
            FindSurface(origin.LastHit);
        }

        public void OnHitNonDamageable(Damager origin)
        {
            FindSurface(origin.LastHit);
        }

        protected void FindSurface(Collider2D collider)
        {
            var forward = spriteOriginallyFacesLeft ? Vector3.left : Vector3.right;
            if (m_SpriteRenderer.flipX) forward.x = -forward.x;

            var surfaceHit = PhysicsHelper.FindTileForOverride(collider, transform.position, forward);

            VFXController.Instance.Trigger(VFX_HASH, transform.position, 0, m_SpriteRenderer.flipX, null, surfaceHit);
        }
    }
}