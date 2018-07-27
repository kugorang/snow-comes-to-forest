#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class Translator : MonoBehaviour
    {
        protected static Translator s_Instance;

        [SerializeField] protected int m_LanguageIndex;

        public List<OriginalPhrases> phrases = new List<OriginalPhrases>();

        public static Translator Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindObjectOfType<Translator>();

                if (s_Instance != null)
                    return s_Instance;

                return CreateDefault();
            }
        }

        public static string CurrentLanguage
        {
            get { return Instance.phrases[Instance.m_LanguageIndex].language; }
        }

        public string this[string key]
        {
            get { return phrases[m_LanguageIndex][key]; }
        }

        private static Translator CreateDefault()
        {
            var prefab = Resources.Load<Translator>("Translator");
            var defaultInstance = Instantiate(prefab);
            return defaultInstance;
        }

        public static bool SetLanguage(int index)
        {
            if (index >= Instance.phrases.Count || index < 0)
                return false;

            Instance.m_LanguageIndex = index;
            return true;
        }

        public static bool SetLanguage(string language)
        {
            for (var i = 0; i < Instance.phrases.Count; i++)
                if (Instance.phrases[i].language == language)
                {
                    Instance.m_LanguageIndex = i;
                    return true;
                }

            return false;
        }

        public static void SetLanguage(TranslatedPhrases phrases)
        {
            for (var i = 0; i < Instance.phrases.Count; i++)
                if (Instance.phrases[i] == phrases)
                {
                    Instance.m_LanguageIndex = i;
                    return;
                }

            Instance.phrases.Add(phrases);
            Instance.m_LanguageIndex = Instance.phrases.Count - 1;
        }
    }
}