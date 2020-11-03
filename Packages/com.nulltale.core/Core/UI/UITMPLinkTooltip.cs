using System;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;

namespace Core
{
    public class UITMPLinkTooltip : MonoBehaviour, UITMPLinkTooltip.ITooltipSource
    {
        public TextMeshProUGUI				m_Target;

        [SerializeField]
        private TooltipBehavior				m_TooltipBehavior;

        [Tooltip("Link description tooltip, first calls local, second global")]
        public LinkDisctionary				m_LocalDictionary;
        public UITMPLinkTooltipDictionary	m_AssetDictionary;

        private UITMPLinkTooltipAgent		m_Agent;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class LinkDisctionary : SerializableDictionaryBase<string, string> {};

        public interface ITooltipSource
        {
            string iGetTooltip(string linkID);
        }

        [Serializable]
        public enum TooltipBehavior
        {
            OnClick,
            OnHover,
        }

        public abstract class UITMPLinkTooltipAgent : MonoBehaviour
        {
            public abstract void Init(UITMPLinkTooltip owner, TextMeshProUGUI text);
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            switch (m_TooltipBehavior)
            {
                case TooltipBehavior.OnClick:
                {
                    m_Agent  = m_Target.gameObject.AddComponent<UITMPLinkTooltipAgentClick>();
                    m_Agent.Init(this, m_Target);
                }	break;
                case TooltipBehavior.OnHover:
                {
                    m_Agent = m_Target.gameObject.AddComponent<UITMPLinkTooltipAgentHover>();
                    m_Agent.Init(this, m_Target);
                }	break;
            }
        }

        private void OnDestroy()
        {
            if (m_Agent != null)
            {
                Destroy(m_Agent);
                m_Agent = null;
            }
        }
	
        public void Show(string linkID)
        {
            UITinyTooltip.Instance.Show(iGetTooltip(linkID));
        }

        public void Hide()
        {
            UITinyTooltip.Instance.Hide();
        }

        public string iGetTooltip(string linkID)
        {
            if (m_LocalDictionary.TryGetValue(linkID, out var str))
                return str;

            return m_AssetDictionary?.iGetTooltip(linkID) ?? "unknown";
        }
    }
}