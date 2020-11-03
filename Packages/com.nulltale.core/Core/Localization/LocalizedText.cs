using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private LocalizedString     m_String;

        public string               Key
        {
            set
            {
                m_String.Key = value;
                Localize();
            }
        }

        public string               Data => m_String.Data;


        //////////////////////////////////////////////////////////////////////////
        private void OnValidate()
        {
            Localize();
        }

        public void Start()
        {
            Localize();
        }

        public void OnDestroy()
        {
        }

        private void Localize()
        {
            if (m_String.IsEmpty)
                return;

            if (TryGetComponent(out Text text))
                text.text = m_String;
            else
            if (TryGetComponent(out TMP_Text tmpText))
                tmpText.text = m_String;
        }
    }
}