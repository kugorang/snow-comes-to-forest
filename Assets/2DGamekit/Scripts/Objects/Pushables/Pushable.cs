#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Pushable : MonoBehaviour
    {
        private static readonly ContactPoint2D[] s_ContactPointBuffer = new ContactPoint2D[16];

        private static readonly Dictionary<Collider2D, Pushable> s_PushableCache =
            new Dictionary<Collider2D, Pushable>();

        public AudioClip endPushClip;
        public AudioClip loopPushClip;
        protected bool m_Grounded;
        protected Rigidbody2D m_Rigidbody2D;

        protected SpriteRenderer m_SpriteRenderer;
        private Collider2D[] m_WaterColliders;
        public Transform playerPushingLeftPosition;

        public Transform playerPushingRightPosition;

        public AudioSource pushableAudioSource;
        public Transform pushablePosition;
        public AudioClip startingPushClip;

        public bool Grounded
        {
            get { return m_Grounded; }
        }

        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            if (s_PushableCache.Count == 0)
            {
                var pushables = FindObjectsOfType<Pushable>();

                for (var i = 0; i < pushables.Length; i++)
                {
                    var pushableColliders = pushables[i].GetComponents<Collider2D>();

                    for (var j = 0; j < pushableColliders.Length; j++)
                        s_PushableCache.Add(pushableColliders[j], pushables[i]);
                }
            }

            var waterAreas = FindObjectsOfType<WaterArea>();
            m_WaterColliders = new Collider2D[waterAreas.Length];
            for (var i = 0; i < waterAreas.Length; i++) m_WaterColliders[i] = waterAreas[i].GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            var velocity = m_Rigidbody2D.velocity;
            velocity.x = 0f;
            m_Rigidbody2D.velocity = velocity;

            CheckGrounded();

            for (var i = 0; i < m_WaterColliders.Length; i++)
                if (m_Rigidbody2D.IsTouching(m_WaterColliders[i]))
                    m_Rigidbody2D.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }

        public void StartPushing()
        {
            pushableAudioSource.loop = false;
            pushableAudioSource.clip = startingPushClip;
            pushableAudioSource.Play();
        }

        public void EndPushing()
        {
            pushableAudioSource.loop = false;
            pushableAudioSource.clip = endPushClip;
            pushableAudioSource.Play();
        }

        public void Move(Vector2 movement)
        {
            m_Rigidbody2D.position = m_Rigidbody2D.position + movement;

            if (!pushableAudioSource.isPlaying)
            {
                pushableAudioSource.clip = loopPushClip;
                pushableAudioSource.loop = true;
                pushableAudioSource.Play();
            }
        }

        protected void CheckGrounded()
        {
            m_Grounded = false;

            var count = m_Rigidbody2D.GetContacts(s_ContactPointBuffer);
            for (var i = 0; i < count; ++i)
                if (s_ContactPointBuffer[i].normal.y > 0.9f)
                {
                    m_Grounded = true;

                    Pushable pushable;

                    if (s_PushableCache.TryGetValue(s_ContactPointBuffer[i].collider, out pushable))
                        m_SpriteRenderer.sortingOrder = pushable.m_SpriteRenderer.sortingOrder + 1;
                }
        }
    }
}