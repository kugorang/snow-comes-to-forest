#region

using System;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    public class Damager : MonoBehaviour
    {
        [Tooltip("If disabled, damager ignore trigger when casting for damage")]
        public bool canHitTriggers;

        public int damage = 1;
        public bool disableDamageAfterHit;

        [Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
        public bool forceRespawn;

        public LayerMask hittableLayers;

        [Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
        public bool ignoreInvincibility;

        protected ContactFilter2D m_AttackContactFilter;
        protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
        protected bool m_CanDamage = true;
        protected Transform m_DamagerTransform;
        protected Collider2D m_LastHit;

        protected bool m_SpriteOriginallyFlipped;
        public Vector2 offset = new Vector2(1.5f, 1f);

        [Tooltip(
            "If this is set, the offset x will be changed base on the sprite flipX setting. e.g. Allow to make the damager alway forward in the direction of sprite")]
        public bool offsetBasedOnSpriteFacing = true;

        public DamagableEvent OnDamageableHit;
        public NonDamagableEvent OnNonDamageableHit;
        public Vector2 size = new Vector2(2.5f, 1f);

        [Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
        public SpriteRenderer spriteRenderer;

        //call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
        public Collider2D LastHit
        {
            get { return m_LastHit; }
        }

        private void Awake()
        {
            m_AttackContactFilter.layerMask = hittableLayers;
            m_AttackContactFilter.useLayerMask = true;
            m_AttackContactFilter.useTriggers = canHitTriggers;

            if (offsetBasedOnSpriteFacing && spriteRenderer != null)
                m_SpriteOriginallyFlipped = spriteRenderer.flipX;

            m_DamagerTransform = transform;
        }

        public void EnableDamage()
        {
            m_CanDamage = true;
        }

        public void DisableDamage()
        {
            m_CanDamage = false;
        }

        private void FixedUpdate()
        {
            if (!m_CanDamage)
                return;

            Vector2 scale = m_DamagerTransform.lossyScale;

            var facingOffset = Vector2.Scale(offset, scale);
            if (offsetBasedOnSpriteFacing && spriteRenderer != null &&
                spriteRenderer.flipX != m_SpriteOriginallyFlipped)
                facingOffset = new Vector2(-offset.x * scale.x, offset.y * scale.y);

            var scaledSize = Vector2.Scale(size, scale);

            var pointA = (Vector2) m_DamagerTransform.position + facingOffset - scaledSize * 0.5f;
            var pointB = pointA + scaledSize;

            var hitCount = Physics2D.OverlapArea(pointA, pointB, m_AttackContactFilter, m_AttackOverlapResults);

            for (var i = 0; i < hitCount; i++)
            {
                m_LastHit = m_AttackOverlapResults[i];
                var damageable = m_LastHit.GetComponent<Damageable>();

                if (damageable)
                {
                    OnDamageableHit.Invoke(this, damageable);
                    damageable.TakeDamage(this, ignoreInvincibility);
                    if (disableDamageAfterHit)
                        DisableDamage();
                }
                else
                {
                    OnNonDamageableHit.Invoke(this);
                }
            }
        }

        [Serializable]
        public class DamagableEvent : UnityEvent<Damager, Damageable>
        {
        }


        [Serializable]
        public class NonDamagableEvent : UnityEvent<Damager>
        {
        }
    }
}