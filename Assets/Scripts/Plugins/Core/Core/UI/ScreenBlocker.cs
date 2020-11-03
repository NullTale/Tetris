using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core
{
    public class ScreenBlocker
    {
        public const int				c_CanvasSortingOrder = 29999;
        private GameObject				m_Blocker;

        //////////////////////////////////////////////////////////////////////////
        public void Create(Action onClick, GameObject caller, int canvasSortingOrder = c_CanvasSortingOrder)
        {
            // recreate blocker in code
            Release();
            m_Blocker = new GameObject("blocker", typeof(RectTransform), typeof(Button), typeof(Canvas), typeof(CanvasRenderer), typeof(GraphicRaycaster), typeof(Image));
		
            var rectTransform = m_Blocker.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.SetParent(implTopMostComponent<Canvas>(caller).transform, false);

            var canvas = m_Blocker.GetComponent<Canvas>();
            //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.overrideSorting = true;
            canvas.sortingOrder = canvasSortingOrder;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var image = m_Blocker.GetComponent<Image>();
            image.color = Color.clear;

            var button = m_Blocker.GetComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation(){mode = Navigation.Mode.None};
            button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void Release()
        {
            if (m_Blocker != null)
            {
                Object.Destroy(m_Blocker);
                m_Blocker = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private T implTopMostComponent<T>(GameObject caller) where T : Component
        {	// get top most component of type

            // current can contain component
            var result = caller.GetComponent<T>();
            var current = caller;

            // go at top of the hierarchy
            while (true)
            {
                var comp = current.transform.parent?.GetComponentInParent<T>();
                if (comp != null)
                {
                    result = comp;
                    current = comp.gameObject;
                }
                else
                    break;
            }

            return result;
        }
    }
}