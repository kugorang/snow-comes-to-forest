#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CreateAssetMenu]
    public class OriginalPhrases : ScriptableObject
    {
        public string language;
        public List<Phrase> phrases = new List<Phrase>();

        public string this[string key]
        {
            get
            {
                for (var i = 0; i < phrases.Count; i++)
                    if (phrases[i].key == key)
                        return phrases[i].value;

                return "Key not found.";
            }
        }
    }

    [Serializable]
    public class Phrase
    {
        public string key;
        public string value;
    }
}