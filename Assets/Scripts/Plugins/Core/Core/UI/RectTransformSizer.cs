using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class RectTransformSizer
    {
        public RectTransform	m_RectTransform;
        public float			m_PaddingLeft;
        public float            m_PaddingRight;
        public float            m_PaddingTop;
        public float			m_PaddingBottom;

        [NonSerialized]
        public float			m_ApplyedWidth;
        [NonSerialized]
        public float			m_ApplyedHeight;

        public float			ApplyedHeight => m_ApplyedHeight;
        public float			ApplyedWidth => m_ApplyedWidth;

        //////////////////////////////////////////////////////////////////////////
        public void Apply(float width, float height)
        {
            m_ApplyedWidth = width + m_PaddingLeft + m_PaddingRight;
            m_ApplyedHeight = height + m_PaddingTop + m_PaddingBottom;

            var newFieldSize = new Vector2(m_ApplyedWidth, m_ApplyedHeight);
            m_RectTransform.sizeDelta = newFieldSize;
        }

        public void ApplyWidth(float width)
        {
            m_ApplyedWidth = width + m_PaddingLeft + m_PaddingRight;
            m_ApplyedHeight = m_RectTransform.sizeDelta.y;

            var newFieldSize = new Vector2(m_ApplyedWidth, m_ApplyedHeight);
            m_RectTransform.sizeDelta = newFieldSize;
        }

        public void ApplyHeightWidth(float height)
        {
            m_ApplyedWidth = m_RectTransform.sizeDelta.x;
            m_ApplyedHeight = height + m_PaddingTop + m_PaddingBottom;

            var newFieldSize = new Vector2(m_ApplyedWidth, m_ApplyedHeight);
            m_RectTransform.sizeDelta = newFieldSize;
        }
    }
}