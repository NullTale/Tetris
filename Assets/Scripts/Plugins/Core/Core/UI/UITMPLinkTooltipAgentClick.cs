using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UITMPLinkTooltipAgentClick : UITMPLinkTooltip.UITMPLinkTooltipAgent, IPointerClickHandler
    {
        public UITMPLinkTooltip     m_UITMPLinkTooltip;
        public TextMeshProUGUI      m_Text;
        private Camera              m_Camera;
        private OnUpdateCallback    m_OnUpdate;

        //////////////////////////////////////////////////////////////////////////
        public override void Init(UITMPLinkTooltip owner, TextMeshProUGUI text)
        {
            hideFlags = HideFlags.HideInInspector;
            m_UITMPLinkTooltip = owner;
            m_Text = text;
            if (m_Text == null)
                m_Text = GetComponent<TextMeshProUGUI>();

            var canvas = m_Text.canvas;
            m_Camera = canvas != null
                ? canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : global::Core.Core.Instance.Camera
                : global::Core.Core.Instance.Camera;
        }

        private void implHide()
        {
            if (m_OnUpdate != null)
            {
                Destroy(m_OnUpdate);
                m_OnUpdate = null;
                m_UITMPLinkTooltip.Hide();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkID = TMP_TextUtilities.FindIntersectingLink(m_Text, global::Core.Core.Instance.MouseWorldPosition.ScreenPosition, m_Camera);
            if (linkID != -1)
            {
                var linkInfo = m_Text.textInfo.linkInfo[linkID];
                m_UITMPLinkTooltip.Show(linkInfo.GetLinkID());

                if (m_OnUpdate == null)
                {
                    m_OnUpdate = gameObject.AddComponent<OnUpdateCallback>();
                    m_OnUpdate.hideFlags = HideFlags.HideInInspector;
                    m_OnUpdate.Action = () =>
                    {
                        var currentLinkID = TMP_TextUtilities.FindIntersectingLink(m_Text, global::Core.Core.Instance.MouseWorldPosition.ScreenPosition, m_Camera);
                        if (currentLinkID != linkID)
                            implHide();
                    };
                }
            }
        }
	
        private void OnDestroy()
        {
            if (m_OnUpdate != null)
            {
                Destroy(m_OnUpdate);
                m_OnUpdate = null;
            }
        }
    }
}