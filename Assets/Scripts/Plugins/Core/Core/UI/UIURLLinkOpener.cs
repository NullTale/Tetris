using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UIURLLinkOpener : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text		        m_Text;

        [SerializeField]
        private SoundPlayer             soundPlayer;
        [SerializeField]
        private string                  m_Click;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Text = GetComponentInChildren<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
		
            //var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, Input.mousePosition, m_Camera == null ? null : m_Camera);

            var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, Input.mousePosition, 
                m_Text.canvas == null 
                    ? (global::Core.Core.Instance.Camera != null 
                        ? global::Core.Core.Instance.Camera 
                        : Camera.main)
                    : (m_Text.canvas.renderMode == RenderMode.ScreenSpaceOverlay 
                        ? null 
                        : m_Text.canvas.worldCamera)
            );
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkIndex];

                // play sound
                soundPlayer?.Play(m_Click);

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}