using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Effect;
using UnityEngine;

public class CollapseEffectModule : ModuleUpdatable
{
    [Serializable]
    public enum Method
    {
        FromCenter,
        FromSides,
        Immediate,
        Random,
        FromLeft,
        FromRight,
        FromTop,
        FromButtom,
        //FromRandomBlock
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField, Range(0.0f, 1.0f)]
    private float       m_Progress;
    [SerializeField]
    private Method      m_Method;
    private float       m_BoardLenght;
    private Vector2     m_CollapseRange;
    private int         m_InitialCount;

    private Dictionary<BlockVisualizer, BlockVisualizer[]>            m_Collapse;

    //////////////////////////////////////////////////////////////////////////
    public Effect Set(HashSet<BlockVisualizer> collapse, HashSet<BlockVisualizer> blocks, Method method)
    {
        m_Method = method;
        return Set(collapse, blocks);
    }

    public Effect Set(HashSet<BlockVisualizer> collapse, HashSet<BlockVisualizer> blocks)
    {
        // save values
        m_Collapse = collapse.ToDictionary(key => key, toCollapse => blocks.Where(block => block.Position.x == toCollapse.Position.x && block.Position.y > toCollapse.Position.y).ToArray());

        m_InitialCount = collapse.Count;

        // move all top blocks down
        foreach (var block in m_Collapse.Values.SelectMany(n => n))
        {
            block.Position += Vector2Int.down;
            block.PositionOffset += Vector2.up;
        }

        return Effect;
    }

    public override void Begin()
    {
        m_BoardLenght = TetrisManager.Instance.GameManager.BoardManager.NumberOfColumns * 1.0f;
        m_CollapseRange = new Vector2(m_Collapse.Keys.Min(n => n.Anchor.transform.localPosition.y) - 0.5f, m_Collapse.Keys.Max(n => n.Anchor.transform.localPosition.y) + 0.5f);

        

        base.Begin();
    }

    protected override void _Update()
    {
        List<BlockVisualizer> toDestroy = null;

        // select blocks by method
        switch (m_Method)
        {
            case Method.FromCenter:
            {
                var halfLenght = m_BoardLenght * 0.5f;
                var deadRange = new Vector2(halfLenght - halfLenght * m_Progress, halfLenght + halfLenght * m_Progress);

                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.x)).ToList();
            } break;
            case Method.FromSides:
            {
                var halfLenght = m_BoardLenght * 0.5f;
                var deadRange = new Vector2(halfLenght - halfLenght * (1.0f - m_Progress), halfLenght + halfLenght * (1.0f - m_Progress));

                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.x) == false).ToList();
            } break;
            case Method.Immediate:
            {
                if (m_Progress >= 1.0f)
                    toDestroy = m_Collapse.Keys.ToList();
            } break;
            case Method.Random:
            {
                // how much blocks must contain collapse
                var collapseCount = Mathf.FloorToInt((1.01f - m_Progress) * m_InitialCount);
                var toDestroyCount = m_Collapse.Count - collapseCount;

                if (toDestroyCount == 0)
                    break;

                toDestroy = new List<BlockVisualizer>(toDestroyCount);

                // take random items from collapse
                var collapseList = m_Collapse.Keys.ToList();
                for (var n = 0; n < toDestroyCount; n++)
                {
                    var randomItem = collapseList.RandomItem();
                    collapseList.Remove(randomItem);
                    toDestroy.Add(randomItem);
                }
            } break;
            case Method.FromLeft:
            {
                var deadRange = new Vector2(0.0f, m_BoardLenght * m_Progress);
                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.x)).ToList();

            } break;
            case Method.FromRight:
            {
                var deadRange = new Vector2(m_BoardLenght * (1.0f - m_Progress), m_BoardLenght);
                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.x)).ToList();
            } break;
            case Method.FromTop:
            {
                var deadRange = new Vector2(m_CollapseRange.x + (m_CollapseRange.y - m_CollapseRange.x) * (1.0f - m_Progress), m_CollapseRange.y);
                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.y)).ToList();
            } break;
            case Method.FromButtom:
            {
                var deadRange = new Vector2(m_CollapseRange.x + (m_CollapseRange.y - m_CollapseRange.x) * m_Progress, m_CollapseRange.y);
                toDestroy = m_Collapse.Keys.Where(n => deadRange.InRangeOfInc(n.Anchor.transform.localPosition.y)).ToList();
            } break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // skip if nothing
        if (toDestroy == null || toDestroy.Count == 0)
            return;

        // destroy blocks
        foreach (var block in toDestroy)
        {
            foreach (var toMoveDown in m_Collapse[block])
                toMoveDown.PositionOffset += Vector2.down;

            m_Collapse.Remove(block);

            block.Destroy();
        }
    }
}
