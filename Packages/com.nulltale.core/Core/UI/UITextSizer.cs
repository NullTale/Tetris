using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    [ExecuteInEditMode, DefaultExecutionOrder(2)]
    public class UITextSizer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text            m_Text;
        [SerializeField]         
        private Vector2             m_Padding;
        [SerializeField]         
        private Vector2             m_MaxSize = new Vector2(1000, float.PositiveInfinity);
        [SerializeField]         
        private Vector2             m_MinSize;
        [SerializeField]         
        private Mode                m_ControlAxes = Mode.XY;
        [SerializeField]         
        private bool                m_ResizeTextObject;
 
        private RectTransform       m_TextRectTransform;
        private RectTransform       m_SelfRectTransform;

        private bool                m_Lock;
        private bool                m_Registered;

        protected virtual float MinX 
        { 
            get 
            {
                if (m_ControlAxes.HasFlag(Mode.X)) 
                    return m_MinSize.x;
                return m_SelfRectTransform.rect.width - m_Padding.x;
            }
        }
        protected virtual float MinY 
        { 
            get 
            {
                if (m_ControlAxes.HasFlag(Mode.Y)) 
                    return m_MinSize.y;
                return m_SelfRectTransform.rect.height - m_Padding.y;
            }
        }
        protected virtual float MaxX 
        {
            get 
            {
                if (m_ControlAxes.HasFlag(Mode.X))
                    return m_MaxSize.x;
                return m_SelfRectTransform.rect.width - m_Padding.x;
            }
        }
        protected virtual float MaxY 
        { 
            get 
            {
                if (m_ControlAxes.HasFlag(Mode.Y)) 
                    return m_MaxSize.y;
                return m_SelfRectTransform.rect.height - m_Padding.y;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        [Flags]
        public enum Mode
        {
            None        = 0,
            X           = 1,
            Y           = 1 << 1,
            XY          = X | Y
        }

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            if (m_Text == null)
                return;

            m_TextRectTransform = m_Text.GetComponent<RectTransform>();
            m_SelfRectTransform = GetComponent<RectTransform>();

            m_Text.RegisterDirtyVerticesCallback(Refresh);
            m_Registered = true;
        }

        private void OnDisable()
        {
            if (m_Registered)
            {
                m_Text.UnregisterDirtyVerticesCallback(Refresh);
                m_Registered = false;
            }
        }

        private void OnValidate()
        {
            if (m_SelfRectTransform == null || m_TextRectTransform == null)
                return;

            Refresh();
        }

        public void Refresh()
        {
            if (m_Lock)
                return;

            m_Lock = true;
            
            if (enabled == false)
                return;

            if (m_Text == null)
                return;

            var preferredSize = m_Text.GetPreferredValues(MaxX, MaxY);
            preferredSize.x = Mathf.Clamp(preferredSize.x, MinX, MaxX);
            preferredSize.y = Mathf.Clamp(preferredSize.y, MinY, MaxY);
            preferredSize += m_Padding;

            if ((m_ControlAxes & Mode.X) != 0)
            {
                m_SelfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
                if (m_ResizeTextObject)
                    m_TextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
            }
            if ((m_ControlAxes & Mode.Y) != 0)
            {
                m_SelfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
                if (m_ResizeTextObject)
                    m_TextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
            }

            m_Lock = false;
        }
    }
}