#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioPlayer : MonoBehaviour
    {
        public AudioClip[] clips;
        protected Dictionary<TileBase, AudioClip[]> m_LookupOverride;

        protected AudioSource m_Source;

        public TileOverride[] overrides;
        public float pitchRange = 0.2f;

        public bool randomizePitch;

        private void Awake()
        {
            m_Source = GetComponent<AudioSource>();
            m_LookupOverride = new Dictionary<TileBase, AudioClip[]>();

            for (var i = 0; i < overrides.Length; ++i)
            {
                if (overrides[i].tile == null)
                    continue;

                m_LookupOverride[overrides[i].tile] = overrides[i].clips;
            }
        }

        public void PlayRandomSound(TileBase surface = null)
        {
            var source = clips;

            AudioClip[] temp;
            if (surface != null && m_LookupOverride.TryGetValue(surface, out temp))
                source = temp;

            var choice = Random.Range(0, source.Length);

            if (randomizePitch)
                m_Source.pitch = Random.Range(1.0f - pitchRange, 1.0f + pitchRange);

            m_Source.PlayOneShot(source[choice]);
        }

        public void Stop()
        {
            m_Source.Stop();
        }

        [Serializable]
        public struct TileOverride
        {
            public TileBase tile;
            public AudioClip[] clips;
        }
    }
}