using System.Collections.Generic;
using System.Linq;
using Core.Effect;
using NaughtyAttributes;
using UnityEngine;

public class ColorizeEffectModule : ModuleUpdatable
{
    private List<BlockVisualizer>   m_Blocks;
    [SerializeField]
    private Color                   m_Color;
    [SerializeField] [Range(0.0f, 1.0f)]
    private float                   m_Progress;
    private float                   m_LastScale;

    //////////////////////////////////////////////////////////////////////////
    private class BlockData
    {
        private GameObject       m_GameObject;
        private SpriteRenderer   m_SpriteRenderer;
        private Color            m_InitialColor;

        //////////////////////////////////////////////////////////////////////////
        public void Update(float scale, in Color color)
        {
            if (m_GameObject != null)
                m_SpriteRenderer.color = Color.LerpUnclamped(m_InitialColor, color, scale);
        }

        public BlockData(GameObject gameObject)
        {
            m_GameObject = gameObject;
            m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            m_InitialColor = m_SpriteRenderer.color;
        }
    }

    private List<BlockData>     m_BlockDataList;

    //////////////////////////////////////////////////////////////////////////
    public Effect Set(List<BlockVisualizer> blocks)
    {
        m_Blocks = blocks;
        m_BlockDataList = m_Blocks.Select(n => new BlockData(n.View.gameObject)).ToList();

        return Effect;
    }

    public override void Begin()
    {
        base.Begin();
    }

    protected override void _Update()
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (m_Progress == m_LastScale)
            return;

        foreach (var blockData in m_BlockDataList)
            blockData.Update(m_Progress, m_Color);

        m_LastScale = m_Progress;
    }
}