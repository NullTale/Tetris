using System;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    [RequireComponent(typeof(TMP_Text))]
    public class UILinkActivator : MonoBehaviour, IPointerClickHandler 
    {
        private TMP_Text		m_Text;
        [SerializeField]
        private EventDictionary	m_EventDictionary;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class EventDictionary: SerializableDictionaryBase<string, ActivationEvent> {}

        [Serializable]
        public class ActivationEvent : UltEvent<GameObject> {}

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, eventData.position
                , 
                m_Text.canvas == null 
                    ? (global::Core.Core.Instance.Camera != null								// text doesn't use canvas set world camera
                        ? global::Core.Core.Instance.Camera 
                        : Camera.main)
                    : (m_Text.canvas.renderMode == RenderMode.ScreenSpaceOverlay	// text use canvas, if ScreenSpaceOverlay set null, is not use canvas camera
                        ? null 
                        : m_Text.canvas.worldCamera)
            );

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkIndex];

                // run event
                if (m_EventDictionary.TryGetValue(linkInfo.GetLinkID(), out var ultEvent))
                    ultEvent.Invoke(gameObject);
                else
                    Debug.LogWarning("Link value not presented in the dictionary");
            }
        }
    }
}