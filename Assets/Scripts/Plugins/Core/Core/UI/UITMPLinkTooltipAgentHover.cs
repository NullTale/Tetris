using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UITMPLinkTooltipAgentHover : UITMPLinkTooltip.UITMPLinkTooltipAgent, IPointerEnterHandler, IPointerExitHandler
    {
        public UITMPLinkTooltip		m_UITMPLinkTooltip;
        public TextMeshProUGUI		m_Text;
        private Camera				m_Camera;
        private OnUpdateCallback	m_OnUpdate;
        private int					m_LinkID;

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

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_OnUpdate == null)
            {
                m_OnUpdate = gameObject.AddComponent<OnUpdateCallback>();
                m_OnUpdate.hideFlags = HideFlags.HideInInspector;
                m_LinkID = -1;

                m_OnUpdate.Action = () =>
                {
                    var currentLinkID = TMP_TextUtilities.FindIntersectingLink(m_Text, global::Core.Core.Instance.MouseWorldPosition.ScreenPosition, m_Camera);
                    if (currentLinkID == -1)
                    {	// disable tooltip
                        m_LinkID = currentLinkID;
                        m_UITMPLinkTooltip.Hide();
                    }

                    if (currentLinkID != m_LinkID)
                    {	// show tooltip
                        m_LinkID = currentLinkID;

                        var linkInfo = m_Text.textInfo.linkInfo[currentLinkID];
                        m_UITMPLinkTooltip.Show(linkInfo.GetLinkID());
                    }
                };
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

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_OnUpdate != null)
            {
                Destroy(m_OnUpdate);
                m_OnUpdate = null;
            }
            m_UITMPLinkTooltip.Hide();
        }
    }
}