using System;
using Malee;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class LocalizedDropdown : MonoBehaviour
    {
        [Serializable]
        public class OptionList : ReorderableArray<LocalizedString> {}

        //////////////////////////////////////////////////////////////////////////
        [SerializeField]
        private OptionList     m_Options;

        //////////////////////////////////////////////////////////////////////////
        private void OnValidate()
        {
            Localize();
        }

        public void Start()
        {
            Localize();
        }

        private void Localize()
        {
            if (m_Options.IsEmpty())
                return;

            if (TryGetComponent(out Dropdown dropdown))
            {
                var options = dropdown.options;
                for (var n = 0; n < Mathf.Min(m_Options.Count, options.Count); n++)
                    options[n].text = m_Options[n].Data;

                if (m_Options.Count >= dropdown.value)
                    dropdown.captionText.text = m_Options[dropdown.value].Data;
            }
            else
            if (TryGetComponent(out TMP_Dropdown tmpDropdown))
            {
            
                var options = tmpDropdown.options;
                for (var n = 0; n < Mathf.Min(m_Options.Count, options.Count); n++)
                    options[n].text = m_Options[n].Data;

                if (m_Options.Count >= tmpDropdown.value)
                    tmpDropdown.captionText.text = m_Options[tmpDropdown.value].Data;
            }
        }
    }
}