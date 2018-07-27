#region

using System;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Gamekit2D
{
    [Serializable]
    public class ScrollingTextBehaviour : PlayableBehaviour
    {
        public float holdDelay;

        protected float m_Duration;
        protected float m_InverseScrollingDuration;
        public string message;
        public float startDelay;

        public override void OnGraphStart(Playable playable)
        {
            m_Duration = (float) playable.GetDuration();
            var scrollingDuration = Mathf.Clamp(m_Duration - holdDelay - startDelay, float.Epsilon, m_Duration);
            m_InverseScrollingDuration = 1f / scrollingDuration;
        }

        public string GetMessage(float localTime)
        {
            localTime = Mathf.Clamp(localTime - startDelay, 0f, m_Duration);
            var messageProportion = Mathf.Clamp01(localTime * m_InverseScrollingDuration);
            var characterCount = Mathf.FloorToInt(message.Length * messageProportion);
            return message.Substring(0, characterCount);
        }
    }
}