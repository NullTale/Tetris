using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpriteOrderer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer      m_SpriteRenderer;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            if (m_SpriteRenderer == null)
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            m_SpriteRenderer.sortingOrder = (int)(transform.position.y * 128.0f);
        }
    }
}