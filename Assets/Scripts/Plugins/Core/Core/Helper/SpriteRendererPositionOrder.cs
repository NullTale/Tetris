using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(1)]
    public class SpriteRendererPositionOrder : MonoBehaviour
    {
        public List<InitialData>		m_SpriteRendererList = new List<InitialData>();

        public struct InitialData
        {
            public SpriteRenderer	m_SpriteRenderer;
            public int				m_InitialOrder;
        }

        //////////////////////////////////////////////////////////////////////////
        void Start()
        {
            foreach (var n in GetComponentsInChildren<SpriteRenderer>())
                m_SpriteRendererList.Add(new InitialData(){ m_SpriteRenderer = n, m_InitialOrder = n.sortingOrder});
        }

        void Update()
        {
            int positionOrder = (int)(-transform.position.y * 100.0f);
            foreach (var n in m_SpriteRendererList)
                n.m_SpriteRenderer.sortingOrder = n.m_InitialOrder + positionOrder;
        }
    }
}