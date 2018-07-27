#region

using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

#endregion

namespace Gamekit2D
{
    public class PressurePad : MonoBehaviour
    {
        public enum ActivationType
        {
            ItemCount,
            ItemMass
        }

        //bug in 17.3 make rigidbody loose all contacts when sprites of different size/pivot are swapped in spriterenderer
        //so we delay (de)activation to "ignore" any outlier single frame problem 
        private static readonly int DELAYEDFRAME_COUNT = 2;
        public Sprite activatedBoxSprite;
        public ActivationType activationType;
        public SpriteRenderer[] boxes;
        public Sprite deactivatedBoxSprite;
        protected int m_ActivationFrameCount;

        protected bool m_EventFired;
        protected bool m_PreviousWasPressed;
        public UnityEvent OnPressed;
        public UnityEvent OnRelease;

        public PlatformCatcher platformCatcher;
        public int requiredCount;
        public float requiredMass;

        private void FixedUpdate()
        {
            if (activationType == ActivationType.ItemCount)
            {
                if (platformCatcher.CaughtObjectCount >= requiredCount)
                {
                    if (!m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = true;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }

                    if (m_ActivationFrameCount > DELAYEDFRAME_COUNT && !m_EventFired)
                    {
                        OnPressed.Invoke();
                        m_EventFired = true;
                    }
                }
                else
                {
                    if (m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = false;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }

                    if (m_ActivationFrameCount > DELAYEDFRAME_COUNT && m_EventFired)
                    {
                        OnRelease.Invoke();
                        m_EventFired = false;
                    }
                }
            }
            else
            {
                if (platformCatcher.CaughtObjectsMass >= requiredMass)
                {
                    if (!m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = true;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }


                    if (m_ActivationFrameCount > DELAYEDFRAME_COUNT && !m_EventFired)
                    {
                        OnPressed.Invoke();
                        m_EventFired = true;
                    }
                }
                else
                {
                    if (m_PreviousWasPressed)
                    {
                        m_PreviousWasPressed = false;
                        m_ActivationFrameCount = 1;
                    }
                    else
                    {
                        m_ActivationFrameCount += 1;
                    }

                    if (m_ActivationFrameCount > DELAYEDFRAME_COUNT && m_EventFired)
                    {
                        OnRelease.Invoke();
                        m_EventFired = false;
                    }
                }
            }

            for (var i = 0; i < boxes.Length; i++)
                boxes[i].sprite = platformCatcher.HasCaughtObject(boxes[i].gameObject)
                    ? activatedBoxSprite
                    : deactivatedBoxSprite;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var rb = GetComponentInChildren<Rigidbody2D>();
            if (rb == null)
                return;

            if (rb.bodyType == RigidbodyType2D.Static && GetComponentInParent<MovingPlatform>() != null)
            {
                errorStyle.alignment = TextAnchor.MiddleLeft;
                errorStyle.fontSize = Mathf.FloorToInt(18 * (1.0f / HandleUtility.GetHandleSize(transform.position)));
                errorStyle.normal.textColor = Color.white;

                Handles.Label(transform.position + Vector3.up * 1.5f + Vector3.right,
                    "ERROR : Rigidbody body type on that pressure plate is set to Static!\n It won't move with the moving platform. Change it to Kinematic.",
                    errorStyle);

                Handles.color = Color.red;
                Handles.DrawWireDisc(transform.position, Vector3.back, 0.5f);
                Handles.color = Color.white;
                Handles.DrawLine(transform.position + Vector3.up * 1.0f + Vector3.right, transform.position);
            }
        }
#endif

#if UNITY_EDITOR
        protected GUIStyle errorStyle = new GUIStyle();
        protected GUIStyle errorBackgroundStyle = new GUIStyle();
#endif
    }
}