using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteRendererColorLerp : MonoBehaviour
{
    private SpriteRenderer      m_SpriteRenderer;
    [SerializeField]
    private Color               m_Color;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float               m_Lerp;
    private Color               m_InitialColor;
    [SerializeField]
    private bool                m_RestoreColor;

    //////////////////////////////////////////////////////////////////////////
    private void OnEnable()
    {
        if (m_SpriteRenderer == null)
            if (TryGetComponent(out m_SpriteRenderer) == false)
                enabled = false;
            else
            {
                // save as initial
                m_InitialColor = m_SpriteRenderer.color;
            }
    }

    private void OnDisable()
    {
        if (m_SpriteRenderer != null)
        {
            if (m_RestoreColor)
                m_SpriteRenderer.color = m_InitialColor;
            m_SpriteRenderer = null;
        }
    }

    public void Update()
    {
        // apply
        m_SpriteRenderer.color = Color.LerpUnclamped(m_InitialColor, m_Color, m_Lerp);
    }
}
