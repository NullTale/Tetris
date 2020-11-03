using System;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(Localization), menuName = Core.c_CoreModuleMenu + nameof(Localization))]
    public class Localization : Core.Module
    {
        public string					m_LocalizationPath = "Localization";
        public string					m_Language = "Auto";//"en-US";
        private LocalizationManager     m_Manager;

        public LocalizationManager      Manager => m_Manager;
        
        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            if (m_Manager == null)
                m_Manager = new LocalizationManager();

            // read dictionary
            m_Manager.Read(m_LocalizationPath, true);

            // set or autodetect language
            if (string.IsNullOrEmpty(m_Language) || m_Language == "Auto")
            {
                // autodetect
                var lang = string.Empty;
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        lang = "en-US";
                        break;
                    case SystemLanguage.Russian:
                        lang = "ru-RU";
                        break;
                    default:    
                        lang = "en-US"; 
                        break;
                }

                m_Manager.Language = lang;
            }
            else
            {
                // set
                m_Manager.Language = m_Language;
            }
        }

#if UNITY_EDITOR
        public LocalizationManager GetEditorLocalizationManager()
        {
            if (m_Manager == null)
            {
                // instantiate
                m_Manager = new LocalizationManager();
                
                // read dictionary
                m_Manager.Read(m_LocalizationPath, true);
            }

            return m_Manager;
        }
#endif
    }
}