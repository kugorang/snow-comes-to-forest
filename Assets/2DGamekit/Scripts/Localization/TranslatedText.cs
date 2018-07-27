#region

using TMPro;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class TranslatedText : MonoBehaviour
    {
        public string phraseKey;
        public bool setTextOnStart = true;
        public TextMeshProUGUI text;

        private void Awake()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (setTextOnStart)
                text.text = Translator.Instance[phraseKey];
        }

        public void SetText()
        {
            text.text = Translator.Instance[phraseKey];
        }
    }
}